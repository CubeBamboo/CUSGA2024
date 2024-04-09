using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile
{
    /// <summary>
    /// [WIP]
    /// </summary>
    public class BaseAudioCtrl<TId> : CSharpHungrySingletons<BaseAudioCtrl<TId>>
    {
        protected Dictionary<TId, AudioClip> mContainer;

        public void Load(TId id, object key)
        {
            Addressables.LoadAssetAsync<AudioClip>(key).Completed += res =>
            {
                mContainer.Add(id, res.Result);
            };
        }

        public void Relese(TId id)
        {
            
        }

        public AudioClip Get(TId id) => mContainer[id];
    }
}
