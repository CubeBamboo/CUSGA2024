namespace Shuile.Core.Framework
{
    #region ModuleContainer

    public class ModuleContainer
    {
        public LayerableServiceLocator ServiceLocator { get; } = new();
        public TypeEventSystem TypeEventSystem { get; } = new();
        public CommandSystem CommandSystem { get; } = new();
    }

    public static class ModuleContainerExtension
    {
        public static ModuleContainer AddUtilityImplemenation<T>(this ModuleContainer module, System.Func<T> implementation) where T : IUtility
        {
            module.ServiceLocator.AddUtilityCreator(implementation);
            return module;
        }
        public static ModuleContainer AddModelImplemenation<T>(this ModuleContainer module, System.Func<T> implementation) where T : IModel
        {
            module.ServiceLocator.AddModelCreator(implementation);
            return module;
        }
        public static ModuleContainer AddSystemImplemenation<T>(this ModuleContainer module, System.Func<T> implementation) where T : ISystem
        {
            module.ServiceLocator.AddSystemCreator(implementation);
            return module;
        }
        public  static T GetUtilityImplemenation<T>(this ModuleContainer module) where T : IUtility
        {
            return module.ServiceLocator.GetUtility<T>();
        }
        public static T GetModelImplemenation<T>(this ModuleContainer module) where T : IModel
        {
            return module.ServiceLocator.GetModel<T>();
        }
        public static T GetSystemImplemenation<T>(this ModuleContainer module) where T : ISystem
        {
            return module.ServiceLocator.GetSystem<T>();
        }

        public static void ExecuteCommand<T>(this ModuleContainer module, T command) where T : ICommand
        {
            module.CommandSystem.Execute(command);
        }
        public static void RegisterEvent<T>(this ModuleContainer module, System.Action<T> action) where T : ITypeEvent
        {
            module.TypeEventSystem.Register(action);
        }
        public static void UnRegisterEvent<T>(this ModuleContainer module, System.Action<T> action) where T : ITypeEvent
        {
            module.TypeEventSystem.UnRegister(action);
        }
        public static void ClearEventOf<T>(this ModuleContainer module) where T : ITypeEvent
        {
            module.TypeEventSystem.ClearEventOf<T>();
        }
        public static void TriggerEvent<T>(this ModuleContainer module, T para) where T : ITypeEvent
        {
            module.TypeEventSystem.Trigger(para);
        }
    }

    #endregion

    #region ModuleControl

    public interface IBelongsToModuleControl
    {
        ModuleContainer GetModule();
    }

    public interface ICanGetUtility : IBelongsToModuleControl
    {
    }
    public interface ICanGetModel : IBelongsToModuleControl
    {
    }
    public interface ICanGetSystem : IBelongsToModuleControl
    {
    }
    public interface ICanRegisterEvent : IBelongsToModuleControl
    {
    }
    public interface ICanTriggerEvent : IBelongsToModuleControl
    {
    }
    public interface ICanExecuteCommand : IBelongsToModuleControl
    {
    }

    public interface IUtility
    {
    }

    // basically store data
    public interface IModel : IBelongsToModuleControl, ICanTriggerEvent, ICanExecuteCommand, ICanGetUtility, ICanGetModel
    {
    }

    // oprate data, and not response event for better, dont refernce IEntity (like singleton)
    public interface ISystem : IBelongsToModuleControl, ICanTriggerEvent, ICanExecuteCommand,
        ICanGetUtility, ICanGetModel, ICanGetSystem
    {
    }

    // designed for game object, esspesically those who like update you update me update everything and i dont know what and why they update... the "Update()" function is hard to control.
    public interface IEntity : IBelongsToModuleControl, ICanRegisterEvent, ICanTriggerEvent, ICanExecuteCommand,
        ICanGetUtility, ICanGetModel, ICanGetSystem
    {
        bool SelfEnable
        {
            get => true;
            set { }
        }
        void OnInitData(object data) { }
    }

    public static class IModuleExtension // a better way for ServiceLocator
    {
        public static T GetUtility<T>(this ICanGetUtility utility) where T : IUtility
        {
            return utility.GetModule().GetUtilityImplemenation<T>();
        }
        public static T GetModel<T>(this ICanGetModel model) where T : IModel
        {
            return model.GetModule().GetModelImplemenation<T>();
        }
        public static T GetSystem<T>(this ICanGetSystem system) where T : ISystem
        {
            return system.GetModule().GetSystemImplemenation<T>();
        }

        public static void RegisterEvent<T>(this ICanRegisterEvent entity, System.Action<T> action) where T : ITypeEvent
        {
            entity.GetModule().RegisterEvent(action);
        }
        public static void UnRegisterEvent<T>(this ICanRegisterEvent entity, System.Action<T> action) where T : ITypeEvent
        {
            entity.GetModule().UnRegisterEvent(action);
        }
        public static void TriggerEvent<T>(this ICanTriggerEvent entity, T para) where T : ITypeEvent
        {
            entity.GetModule().TriggerEvent(para);
        }

        public static void ExecuteCommand<T>(this ICanExecuteCommand entity, T command) where T : ICommand
        {
            entity.GetModule().ExecuteCommand(command);
        }
    }

    #endregion
}