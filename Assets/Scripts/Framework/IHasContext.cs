namespace Shuile.Framework
{
    public interface IHasContext
    {
        /// <summary>
        ///     it will be invoked by <see cref="MonoContainer" />.Awake(), the MonoContainer with a parent transform will be added
        ///     to the parent's service locator, which shares the services.
        /// </summary>
        void ResolveContext(IReadOnlyServiceLocator context);

        /// <summary>
        /// used to build the context, will be called by <see cref="MonoContainer"/>.Awake()
        /// </summary>
        void BuildContext(ServiceLocator context);
    }
}
