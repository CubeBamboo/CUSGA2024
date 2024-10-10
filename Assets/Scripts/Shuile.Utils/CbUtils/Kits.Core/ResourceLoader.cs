using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile
{
    public class ResourceLoader
    {
        public T Load<T>(string key, LoadSource loadSource = LoadSource.Addressable)
        {
            switch (loadSource)
            {
                case LoadSource.Addressable:
                    return LoadAddressable<T>(key);
                case LoadSource.Resources:
                    return LoadResources<T>(key);
                default:
                    return default;
            }
        }

        private T LoadResources<T>(string key)
        {
            return (T)(object)Resources.Load(key);
        }

        private T LoadAddressable<T>(string key)
        {
            var loadAssetAsync = Addressables.LoadAssetAsync<T>(key);
            var res = loadAssetAsync.WaitForCompletion();
            return res;
        }

        public enum LoadSource
        {
            Addressable,
            Resources,
        }
    }
}
