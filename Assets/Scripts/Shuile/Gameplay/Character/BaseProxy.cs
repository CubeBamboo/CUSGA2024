using Shuile.Framework;

namespace Shuile.Gameplay.Character
{
    public abstract class BaseProxy
    {
        protected BaseProxy(UnityEntryPointScheduler scheduler, IReadOnlyServiceLocator dependencies)
        {
        }

        /// <summary>
        /// using scheduler to access event, only need exterior to create it and pass the dependencies. <br/>
        /// <see cref="Forget"/> to eliminate the suggestions from the IDE.
        /// </summary>
        public void Forget()
        {
        }
    }
}
