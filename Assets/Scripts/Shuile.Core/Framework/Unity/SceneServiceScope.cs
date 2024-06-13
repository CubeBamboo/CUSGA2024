using CbUtils.Extension;
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
    public class SceneServiceScope<TScope> : MonoBehaviour, IRegisterableScope, IGetableScope where TScope : SceneServiceScope<TScope>
    {
        public static TScope Interface { get; private set; }
        private readonly HashSet<Type> _getChain = new();
        public bool EnableGetChainCheck { get; set; } = true;
        
        private readonly ServiceLocator _serviceLocator = new();
        private readonly Dictionary<Type, Func<object>> _entryPointCreators = new();
        private readonly List<object> _entryPoints = new();
        
        private void Awake()
        {
            if (Interface != null)
                throw new InvalidOperationException("MonoSceneServiceLocator is a singleton and can only be initialized once.");
            Interface = this as TScope;
            Configure(Interface);
            
            // resolve entry points
            PreInitializeEntryPoints();
            var trigger = gameObject.AddComponent<EntryPointTrigger>();
            trigger.enabled = false;
            trigger.EntryPoints = _entryPoints;
            trigger.enabled = true;
        }

        private void PreInitializeEntryPoints()
        {
            // init creator
            foreach ((Type key, Func<object> value) in _entryPointCreators)
            {
                _serviceLocator.RegisterCreator(key, () =>
                {
                    var instance = value();
                    _entryPoints.Add(instance);
                    return instance;
                });
            }
            
            // init entry points
            foreach (var pair in _entryPointCreators)
            {
                this.GetImplementation(pair.Key);
            }
        }
        
        public void Register<T>(Func<T> implementation)
        {
            _serviceLocator.RegisterCreator(implementation);
        }
        public void RegisterMonoComponent<T>(T instance) where T : MonoBehaviour
        {
            _serviceLocator.AddServiceDirectly(instance);
        }
        public void RegisterEntryPoint<T>(Func<T> implementation)
        {
            _entryPointCreators.Add(typeof(T), () => implementation());
        }

        public T GetImplementation<T>() => (T)GetImplementation(typeof(T));
        public object GetImplementation(Type type)
        {
            if(!EnableGetChainCheck) return _serviceLocator.GetService(type);
            
            if (!_getChain.Add(type))
            {
                string log = _getChain.IEnumerableToString(x => "-> " + x.FullName, "", " ") + "->" + _getChain.First().FullName;
                _getChain.Clear();
                throw new InvalidOperationException($"Circular dependency detected, you need to refactor your code: {log}");
            }
            var res = _serviceLocator.GetService(type);
            _getChain.Remove(type);
            return res;
        }

        public void ClearExisting() => _serviceLocator.ClearAllServices();
        
        public virtual void Configure(IRegisterableScope scope)
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
