using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shuile.Framework
{
    public interface IReadOnlyServiceLocator
    {
        T Get<T>();
        object Get(Type type);
        bool Contains(Type type);
        public bool ContainsInternalWithoutParent(Type type);
    }

    // maybe diagnostic window or track reference in debug mode is a better choice.
    [Obsolete]
    public interface IDebugServiceLocator
    {
        string KeyName { get; set; }
        FormattedRegistry CurrentRegistry { get; }
        FormattedParent CurrentParentGraph { get; }
    }

    public interface IServiceLocatorRegister
    {
        void RegisterFactory<T>(Func<T> func);
        void RegisterFactory(Type type, Func<object> func);
        void UnRegisterFactory<T>();
        void UnRegisterFactory(Type type);
        void RegisterInstance<T>(T service);
        void RegisterInstance(Type type, object service);
    }

    /// <summary>
    /// service locator (implementation of inversion of control), provide instance and factory registration. contains parent locators.
    /// if you are using <see cref="MonoContainer"/>, use <see cref="RuntimeContext"/> instead of this, it's also a <see cref="IReadOnlyServiceLocator"/> and more maintainable.
    /// </summary>
    public class ServiceLocator : IReadOnlyServiceLocator, IServiceLocatorRegister, IDebugServiceLocator
    {
        /// <summary>
        /// it seems not a good idea...
        /// </summary>
        public string KeyName { get; set; }

        private static readonly Type objectType = typeof(object);
        private readonly Dictionary<Type, Func<object>> _serviceFactory = new();

        private readonly Dictionary<Type, object> _services = new();

        private List<IReadOnlyServiceLocator> parents;
        public IReadOnlyList<IReadOnlyServiceLocator> Parents => parents;

        public ServiceLocator()
        {
        }

        public ServiceLocator(IReadOnlyServiceLocator parent) : this()
        {
            AddParent(parent);
        }

        public void AddParent(IReadOnlyServiceLocator parent)
        {
            if (parent == null)
            {
                throw new InvalidOperationException("Parent cannot be null");
            }
            if (parent == this)
            {
                throw new InvalidOperationException("Parent cannot be itself");
            }
            if (parents != null && parents.Contains(parent))
            {
                throw new InvalidOperationException($"Parent already exists. {(parent as IDebugServiceLocator)?.KeyName}");
            }

            parents ??= new List<IReadOnlyServiceLocator>();
            parents.Add(parent);
        }

        public void RegisterFactory<T>(Func<T> func)
        {
            RegisterFactory(typeof(T), () => func());
        }

        public void RegisterFactory(Type type, Func<object> func)
        {
            if (type == objectType)
            {
                throw new ArgumentException("Registering object type is not allowed.");
            }

            _serviceFactory[type] = func;
        }

        public void UnRegisterFactory<T>()
        {
            UnRegisterFactory(typeof(T));
        }

        public void UnRegisterFactory(Type type)
        {
            _serviceFactory.Remove(type);
        }

        public void RegisterInstance<T>(T service)
        {
            RegisterInstance(typeof(T), service);
        }

        public void RegisterInstance(Type type, object service)
        {
            RegisterInstanceInternal(type, service);
        }

        protected virtual void RegisterInstanceInternal(Type type, object service)
        {
            _services[type] = service;
        }

        [DebuggerHidden]
        public T Get<T>()
        {
            return (T)GetInternal(typeof(T));
        }

        [DebuggerHidden]
        public object Get(Type type)
        {
            return GetInternal(type);
        }

        private object GetInternal(Type type)
        {
            // in cache
            if (_services.TryGetValue(type, out var obj))
            {
                return obj;
            }

            // load from factory
            if (_serviceFactory.TryGetValue(type, out var cre))
            {
                var service = cre();
                RegisterInstanceInternal(type, service);
                return service;
            }

            // get from parent
            if (parents != null)
            {
                foreach (var locator in parents)
                {
                    var get = locator.Get(type);
                    if (get != null)
                    {
                        return get;
                    }
                }
            }

            return null;
        }

        public void ClearAllInstance()
        {
            _services.Clear();
        }

        public void ClearAll()
        {
            _services.Clear();
            _serviceFactory.Clear();
        }

        public bool Contains<T>()
        {
            return ContainsInternalWithoutParent(typeof(T));
        }

        public bool Contains(Type type)
        {
            return ContainsInternalWithoutParent(type);
        }

        public bool ContainsInternalWithoutParent(Type type)
        {
            return _serviceFactory.ContainsKey(type) || _services.ContainsKey(type);
        }

        public FormattedRegistry CurrentRegistry
        {
            get
            {
                var container = new FormattedRegistry { Current = _serviceFactory.Keys.Union(_services.Keys) };

                if (parents != null)
                {
                    container.Parents = parents.Select(locator => (locator as IDebugServiceLocator)?.CurrentRegistry);
                }

                return container;
            }
        }

        public FormattedParent CurrentParentGraph
        {
            get
            {
                var container = new FormattedParent { Current = this };

                if (parents != null)
                {
                    container.Parents = parents.Select(locator => (locator as IDebugServiceLocator)?.CurrentParentGraph);
                }

                return container;
            }
        }

        public static ServiceLocator Global { get; set; } = new();
    }

    // null value will be handled as cannot debug.
    public record FormattedParent
    {
        public IDebugServiceLocator Current { get; set; }
        public IEnumerable<FormattedParent> Parents { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            PrintCurrent(this, 0);

            return builder.ToString();

            void PrintCurrent(FormattedParent registry, int depth)
            {
                builder.Append($"parents {depth}: ");
                if (registry.Current != null)
                {
                    builder.Append(registry.Current.KeyName);
                }
                else
                {
                    builder.Append("empty");
                }

                if (registry.Parents == null)
                {
                    return;
                }

                builder.AppendLine();

                foreach (var parent in registry.Parents)
                {
                    PrintCurrent(parent, depth + 1);
                }
            }
        }
    }

    // null value will be handled as cannot debug.
    public record FormattedRegistry
    {
        public IEnumerable<Type> Current { get; set; }
        public IEnumerable<FormattedRegistry> Parents { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            PrintCurrent(this, 0);

            return builder.ToString();

            void PrintCurrent(FormattedRegistry registry, int depth)
            {
                builder.Append($"parentRegistry {depth}: ");
                if (registry.Current.Any())
                {
                    builder.AppendJoin(", ", registry.Current.Select(x => x?.ToString() ?? "not debuggable"));
                }
                else
                {
                    builder.Append("empty");
                }

                if (registry.Parents == null)
                {
                    return;
                }

                foreach (var parent in registry.Parents)
                {
                    PrintCurrent(parent, depth + 1);
                }
            }
        }
    }
}
