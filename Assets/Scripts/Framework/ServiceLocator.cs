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
        public FormattedRegistry CurrentRegistry { get; }
    }

    public class ServiceLocator : IReadOnlyServiceLocator
    {
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
                throw new InvalidOperationException($"Parent already exists: {parent.CurrentRegistry}");
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

        private void RegisterInstanceInternal(Type type, object service)
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
            if (_services.TryGetValue(type, out var obj))
            {
                return obj;
            }

            if (_serviceFactory.TryGetValue(type, out var cre))
            {
                var service = cre();
                RegisterInstanceInternal(type, service);
                return service;
            }

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
                var container = new FormattedRegistry { Types = _serviceFactory.Keys.Union(_services.Keys) };

                if (parents != null)
                {
                    container.Parents = parents.Select(locator => locator.CurrentRegistry);
                }

                return container;
            }
        }

        public static ServiceLocator Global { get; set; }
    }

    public record FormattedRegistry
    {
        public IEnumerable<Type> Types { get; set; }
        public IEnumerable<FormattedRegistry> Parents { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            PrintCurrent(this, 0);

            return builder.ToString();

            void PrintCurrent(FormattedRegistry registry, int depth)
            {
                builder.AppendLine($"parents {depth}");
                var indent = new string(' ', depth * 2);

                var hasType = false;
                foreach (var type in registry.Types)
                {
                    builder.AppendLine($"{indent}{type.Name}");
                    hasType = true;
                }

                if (!hasType)
                {
                    builder.AppendLine($"{indent}empty");
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
