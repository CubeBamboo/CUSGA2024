using CbUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Audio
{
    public class SimpleAudioPlayer : AudioPlayerInUnity
    {
        public override AudioSource TargetSource => AudioManager.Instance.OtherSource;

        public new void Reset()
        {
            TargetSource.Stop();
            TargetSource.clip = null;
            TargetSource.volume = 1f;
        }
    }
}
