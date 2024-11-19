using CbUtils.Extension;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile.UI
{
    /// <summary>
    /// a lightweight proxy for mono behaviour, constructor can be used.
    /// </summary>
    public abstract class MonoProxy
    {
        private GameObject _gameObject;

        protected abstract IGameObjectProvider gameObjectProvider { get; }

        // we use multiple ways to load view. 1. load from addressable 2. instantiate from GameObject 3. use existing GameObject
        public virtual GameObject LoadView()
        {
            gameObject = gameObjectProvider.GetGameObject();
            AfterLoadView();
            return gameObject;
        }

        public void UnloadView()
        {
            if (IsLoaded)
            {
                Addressables.ReleaseInstance(gameObject);
                gameObject = null;
            }
        }

        protected virtual void AfterLoadView()
        {
        }

        public GameObject gameObject
        {
            get
            {
                if (!_gameObject)
                {
                    throw new InvalidOperationException($"gameObject is not loaded, call {nameof(LoadView)}() first");
                }
                return _gameObject;
            }
            private set => _gameObject = value;
        }

        public Transform transform => gameObject.transform;

        public bool IsLoaded => gameObject;

        public Transform WithChild(Transform child)
        {
            child.SetParent(transform, false);
            return child;
        }

        public Transform[] Children
        {
            set
            {
                foreach (var trans in value)
                {
                    trans.SetParent(transform, false);
                }
            }
        }

        public interface IGameObjectProvider
        {
            GameObject GetGameObject();
        }

        public class AddressableGameObjectProvider : IGameObjectProvider
        {
            private readonly string _resourceKey;

            public AddressableGameObjectProvider(string resourceKey)
            {
                _resourceKey = resourceKey;
            }

            public GameObject GetGameObject()
            {
                return Addressables.LoadAssetAsync<GameObject>(_resourceKey).WaitForCompletion().Instantiate();
            }
        }

        public class InstantiateGameObjectProvider : IGameObjectProvider
        {
            private readonly GameObject _prefab;

            public InstantiateGameObjectProvider(GameObject prefab)
            {
                _prefab = prefab;
            }

            public GameObject GetGameObject()
            {
                return _prefab.Instantiate();
            }
        }

        public class ExistingGameObjectProvider : IGameObjectProvider
        {
            private readonly GameObject _existing;

            public ExistingGameObjectProvider(GameObject existing)
            {
                _existing = existing;
            }

            public GameObject GetGameObject()
            {
                return _existing;
            }
        }

        public class FunctionGameObjectProvider : IGameObjectProvider
        {
            private readonly Func<GameObject> _func;

            public FunctionGameObjectProvider(Func<GameObject> func)
            {
                _func = func;
            }

            public GameObject GetGameObject()
            {
                return _func();
            }
        }
    }
}
