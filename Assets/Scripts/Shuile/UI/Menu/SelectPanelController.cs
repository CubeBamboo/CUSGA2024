using CbUtils.Kits.Tasks;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay;
using Shuile.Gameplay.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Menu
{
    public class SelectPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_Return;
        [SerializeField] private Button btn_Next;
        [SerializeField] private Button btn_Prev;
        [SerializeField] private Button btn_Play;
        [SerializeField] private AudioSpectrum spectrum;

        [SerializeField] private TextMeshProUGUI text_SongName;
        [SerializeField] private TextMeshProUGUI text_SongArtist;

        [SerializeField] private MainMenuUIStateMachine stateMachine;

        private List<SelectElement> _selectElements;

        private int _currentIndex;

        public int SelectionLength => _selectElements.Count;

        private void Awake()
        {
            var levelDataMap = GameApplication.BuiltInData.levelDataMap;

            _selectElements = new List<SelectElement>(levelDataMap.levelDataList.Length);
            foreach (var levelData in levelDataMap.levelDataList)
            {
                if (levelData.label.StartsWith('#')) continue; // it will be ignored

                _selectElements.Add(new SelectElement
                {
                    Origin = levelData, Composer = levelData.composer, SongName = levelData.songName
                });
            }

            ConfigureButtonEvent();
        }

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            _currentIndex = 0;
            RefreshSongView();
        }

        private void ConfigureButtonEvent()
        {
            btn_Return.onClick.AddListener(() => stateMachine.SwitchState(MainMenuUIStateMachine.State.Menu));
            btn_Next.onClick.AddListener(() =>
            {
                _currentIndex++;
                _currentIndex %= SelectionLength;
                RefreshSongView();
            });
            btn_Prev.onClick.AddListener(() =>
            {
                _currentIndex--;
                _currentIndex = (_currentIndex + SelectionLength) % SelectionLength;
                RefreshSongView();
            });
            btn_Play.onClick.AddListener(() => StartLevel(_selectElements[_currentIndex]));
        }

        private async void StartLevel(SelectElement data)
        {
            await TaskBus.Instance.Run(LoadLevelResources(data));

            var runtimeLevelData = new SingleLevelData(data.Origin);
            MonoGameRouter.Instance.LoadScene(new LevelSceneMeta(runtimeLevelData));
        }

        private async Task LoadLevelResources(SelectElement _)
        {
            // well now it's empty... actually there were many code previously.
            await Task.Delay(1000); // show our cool loading screen // 展示我们牛逼的加载界面
        }

        private void RefreshSongView()
        {
            var curr = _selectElements[_currentIndex];
            text_SongName.text = curr.SongName;
            text_SongArtist.text = curr.Composer;
        }

        private class SelectElement
        {
            public LevelData Origin;
            public string SongName;
            public string Composer;
        }
    }
}
