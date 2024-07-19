using Shuile.Gameplay;
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
            var scope = LevelScope.Interface;
            _manager = scope.GetImplementation<MusicRhythmManager>();
            ;
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
