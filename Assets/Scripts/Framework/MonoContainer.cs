using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Framework
{
    /// <summary>
    /// provide the context shared based on the hierarchy of the GameObject. <br/>
    /// if you need to access Unity's event functions, use <see cref="UnityEntryPointScheduler"/>. <br/>
    /// c# version is <see cref="PlainContainer"/>. actually, <see cref="MonoContainer"/> is a wrapper(entry-point) of <see cref="PlainContainer"/>.
    /// </summary>
    public abstract class MonoContainer : MonoBehaviour, IHasContext
    {
        [SerializeField] private bool initOnAwake = true;
        [SerializeField] private bool useParentTransformContext = true;

        [Tooltip("for debug info if such MonoBehaviour's construct need extra dependencies")] [SerializeField]
        private bool markAsNeedExtraContext;

        // make monoContainer can be used as a plain container.
        protected readonly MonoPlainContainerAdapter ContainerAdapter;

        public bool IsInit => ContainerAdapter.IsInit;
        public RuntimeContext Context => ContainerAdapter.Context;

        public static List<RuntimeContext> GlobalExtraParentForTop { get; set; }
        public static List<RuntimeContext> GlobalExtraParents { get; set; }

        protected MonoContainer()
        {
            ContainerAdapter = new MonoPlainContainerAdapter
            {
                OnBuildChildContext = BuildSelfContext, OnLoadFromContext = LoadFromParentContext, Reference = this
            };
        }

        public virtual void Awake()
        {
            if (initOnAwake)
            {
                MakeSureInit();
            }
        }

        private void InitIt()
        {
            if (IsInit) throw new InvalidOperationException("monoContainer already initialized.");
            if (GetComponents<MonoContainer>().Length > 1) throw MultiMonoContainerException();

            if (markAsNeedExtraContext && (GlobalExtraParents == null && GlobalExtraParentForTop == null))
            {
                Debug.LogWarning($"{name} is marked as need extra context, but no extra context found. are you forget to use the existing loader class?");
            }

            OnInitContainer();
        }

        protected virtual void OnInitContainer()
        {
            if (useParentTransformContext) InitParentAboveTransform();
            if (GlobalExtraParents != null)
            {
                foreach (var parent in GlobalExtraParents)
                {
                    Context.AddParent(parent);
                }
            }

            if (GlobalExtraParentForTop != null && IsTopContainer()) // is top
            {
                foreach (var parent in GlobalExtraParentForTop)
                {
                    Context.AddParent(parent);
                }
            }

            ContainerHelper.InitContainer(ContainerAdapter);
        }

        private bool IsTopContainer()
        {
            var tr = transform.parent;
            var containerType = typeof(MonoContainer);
            while (tr)
            {
                if (tr.TryGetComponent(containerType, out _)) return false;
                tr = tr.parent;
            }

            return true;
        }

        private void InitParentAboveTransform()
        {
            var parent = FindParent();
            if (parent != null)
            {
                Context.AddParent(parent.Context);
                parent.MakeSureInit();
            }

            return;

            MonoContainer FindParent()
            {
                var transformParent = transform.parent;
                while (transformParent != null)
                {
                    var containers = transformParent.GetComponents<MonoContainer>();
                    switch (containers.Length)
                    {
                        case 0:
                            transformParent = transformParent.parent;
                            break;
                        case 1:
                            return containers[0];
                        default:
                            throw MultiMonoContainerException();
                    }
                }

                return null;
            }
        }

        private void InitChildrenTree(Transform current)
        {
            // till the end
            if (current == null) return;

            for(var i = 0; i < current.childCount; i++)
            {
                var child = current.GetChild(i);

                if (child.TryGetComponent<MonoContainer>(out var container))
                {
                    ContainerAdapter.SetChildContainer(container.ContainerAdapter);
                    container.MakeSureInit(); // remains will be initialized by this child.
                }
                else
                {
                    // deeper search
                    InitChildrenTree(child);
                }
            }
        }

        public virtual void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
        }

        public virtual void BuildSelfContext(RuntimeContext context)
        {
        }

        public void MakeSureInit()
        {
            if (IsInit) return;
            InitIt(); // isInit will be set to true inside.
        }

        private static InvalidOperationException MultiMonoContainerException() => new($"cannot have more than one {nameof(MonoContainer)} in a GameObject, try use child GameObjects or plain c# classes.");

        /// <summary>
        ///   will be injected into top MonoContainer which created during ManagedExtraTopParent's life cycle. top means the MonoContainer has no parent in scene.
        /// </summary>
        public static IDisposable EnqueueParentForTop(RuntimeContext context)
        {
            return new ManagedExtraTopParent(context);
        }

        /// <summary>
        ///   will be injected into MonoContainer which created during ManagedExtraParent's life cycle.
        /// </summary>
        public static IDisposable EnqueueParent(RuntimeContext context)
        {
            return new ManagedExtraParent(context);
        }

        private readonly struct ManagedExtraTopParent : IDisposable
        {
            private readonly RuntimeContext _context;

            public ManagedExtraTopParent(RuntimeContext context)
            {
                _context = context;
                GlobalExtraParentForTop ??= new List<RuntimeContext>();
                GlobalExtraParentForTop.Add(context);
            }

            public void Dispose()
            {
                GlobalExtraParentForTop.Remove(_context);
            }
        }

        private readonly struct ManagedExtraParent : IDisposable
        {
            private readonly RuntimeContext _context;

            public ManagedExtraParent(RuntimeContext context)
            {
                _context = context;
                GlobalExtraParents ??= new List<RuntimeContext>();
                GlobalExtraParents.Add(context);
            }

            public void Dispose()
            {
                GlobalExtraParents.Remove(_context);
            }
        }
    }
}
