using CbUtils.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

namespace Shuile.UI
{
    public partial class RecordsPanel : MonoBehaviour
    {
        public ScrollContainer scrollContainer;
        public UIMaskContainer uiMaskContainer;

        public GameObject content;
        public float cellInterval;
        private RecordListView _listView;

        public GameObject Content => content ? content : gameObject;

        private void Start()
        {
            PanelContainer.MakeSurePanel(Content.transform as RectTransform);

            _listView = new RecordListView(new RecordListView.Settings { CellInterval = 10 });
            _listView.LoadView();

            uiMaskContainer = new UIMaskContainer(new Vector2(0f, -68f), new Vector2(-957f, -398f));
            uiMaskContainer.LoadView();

            scrollContainer = new ScrollContainer();
            scrollContainer.LoadView();
            PanelContainer.MakeSurePanel(scrollContainer.transform as RectTransform);
            var scrollRect = scrollContainer.scrollRect;
            scrollRect.viewport = uiMaskContainer.transform as RectTransform;
            scrollRect.horizontal = false;

            AutoFitContainer.MakeSureAutoFit(_listView.gameObject);
            _listView.gameObject.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            Content.transform
                .WithChild(uiMaskContainer.transform
                    .WithChild(scrollContainer.transform
                        .WithChild(_listView.transform)));

            scrollRect.content = _listView.gameObject.GetComponent<RectTransform>(); // for bugs...
            scrollRect.verticalNormalizedPosition = 0;

            var mock = Enumerable.Range(0, 30).Select((x, index) =>
            {
                var timeDel = TimeSpan.FromDays(Random.Range(0, 100)) + TimeSpan.FromHours(Random.Range(0, 60)) +
                              TimeSpan.FromMinutes(Random.Range(0, 60));
                return new RecordData { Index = index, SongName = "Song" + x, LastPlayTime = DateTime.Now - timeDel };
            });

            _listView.AddItems(mock);
        }

        private void Update()
        {
            if (_listView.settings.CellInterval != cellInterval)
            {
                _listView.UpdateSettings(new RecordListView.Settings { CellInterval = cellInterval });
            }
        }

        public struct RecordData
        {
            public string SongName;
            public DateTime LastPlayTime;
            public int Index;
        }
    }

    public class PanelContainer : MonoProxy
    {
        protected override IGameObjectProvider gameObjectProvider { get; }

        public PanelContainer(GameObject root)
        {
            MakeSurePanel(root.GetComponent<RectTransform>());
            gameObjectProvider = new ExistingGameObjectProvider(root);
        }

        public static void MakeSurePanel(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }
    }

    public class AutoFitContainer : MonoProxy
    {
        protected override IGameObjectProvider gameObjectProvider { get; }

        public AutoFitContainer(GameObject root)
        {
            MakeSureAutoFit(root);
            gameObjectProvider = new ExistingGameObjectProvider(root);
        }

        public static void MakeSureAutoFit(GameObject root)
        {
            root.GetOrAddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
            var layoutGroup = root.GetOrAddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childScaleHeight = true;
        }
    }

    public class ScrollContainer : MonoProxy
    {
        public ScrollRect scrollRect;

        protected override IGameObjectProvider gameObjectProvider { get; } =
            new FunctionGameObjectProvider(() =>
            {
                var types = new[] { typeof(RectTransform), typeof(ScrollRect) };
                return new GameObject("ScrollContainer", types);
            });

        protected override void AfterLoadView()
        {
            base.AfterLoadView();
            scrollRect = gameObject.GetComponent<ScrollRect>();
        }
    }

    public class UIMaskContainer : MonoProxy
    {
        protected override IGameObjectProvider gameObjectProvider { get; }

        public UIMaskContainer(Vector2 origin, Vector2 sizeDelta)
        {
            gameObjectProvider = new FunctionGameObjectProvider(() =>
            {
                var types = new[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask) };
                var inst = new GameObject("UIMaskContainer", types);

                var img = inst.GetComponent<Image>();
                img.color = new Color(1, 1, 1, 1);
                img.sprite = null;

                inst.GetComponent<Mask>().showMaskGraphic = false;
                var rectTransform = inst.GetComponent<RectTransform>();
                PanelContainer.MakeSurePanel(rectTransform);

                rectTransform.anchoredPosition = origin;
                rectTransform.sizeDelta = sizeDelta;

                return inst;
            });
        }

        public void Add(GameObject obj)
        {
            obj.ChangeParent(transform);
        }
    }

    public partial class RecordsPanel
    {
        public class RecordListView : MonoProxy
        {
            private GameObject _root;
            private Settings _settings;

            public Settings settings => _settings;

            private List<RecordListCell> _instanceList = new();

            public RecordListView(Settings settings)
            {
                gameObjectProvider = new FunctionGameObjectProvider(() => new GameObject("ListView", typeof(RectTransform)));
                _settings = settings;
            }

            protected override void AfterLoadView()
            {
                base.AfterLoadView();
                _root = gameObject;
            }

            public void UpdateSettings(Settings se)
            {
                _settings = se;
                for (var i = 0; i < _instanceList.Count; i++)
                {
                    var cellInterval = _settings.CellInterval * Vector3.down;
                    var offset = _settings.Offset * Vector3.down;
                    var pos = offset + i * cellInterval;
                    _instanceList[i].transform.localPosition = pos;
                }

                if (_root.TryGetComponent<LayoutGroup>(out var group))
                {
                    LayoutRebuilder.MarkLayoutForRebuild(group.transform as RectTransform);
                }
            }

            public void AddItem(RecordData data)
            {
                var inst = new RecordListCell();
                var view = inst.LoadView();
                _root.transform.WithChild(view.transform);
                _instanceList.Add(inst);

                var index = _instanceList.Count;

                // handle layout
                var origin = _settings.Offset * Vector3.down;
                origin.y -= _settings.CellInterval * index;
                view.transform.localPosition = origin;

                // fill data
                inst.FillData(data);
            }

            public void AddItems(IEnumerable<RecordData> data)
            {
                foreach (var recordData in data)
                {
                    AddItem(recordData);
                }
            }

            public struct Settings
            {
                public float Offset;
                public float CellInterval;
            }

            protected override IGameObjectProvider gameObjectProvider { get; }
        }

        public class RecordListCell : MonoProxy
        {
            public TextMeshProUGUI playTimeDelta;

            protected override IGameObjectProvider gameObjectProvider { get; }
                = new InstantiateGameObjectProvider(GameApplication.BuiltInData.GetFromPrefabArray("RecordListCell"));

            protected override void AfterLoadView()
            {
                gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("enter detail");
                });
            }

            public void FillData(RecordData data)
            {
                var trans = gameObject.transform;
                trans.GetChildByName<TextMeshProUGUI>("index").text = data.Index.ToString();
                trans.GetChildByName<TextMeshProUGUI>("songName").text = data.SongName;

                playTimeDelta = trans.GetChildByName<TextMeshProUGUI>("playTimeDelta");
                UpdateTimeText(DateTime.Now - data.LastPlayTime);
            }

            private void UpdateTimeText(TimeSpan delta)
            {
                var del = delta;
                string timeDeltaText;
                var num = 0;
                var isJustNow = false;
                if (del.Days > 0)
                {
                    timeDeltaText = $"{del.Days}d";
                    num = del.Days;
                }
                else if (del.Hours > 0)
                {
                    timeDeltaText = $"{del.Hours}h";
                    num = del.Hours;
                }
                else if (del.Minutes > 0)
                {
                    timeDeltaText = $"{del.Minutes}m";
                    num = del.Minutes;
                }
                else
                {
                    timeDeltaText = "Just now";
                    isJustNow = true;
                }

                if (isJustNow) return;
                if (num > 1) timeDeltaText += "s";
                timeDeltaText += " ago";
                playTimeDelta.text = timeDeltaText;
            }
        }
    }
}
