using CbUtils;

namespace Shuile.Framework
{
    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    /// a command canbe add listener before and after execute
    /// </summary>
    public abstract class HearableCommand : ICommand
    {
        System.Action _beforeCommand, _afterCommand;
        public void Execute()
        {
            _beforeCommand?.Invoke();
            OnExecute();
            _afterCommand?.Invoke();
        }

        public void OnCommandBefore(System.Action action)
            => _beforeCommand += action;
        public void OnCommandAfter(System.Action action)
            => _afterCommand += action;

        public abstract void OnExecute();
    }

    public abstract class EasyHearableCommand
    {
        readonly EasyEvent _beforeCommand = new(), _afterCommand = new();
        public void Execute()
        {
            _beforeCommand?.Invoke();
            OnExecute();
            _afterCommand?.Invoke();
        }
        
        public abstract void OnExecute();
        public ICustomUnRegister OnCommandBefore(System.Action action)
            => _beforeCommand.Register(action);
        public ICustomUnRegister OnCommandAfter(System.Action action)
            => _afterCommand.Register(action);
    }


    public abstract class BaseCommand<TData> : ICommand where TData : struct
    {
        public TData state { get; protected set; }

        readonly EasyEvent _beforeCommand = new(), _afterCommand = new();
        
        public void Execute()
        {
            _beforeCommand?.Invoke();
            OnExecute();
            _afterCommand?.Invoke();
        }

        public BaseCommand<TData> Bind(TData data)
        {
            this.state = data;
            return this;
        }

        public abstract void OnExecute();
        public ICustomUnRegister OnCommandBefore(System.Action action)
            => _beforeCommand.Register(action);
        public ICustomUnRegister OnCommandAfter(System.Action action)
            => _afterCommand.Register(action);
    }

    // TODO: interceptable command (provide a async call before execute)
}
