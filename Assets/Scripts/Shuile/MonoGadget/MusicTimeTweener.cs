using Shuile.Framework;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoBehaviour
    {
        private float _lastManagerTime;
        private MusicRhythmManager _manager;

        public float TweenTime { get; private set; }

        protected void Awake()
        {
            SceneContainer.Instance.Context.Resolve(out _manager);
            TweenTime = _lastManagerTime = _manager.CurrentTime;
        }

        private void Update()
        {
            if (!_manager.IsPlaying || _lastManagerTime != _manager.CurrentTime)
            {
                TweenTime = _lastManagerTime = _manager.CurrentTime;
            }
            else
            {
                TweenTime += Time.deltaTime;
            }
        }
    }
}
