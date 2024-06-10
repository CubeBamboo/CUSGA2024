/*using CbUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Framework
{
    [System.Obsolete("use LevelScope")]
    public class UICtrl : CSharpHungrySingletons<UICtrl>
    {
        #region PanelManage

        protected readonly Dictionary<System.Type, PanelCreator> _panelCreator = new();
        protected readonly Dictionary<System.Type, List<IPanel>> _multiPanels = new();

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
        public void RegisterCreator<T>(PanelCreator creator) where T : IPanel, new()
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

    [System.Obsolete("use LevelScope")]
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

    [System.Obsolete("use LevelScope")]
    public abstract class BasePanelWithMono : MonoBehaviour, IPanel
    {
        public virtual void Init() { }
        public virtual void DeInit() { }
        public abstract void Hide();
        public abstract void Show();
    }

    public delegate IPanel PanelCreator(); // Func<IPanel>

    public static class IPanelExt
    {
        public static void SetParent(this BasePanelWithMono panel, Transform canvas)
        {
            panel.transform.SetParent(canvas, false);
        }
    }
}
*/