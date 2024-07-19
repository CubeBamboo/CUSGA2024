namespace Shuile.Core.Framework
{
    // TODO: [WIP]
    public class ApplicationServiceScope<TScope>
    {
        private readonly ServiceLocator _serviceLocator = new();
        public static TScope Interface { get; private set; }
    }
}
