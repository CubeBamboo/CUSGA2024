namespace Shuile.Core.Framework
{
    public interface IBelongsToLayerControl
    {
        LayerableServiceLocator GetLocator(); // shortcut for ServiceLcator's get
    }

    // basically store data
    public interface IModel : IBelongsToLayerControl
    {
    }

    // oprate data, and not response event for better, dont refernce IEntity (like singleton)
    public interface ISystem : IBelongsToLayerControl
    {
    }

    // designed for game object, esspesically those who like update you update me update everything and i dont know what and why they update... the "Update()" function is hard to control.
    public interface IEntity : IBelongsToLayerControl
    {
        void OnSelfEnable();
    }

    public static class IMarkExtension // a better way for ServiceLocator
    {
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
    }
}
