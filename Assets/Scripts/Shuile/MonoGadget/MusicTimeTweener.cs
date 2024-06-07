using Shuile.Core.Framework;
using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile.MonoGadget
{
    public class MusicTimeTweener : MonoBehaviour, IEntity
    {
        private MusicRhythmManager _manager;
        private float _lastManagerTime;
        private float _tweenTime;

        public float TweenTime => _tweenTime;

        protected void Awake()
        {
            _manager = MusicRhythmManager.Instance;
            _tweenTime = _lastManagerTime = _manager.CurrentTime;
        }

        private void Update()
        {
            if (!_manager.IsPlaying || _lastManagerTime != _manager.CurrentTime)
                _tweenTime = _lastManagerTime = _manager.CurrentTime;
            else
                _tweenTime += Time.deltaTime;
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
