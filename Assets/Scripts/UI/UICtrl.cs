/* [WIP]
 * to control the ui
 * TODO-list: resources aollcate and release
 */

using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Framework
{
    public class UICtrl : CSharpHungrySingletons<UICtrl>
    {
        #region PanelManage

        protected readonly Dictionary<System.Type, IPanel> _panels = new();

        public void Register<T>(T panel) where T : IPanel
        {
            panel.Init();
            _panels.Add(typeof(T), panel);
        }
        public void UnRegister<T>() where T : IPanel
        {
            var ty = typeof(T);
            _panels[ty].DeInit();
            _panels.Remove(ty);
        }

        public void Clear()
            => _panels.Clear();

        public T Get<T>() where T : class, IPanel
        {
            // TODO: if not found create a new one
            return _panels[typeof(T)] as T;
        }
        //public Create<T>() where T : BasePanel

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

    public abstract class BasePanelInUnity : IPanel
    {
        // TODO: maybe add a method to auto load assets with assets path

        /// <summary> automatically call in uiCtrl, to generate dependengcy </summary>
        public void Init()
        {
            BeforeInit();
            Panel.transform.SetParent(Canvas.transform, false);
            Hide();
        }
        protected abstract GameObject Panel { get; }
        protected abstract Canvas Canvas { get; }
        public abstract void BeforeInit();
        public abstract void DeInit();
        public abstract void Show();
        public abstract void Hide();
    }
}
