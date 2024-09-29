using CbUtils.Extension;
using Shuile.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shuile.Core.Framework.Unity
{
    public interface IGetableScope
    {
        T GetImplementation<T>();
        object GetImplementation(Type type);
    }

    public interface IRegisterableScope
    {
        void Register<T>(Func<T> implementation);
        void RegisterMonoComponent<T>(T instance) where T : MonoBehaviour;
        void RegisterEntryPoint<T>(Func<T> implementation);
        public void ClearExisting();
    }

    [DefaultExecutionOrder(-2000)]
    public class SceneServiceScope<TScope> : MonoBehaviour, IRegisterableScope, IGetableScope
        where TScope : SceneServiceScope<TScope>
    {
        private readonly Dictionary<Type, Func<object>> _entryPointCreators = new();
        private readonly List<object> _entryPoints = new();
        private readonly HashSet<Type> _getChain = new();

        private readonly ServiceLocator _serviceLocator = new();
        public static TScope Interface { get; private set; }
        public bool EnableGetChainCheck { get; set; } = true;

        private void Awake()
        {
            if (Interface != null)
            {
                throw new InvalidOperationException(
                    "MonoSceneServiceLocator is a singleton and can only be initialized once.");
            }

            Interface = this as TScope;
            Configure(Interface);

            // resolve entry points
            PreInitializeEntryPoints();
            var trigger = gameObject.AddComponent<EntryPointTrigger>();
            trigger.enabled = false;
            trigger.EntryPoints = _entryPoints;
            trigger.enabled = true;
        }

        public T GetImplementation<T>()
        {
            return (T)GetImplementation(typeof(T));
        }

        public object GetImplementation(Type type)
        {
            if (!EnableGetChainCheck)
            {
                return _serviceLocator.Get(type);
            }

            if (!_getChain.Add(type))
            {
                var log = _getChain.IEnumerableToString(x => "-> " + x.FullName, "", " ") + "->" +
                          _getChain.First().FullName;
                _getChain.Clear();
                throw new InvalidOperationException(
                    $"Circular dependency detected, you need to refactor your code: {log}");
            }

            var res = _serviceLocator.Get(type);
            _getChain.Remove(type);
            return res;
        }

        public void Register<T>(Func<T> implementation)
        {
            _serviceLocator.RegisterFactory(implementation);
        }

        public void RegisterMonoComponent<T>(T instance) where T : MonoBehaviour
        {
            _serviceLocator.RegisterInstance(instance);
        }

        public void RegisterEntryPoint<T>(Func<T> implementation)
        {
            _entryPointCreators.Add(typeof(T), () => implementation());
        }

        public void ClearExisting()
        {
            _serviceLocator.ClearAllInstance();
        }

        private void PreInitializeEntryPoints()
        {
            // init creator
            foreach ((var key, var value) in _entryPointCreators)
            {
                _serviceLocator.RegisterFactory(key, () =>
                {
                    var instance = value();
                    _entryPoints.Add(instance);
                    return instance;
                });
            }

            // init entry points
            foreach (var pair in _entryPointCreators)
            {
                GetImplementation(pair.Key);
            }
        }

        public virtual void Configure(IRegisterableScope scope)
        {
        }
    }

    [DefaultExecutionOrder(-500)]
    internal class EntryPointTrigger : MonoBehaviour
    {
        private readonly List<IDestroyable> _destroyables = new();
        private readonly List<IFixedTickable> _fixedTickables = new();

        private readonly List<IInitializeable> _initializeables = new();
        private readonly List<ILateTickable> _lateTickables = new();
        private readonly List<IStartable> _startables = new();
        private readonly List<ITickable> _tickables = new();
        public List<object> EntryPoints { get; set; } = new();

        private void Awake()
        {
            InitializeEntryPointType(false);
        }

        private void Start()
        {
            foreach (var startable in _startables)
            {
                startable.Start();
            }
        }

        private void Update()
        {
            for (var i = 0; i < _tickables.Count; i++)
            {
                _tickables[i].Tick();
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _fixedTickables.Count; i++)
            {
                _fixedTickables[i].FixedTick();
            }
        }

        private void LateUpdate()
        {
            for (var i = 0; i < _lateTickables.Count; i++)
            {
                _lateTickables[i].LateTick();
            }
        }

        private void OnEnable()
        {
            InitializeEntryPointType();
        }

        private void OnDestroy()
        {
            foreach (var destroyable in _destroyables)
            {
                destroyable.OnDestroy();
            }
        }

        private void InitializeEntryPointType(bool checkContain = true)
        {
            foreach (var item in EntryPoints)
            {
                // can't use switch case here because here are all if conditions
                if (item is IInitializeable initializeable && (!checkContain || !_initializeables.Contains(initializeable)))
                {
                    initializeable.Initialize(); // directly call
                    _initializeables.Add(initializeable);
                }

                if (item is IStartable startable && (!checkContain || !_startables.Contains(startable)))
                {
                    _startables.Add(startable);
                }

                if (item is ITickable tickable && (!checkContain || !_tickables.Contains(tickable)))
                {
                    _tickables.Add(tickable);
                }

                if (item is IFixedTickable fixedTickable && (!checkContain || !_fixedTickables.Contains(fixedTickable)))
                {
                    _fixedTickables.Add(fixedTickable);
                }

                if (item is ILateTickable lateTickable && (!checkContain || !_lateTickables.Contains(lateTickable)))
                {
                    _lateTickables.Add(lateTickable);
                }

                if (item is IDestroyable destroyable && (!checkContain || !_destroyables.Contains(destroyable)))
                {
                    _destroyables.Add(destroyable);
                }
            }
        }
    }

    public interface IInitializeable
    {
        void Initialize();
    }

    public interface IStartable
    {
        void Start();
    }

    public interface ITickable
    {
        void Tick();
    }

    public interface IFixedTickable
    {
        void FixedTick();
    }

    public interface ILateTickable
    {
        void LateTick();
    }

    public interface IDestroyable
    {
        void OnDestroy();
    }
}
