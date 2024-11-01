using System;

namespace Framework
{
    public readonly struct DeferAction : IDisposable
    {
        private readonly Action _action;

        public DeferAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action.Invoke();
        }
    }
}
