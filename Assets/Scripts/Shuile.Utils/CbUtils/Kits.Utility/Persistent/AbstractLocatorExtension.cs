using Shuile.Framework;

namespace Shuile.Persistent
{
    public static class AbstractLocatorExtension
    {
        public static Viewer<TData> CreatePersistentDataViewer<TData, TLocator>(this TLocator locator)
            where TData : PersistentData<TData>, new()
            where TLocator : AbstractLocator<TLocator>, new()
        {
            return CreatePersistentDataViewer<TData, IAccessor<TData>, TLocator>(locator);
        }

        public static Viewer<TData> CreatePersistentDataViewer<TData, TAccessor, TLocator>(this TLocator locator)
            where TData : PersistentData<TData>, new()
            where TAccessor : IAccessor<TData>
            where TLocator : AbstractLocator<TLocator>, new()
        {
            return new Viewer<TData>(locator.Get<TData>(), locator.Get<TAccessor>());
        }
    }
}
