/* [WIP]
 * UICtrl: to provide global ui container to access the certain ui object
 * IPanel: base interface, and provide methods to register ui and ui operation
 * BasePanelWithMono: use monobehaviour
 */

using CbUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Framework
{
    public class UICtrl : CSharpHungrySingletons<UICtrl>
    {
        #region PanelManage

        //protected readonly Dictionary<System.Type, IPanel> _singlePanels = new();

        protected readonly Dictionary<System.Type, PanelCreateor> _panelCreator = new();
        protected readonly Dictionary<System.Type, List<IPanel>> _multiPanels = new();

        /* 1. register from MonoBehaviour.Awake()
         * 2. register from Script
         */

        /// <summary> register a existing object </summary>
        /// <param name="index"> specific an index. add to back if index less than 0 </param>
        public int Register<T>(T panel, int index = -1) where T : IPanel
        {
            var ty = typeof(T);
            panel.Init();
            if (!_multiPanels.ContainsKey(ty))
                _multiPanels.Add(ty, new List<IPanel>());

            if (index < 0) _multiPanels[ty].Add(panel);
            else _multiPanels[ty].Insert(index, panel);
            int retIndex = index < 0 ? _multiPanels[ty].Count - 1 : index;
            return retIndex;
        }

        /// <summary> register a function to create the panel </summary>
        public void RegisterCreator<T>(PanelCreateor creator) where T : IPanel, new()
        {
            _panelCreator.Add(typeof(T), creator);
        }

        /// <summary> unregister a existing object </summary>
        /// <param name="index"> specific an index. remove last one if index less than 0 </param>
        public void UnRegister<T>(int index = -1) where T : IPanel
        {
            var ty = typeof(T);
            int target = index >= 0 ? index : _multiPanels[ty].Count - 1;
            _multiPanels[ty][target].DeInit();
            _multiPanels[ty].RemoveAt(target);

            if (_multiPanels[ty].Count == 0)
                _multiPanels.Remove(ty);
        }
        public void UnRegister<T>(T panel) where T : IPanel
        {
            var ty = typeof(T);
            panel.DeInit();
            _multiPanels[ty].Remove(panel);
            if (_multiPanels[ty].Count == 0)
                _multiPanels.Remove(ty);
        }

        public void UnRegisterCreator<T>() where T : IPanel
        {
            _panelCreator.Remove(typeof(T));
        }

        public void Clear()
        {
            foreach(var panels in _multiPanels.Values)
                foreach (var panel in panels)
                    panel.DeInit();
            _multiPanels.Clear();
            _panelCreator.Clear();
        }

        public T Get<T>(int index = 0) where T : class, IPanel
        {
            return _multiPanels[typeof(T)][index] as T;
        }

        // not support panel manage
        public T Create<T>() where T : class, IPanel
        {
            if(_panelCreator.TryGetValue(typeof(T), out var creator))
            {
                T res = creator() as T;
                res.Init();
                return res;
            }

            Debug.LogError($"No creator for {typeof(T)}");
            return null;
        }
            

        #endregion

        #region DataReference

        private Canvas _overlayCanvas;
        public Canvas OverlayCanvas
            => _overlayCanvas = _overlayCanvas ? _overlayCanvas : GameObject.Find("OverlayCanvas").GetComponent<Canvas>();

        private Canvas _worldCanvas;
        public Canvas WorldCanvas
            => _worldCanvas = _worldCanvas ? _worldCanvas : GameObject.Find("WorldCanvas").GetComponent<Canvas>();

        #endregion

    }

    public interface IPanel
    {
        void Show();
        void Hide();
        // maybe it should named "OnCreate"
        /// <summary> automatically call in uiCtrl, to generate dependengcy </summary>
        void Init();
        /// <summary> automatically call in uiCtrl </summary>
        void DeInit();
    }

    // TODO: seems not good to use...
    [System.Obsolete]
    public abstract class BasePanelInUnity : IPanel
    {
        // TODO: maybe add a method to auto load assets with assets path

        /// <summary> automatically call in uiCtrl, to generate dependengcy </summary>
        public void Init()
        {
            GenerateGameObject();
            InitProperty();
            Panel.transform.SetParent(Canvas.transform, false);
            Hide();
        }
        private void GenerateGameObject()
        {
            if (PanelAssetsPath == "")
                return;
            
            var panelAssets = Resources.Load<GameObject>(PanelAssetsPath); //TODO: load service
            _genePanel = panelAssets.Instantiate();
        }
        /// <summary>
        /// if you override PanelAssetsPath.Get, you will get the gameObject it generated here.
        /// </summary>
        protected GameObject _genePanel;
        protected virtual string PanelAssetsPath => "";
        protected abstract GameObject Panel { get; }
        protected abstract Canvas Canvas { get; }
        public abstract void InitProperty();
        public abstract void DeInit();
        public abstract void Show();
        public abstract void Hide();
    }

    /// <summary>
    /// derived from monobehaviour, you are recommended to call this.RegisterUI() on Awake(). 
    /// if it need a runtime load, recommeded to implement a "public static PanelCreateor Creator" and register it to UICtrl.creator otherwhere.
    /// </summary>
    public abstract class BasePanelWithMono : MonoBehaviour, IPanel
    {
        public virtual void Init() { }
        public virtual void DeInit() { }
        public abstract void Hide();
        public abstract void Show();
    }

    public delegate IPanel PanelCreateor(); // Func<IPanel>

    public static class IPanelExt
    {
        public static int RegisterUI<T>(this T panel) where T : IPanel
            => UICtrl.Instance.Register<T>(panel);
        public static void UnRegisterUI<T>(this T panel) where T : IPanel
            => UICtrl.Instance.UnRegister<T>(panel);

        public static void SetParent(this BasePanelWithMono panel, Transform canvas)
        {
            panel.transform.SetParent(canvas, false);
        }
    }
}
