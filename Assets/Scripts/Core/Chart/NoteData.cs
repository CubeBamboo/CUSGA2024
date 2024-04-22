using CbUtils;
using Cysharp.Threading.Tasks;

using Shuile.Gameplay;
using Shuile.NoteProduct;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    [System.Obsolete("use Base Note Data instead")]
    public class NoteData
    {
        /// <summary>
        /// integer part - beat count, decimal part - where in a beat. 
        /// for example: targetTimeArray = { 0 + 0f / 4f, 1 + 0f / 4f, 2 + 0f / 4f, 3 + 0f / 4f };
        /// </summary>
        public float targetTime;
        /// <summary>
        /// only for long note, format is same as targetTime
        /// </summary>
        public float? endTime;

        public NoteEventType eventType;
        //public NoteEventData eventData;

        public static NoteData Create(float targetTime)
            => new() { targetTime = targetTime };
    }

    [System.Obsolete("use Base Note Data instead")]
    public enum NoteEventType
    {
        SingleEnemySpawn,
        MultiEnemySpawn, // spawn in a certain frequency
        ObjectTransform,
        LaserSpawn,
        MusicOffsetTestLaser,
    }

    [System.Obsolete("use Base Note Data instead")]
    public static class NoteEventUtils
    {
        public static float DefaultPlayTimeConvert(NoteData noteData)
        {
            float preshowTime = 0f;
            float preshowRealTime = 0f;
            //if (noteData.eventType == NoteEventType.LaserSpawn) preshowTime = 2;
            float res = (noteData.targetTime - preshowTime) * GameplayService.Interface.LevelModel.BpmIntervalInSeconds - preshowRealTime;
            return res;
        }
    }
}
