using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Shuile.Framework
{
    /// <summary>
    /// provide the context shared based on the hierarchy of the GameObject. <br/>
    /// if you need to access Unity's event functions, use <see cref="UnityEntryPointScheduler"/>. <br/>
    /// c# version is <see cref="PlainContainer"/>. actually, <see cref="MonoContainer"/> is a wrapper(entry-point) of <see cref="PlainContainer"/>.
    /// </summary>
    public abstract class MonoContainer : MonoBehaviour, IHasContext
    {
        [FormerlySerializedAs("autoAwake")] [SerializeField] private bool initOnAwake = true;
        [SerializeField] private bool useParentTransformContext = true;

        // make monoContainer can be used as a plain container.
        protected readonly MonoPlainContainerAdapter ContainerAdapter;

        public bool IsInit => ContainerAdapter.IsInit;
        public RuntimeContext Context => ContainerAdapter.Context;

        public static RuntimeContext FallbackContext { get; set; }

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

            OnInitContainer();
        }

        protected virtual void OnInitContainer()
        {
            if(useParentTransformContext) InitParentAboveTransform();
            ContainerHelper.InitContainer(ContainerAdapter);
        }

        private void InitParentAboveTransform()
        {
            var parent = FindParent();
            if (parent != null)
            {
                Context.AddParent(parent.Context);
                parent.MakeSureInit();
            }
            else if (FallbackContext != null)
            {
                Context.AddParent(FallbackContext);
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
    }
}
