//using System;
//using System.Collections.Generic;

//namespace Shuile.Core.Framework
//{
//    public enum ModuleLayer
//    {
//        Controller, System, Model, Utility
//    }

//    public class AbstractModule
//    {
//        private readonly ServiceLocator serviceLocator = new();
//        public static Lazy<AbstractModule> Interface { get; } = new(() => new AbstractModule());
//        private readonly Dictionary<Type, ModuleLayer> serviceLayerDic = new();

//        private bool CompareLayer(ModuleLayer sender, ModuleLayer beGetter)
//        {
//            return sender != ModuleLayer.Controller && sender >= beGetter;
//        }

//        public void RegisterCreator<T>(Func<T> creator, ModuleLayer layer) where T : IModulePart
//        {
//            serviceLayerDic[typeof(T)] = layer;
//            serviceLocator.RegisterCreator(creator);
//        }
//        public void UnRegisterCreator<T>()
//        {
//            serviceLayerDic.Remove(typeof(T));
//            serviceLocator.UnRegisterCreator<T>();
//        }

//        public T GetService<T>(IModulePart sender) where T : IModulePart
//        {
//            sender.Layer
//            return serviceLocator.GetService<T>();
//        }

//        public T DirectlyGetService<T>() => serviceLocator.GetService<T>();
//        public void ClearAllServices() => serviceLocator.ClearAllServices();
//        public void ClearAllServicesCreator() => serviceLocator.ClearAllServicesCreator();
//    }

//    public interface IModulePart
//    {
//        public ModuleLayer Layer { get; }
//        public AbstractModule ModuleManager { get; }
//    }

//    public static class IModulePartExtension
//    {
//        public static T GetService<T>(this IModulePart modulePart) => modulePart.ModuleManager.DirectlyGetService<T>();
//    }
//}
