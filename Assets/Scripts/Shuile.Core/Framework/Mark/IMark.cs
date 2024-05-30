namespace Shuile.Core.Framework
{
    #region LayerControl

    public interface IBelongsToLayerControl
    {
        LayerableServiceLocator GetLocator(); // shortcut for ServiceLcator's get
    }

    public interface ICanUseEvent
    {
    }

    // basically store data
    public interface IModel : IBelongsToLayerControl, ICanUseEvent
    {
    }

    // oprate data, and not response event for better, dont refernce IEntity (like singleton)
    public interface ISystem : IBelongsToLayerControl, ICanUseEvent
    {
    }

    // designed for game object, esspesically those who like update you update me update everything and i dont know what and why they update... the "Update()" function is hard to control.
    public interface IEntity : IBelongsToLayerControl, ICanUseEvent
    {
        bool SelfEnable { get; set; }
        void OnInitData(object data);
    }

    // exist as component of IEntity
    public interface IView : IEntity
    {
    }

    #endregion

    public static class IMarkExtension // a better way for ServiceLocator
    {
        #region LayerControl

        public static T GetModel<T>(this IModel model) where T : IModel
        {
            return model.GetLocator().GetModel<T>();
        }
        public static T GetModel<T>(this ISystem system) where T : IModel
        {
            return system.GetLocator().GetModel<T>();
        }
        public static T GetSystem<T>(this ISystem system) where T : ISystem
        {
            return system.GetLocator().GetSystem<T>();
        }
        public static T GetModel<T>(this IEntity entity) where T : IModel
        {
            return entity.GetLocator().GetModel<T>();
        }
        public static T GetSystem<T>(this IEntity entity) where T : ISystem
        {
            return entity.GetLocator().GetSystem<T>();
        }

        #endregion

        //public static void SendGlobalEvent<T>(this ICanUseEvent self,T para = default)
        //{
        //    TypeEventSystem.Instance.Trigger<T>(para);
        //}
        //public static void RegisterGlobalEvent<T>(this ICanUseEvent self, System.Action<T> action = default)
        //{
        //    TypeEventSystem.Instance.Register<T>(action);
        //}

        /*public static bool TryResolveView<TEntity>(this IEntity entity) where TEntity : IEntity
            => entity.GetLocator().TryResolveView<TEntity>(entity);*/
    }
}
