using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoBehaviour
    {
        private MusicRhythmManager _manager;
        private float _lastManagerTime;
        private float _tweenTime;

        public float TweenTime => _tweenTime;

        protected void Awake()
        {
            var scope = LevelScope.Interface;
            _manager = scope.GetImplementation<MusicRhythmManager>();;
            _tweenTime = _lastManagerTime = _manager.CurrentTime;
        }

        private void Update()
        {
            if (!_manager.IsPlaying || _lastManagerTime != _manager.CurrentTime)
                _tweenTime = _lastManagerTime = _manager.CurrentTime;
            else
                _tweenTime += Time.deltaTime;
        }
    }
}
