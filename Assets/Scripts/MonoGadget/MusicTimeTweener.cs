using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoBehaviour, IEntity
    {
        private MusicRhythmManager manager;
        private float lastManagerTime;
        private float tweenTime;

        public float TweenTime => tweenTime;

        public bool SelfEnable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ModuleContainer GetModule() => GameApplication.Level;

        public void OnInitData(object data)
        {
            throw new System.NotImplementedException();
        }

        protected void Awake()
        {
            manager = this.GetSystem<MusicRhythmManager>();
            tweenTime = lastManagerTime = manager.CurrentTime;
        }

        private void Update()
        {
            if (!manager.IsPlaying || lastManagerTime != manager.CurrentTime)
                tweenTime = lastManagerTime = manager.CurrentTime;
            else
                tweenTime += Time.deltaTime;
        }
    }
}
