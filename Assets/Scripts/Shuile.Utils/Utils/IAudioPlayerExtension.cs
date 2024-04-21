using Cysharp.Threading.Tasks;

using Shuile.Audio;

using System.Collections.Generic;

using UnityEngine;

namespace Shuile
{
    public static class IAudioPlayerExtension
    {
        /// <summary>
        /// 等待dsp时间越过waitTo参数提供的时间，返回值为越过的时间(AudioSettings.dspTime - waitTo)
        /// </summary>
        /// <param name="waitTo"></param>
        /// <returns></returns>
        public static async UniTask<double> WaitPlayScheduled(this IAudioPlayer player, double playAt)
        {
            player.PlayScheduled(playAt);
            await UniTask.WaitUntil(() => AudioSettings.dspTime >= playAt);
            return AudioSettings.dspTime - playAt;
        }
    }
}
