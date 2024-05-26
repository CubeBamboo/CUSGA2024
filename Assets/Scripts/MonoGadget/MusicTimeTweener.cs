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

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;

        public void OnSelfEnable()
        {
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
