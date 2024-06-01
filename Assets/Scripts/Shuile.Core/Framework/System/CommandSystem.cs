namespace Shuile.Core.Framework
{
    public interface ICommand
    {
        void Execute();
    }

    public class CommandSystem
    {
        public void Execute<T>(T command) where T : ICommand
        {
            command.Execute();
        }
    }
}
