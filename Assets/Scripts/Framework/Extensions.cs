using System;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shuile.Framework
{
    public static class Extensions
    {
        public static IReadOnlyServiceLocator Resolve<T>(this IReadOnlyServiceLocator serviceLocator, out T dest)
        {
            dest = serviceLocator.Get<T>();
            if (dest == null)
            {
                throw CannotResolveException<T>(serviceLocator);
            }

            return serviceLocator;
        }

        // compatible with old code
        public static T GetImplementation<T>(this IReadOnlyServiceLocator serviceLocator)
        {
            var dest = serviceLocator.Get<T>();
            if (dest == null)
            {
                throw CannotResolveException<T>(serviceLocator);
            }
            return dest;
        }

        private static Exception CannotResolveException<T>(IReadOnlyServiceLocator serviceLocator = null)
        {
            var msg = new StringBuilder();
            msg.AppendLine($"Cannot resolve {typeof(T).Name}");
            return new Exception(msg.ToString());
        }

        public static UnityEntryPointScheduler RegisterMonoScheduler(this ServiceLocator serviceLocator, MonoBehaviour monoBehaviour)
        {
            UnityEntryPointScheduler scheduler;
            serviceLocator.RegisterInstance(scheduler = UnityEntryPointScheduler.Create(monoBehaviour.gameObject));
            return scheduler;
        }

        public static UnityEntryPointScheduler RegisterMonoScheduler(this RuntimeContext context, MonoBehaviour monoBehaviour)
        {
            UnityEntryPointScheduler scheduler;
            context.RegisterInstance(scheduler = UnityEntryPointScheduler.Create(monoBehaviour.gameObject));
            return scheduler;
        }
    }

    public static class ContainerExtensions
    {
        public static void Inject(this RuntimeContext context, PlainContainer plainContainer)
        {
            plainContainer.Context.AddParent(context);
            ContainerHelper.InitContainer(plainContainer);
        }

        public static bool HasParent<T>(this T container) where T : PlainContainer
        {
            return container.Parents is { Count: > 0 };
        }

        public static SceneContainer FindSceneContainer()
        {
            var sceneContainer = SceneContainer.Instance;
            sceneContainer ??= Object.FindObjectOfType<SceneContainer>();
            sceneContainer.MakeSureInit();
            return sceneContainer;
        }

        public static RuntimeContext FindSceneContext() => FindSceneContainer().Context;
    }
}
