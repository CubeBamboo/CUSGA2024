using CbUtils;
using System;

namespace Shuile.Framework
{
    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    ///     a command canbe add listener before and after execute
    /// </summary>
    public abstract class HearableCommand : ICommand
    {
        private Action _beforeCommand, _afterCommand;

        public void Execute()
        {
            _beforeCommand?.Invoke();
            OnExecute();
            _afterCommand?.Invoke();
        }

        public void OnCommandBefore(Action action)
        {
            _beforeCommand += action;
        }

        public void OnCommandAfter(Action action)
        {
            _afterCommand += action;
        }

        public abstract void OnExecute();
    }

    public abstract class EasyHearableCommand
    {
        private readonly EasyEvent _beforeCommand = new(), _afterCommand = new();

        public void Execute()
        {
            _beforeCommand?.Invoke();
            OnExecute();
            _afterCommand?.Invoke();
        }

        public abstract void OnExecute();

        public ICustomUnRegister OnCommandBefore(Action action)
        {
            return _beforeCommand.Register(action);
        }

        public ICustomUnRegister OnCommandAfter(Action action)
        {
            return _afterCommand.Register(action);
        }
    }


    public abstract class BaseCommand<TData> : ICommand where TData : struct
    {
        private readonly EasyEvent _beforeCommand = new(), _afterCommand = new();
        public TData state { get; protected set; }

        public void Execute()
        {
            _beforeCommand?.Invoke();
            OnExecute();
            _afterCommand?.Invoke();
        }

        public BaseCommand<TData> Bind(TData data)
        {
            state = data;
            return this;
        }

        public abstract void OnExecute();

        public ICustomUnRegister RegisterCommandBefore(Action action)
        {
            return _beforeCommand.Register(action);
        }

        public void UnRegisterBefore(Action action)
        {
            _beforeCommand.UnRegister(action);
        }

        public ICustomUnRegister RegisterCommandAfter(Action action)
        {
            return _afterCommand.Register(action);
        }

        public void UnRegisterAfter(Action action)
        {
            _afterCommand.UnRegister(action);
        }
    }

    public class DelegateCommand<TData> : BaseCommand<TData> where TData : struct
    {
        private readonly Action<TData> action;

        public DelegateCommand(Action<TData> action)
        {
            this.action = action;
        }

        public override void OnExecute()
        {
            action.Invoke(state);
        }
    }
    // TODO: interceptable command (provide a async call before execute)
}
