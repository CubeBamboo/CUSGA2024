using CbUtils.Extension;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Core.Framework.Unity
{
    public interface IGetServiceScope
    {
        T Get<T>();
    }
    
    [DefaultExecutionOrder(-2000)]
    public class SceneServiceScope<TScope> : MonoBehaviour, IGetServiceScope where TScope : SceneServiceScope<TScope>
    {
        public static TScope Interface { get; private set; }
        
        private readonly ServiceLocator _serviceLocator = new();
        private readonly List<object> _entryPoints = new();
        
        private void Awake()
        {
            if (Interface != null)
                throw new InvalidOperationException("MonoSceneServiceLocator is a singleton and may only be initialized once.");
            Interface = this as TScope;
            Configure(Interface);
            
            var resolver = gameObject.AddComponent<EntryPointTrigger>();
            resolver.enabled = false;
            resolver.EntryPoints = _entryPoints;
            resolver.enabled = true;
        }
        
        public void Register<T>(Func<T> implementation)
        {
            _serviceLocator.RegisterCreator(implementation);
        }
        public void RegisterMonoComponent<T>(T instance) where T : MonoBehaviour
        {
            _serviceLocator.RegisterCreator(() => instance);
        }
        public void RegisterEntryPoint<T>(Func<T> implementation)
        {
            var instance = implementation();
            _entryPoints.Add(instance);
            _serviceLocator.RegisterCreator(() => instance);
        }

        public T Get<T>() => _serviceLocator.GetService<T>();
        public void ClearExisting<T>() => _serviceLocator.ClearAllServices();
        
        public virtual void Configure(TScope locator)
        {
        }
    }
    
    [DefaultExecutionOrder(-500)]
    internal class EntryPointTrigger : MonoBehaviour
    {
        public List<object> EntryPoints { get; set; } = new();
        
        private readonly List<IInitializeable> _initializeables = new();
        private readonly List<IStartable> _startables = new();
        private readonly List<ITickable> _tickables = new();
        private readonly List<IFixedTickable> _fixedTickables = new();
        private readonly List<ILateTickable> _lateTickables = new();
        private readonly List<IDestroyable> _destroyables = new();

        private void Awake() => InitializeEntryPointType(false);

        private void OnEnable() => InitializeEntryPointType(true);

        private void Start()
        {
            foreach (var startable in _startables)
            {
                startable.Start();
            }
        }

        private void Update()
        {
            for (int i = 0; i < _tickables.Count; i++)
            {
                _tickables[i].Tick();
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < _fixedTickables.Count; i++)
            {
                _fixedTickables[i].FixedTick();
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < _lateTickables.Count; i++)
            {
                _lateTickables[i].LateTick();
            }
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
                if(item is IInitializeable initializeable && (!checkContain || !_initializeables.Contains(initializeable)))
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
