using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace CbUtils.Kits.Tasks
{
    public interface IBusyScreen
    {
        public void Show();
        public void Hide();
    }

    public class TaskBus
    {
        private static readonly Lazy<TaskBus> instance = new();

        private IBusyScreen busyScreen;
        private bool isBusy;
        private int taskCount;
        public static TaskBus Instance => instance.Value;

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                if (isBusy == value)
                {
                    return;
                }

                isBusy = value;

                OnTaskComplete?.Invoke();

                if (isBusy && busyScreen != null)
                {
                    busyScreen.Show();
                }
                else
                {
                    busyScreen.Hide();
                }
            }
        }

        // if you can call Execute() to solve, don't use this event.
        public event Action OnTaskComplete;

        public void Initialize(IBusyScreen busyScreen)
        {
            this.busyScreen = busyScreen;
        }

        public void Execute(Task task)
        {
            IsBusy = true;
            taskCount++;
            if (task.IsCompleted)
            {
                HandleTaskComplete();
            }
            else
            {
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(HandleTaskComplete);
            }
        }

        public Task Run(Task task)
        {
            IsBusy = true;
            taskCount++;
            if (task.IsCompleted)
            {
                HandleTaskComplete();
            }
            else
            {
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(HandleTaskComplete);
            }

            return task;
        }

        public Task<T> Run<T>(Task<T> task)
        {
            IsBusy = true;
            taskCount++;
            if (task.IsCompleted)
            {
                HandleTaskComplete();
            }
            else
            {
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(HandleTaskComplete);
            }

            return task;
        }

        public void Execute(UniTask unitask)
        {
            IsBusy = true;
            taskCount++;
            if (unitask.Status.IsCompleted())
            {
                HandleTaskComplete();
            }
            else
            {
                unitask.ContinueWith(HandleTaskComplete);
            }
        }

        public UniTask Run(UniTask unitask)
        {
            IsBusy = true;
            taskCount++;
            if (unitask.Status.IsCompleted())
            {
                HandleTaskComplete();
                return unitask;
            }

            var ret = unitask.ContinueWith(HandleTaskComplete);
            return ret;
        }

        /// <summary> [warning]: With GC </summary>
        public UniTask<T> Run<T>(UniTask<T> unitask)
        {
            IsBusy = true;
            taskCount++;

            var tcs = new UniTaskCompletionSource<T>();

            if (unitask.Status.IsCompleted())
            {
                HandleTaskComplete();
                return unitask;
            }

            unitask.ContinueWith(res =>
            {
                HandleTaskComplete();
                tcs.TrySetResult(res);
            });
            return tcs.Task;
        }

        private void HandleTaskComplete()
        {
            HandleTaskCompleteWithCount();
        }

        private void HandleTaskComplete(Task task)
        {
            HandleTaskCompleteWithCount();
        }

        private void HandleTaskComplete(UniTask unitask)
        {
            HandleTaskCompleteWithCount();
        }

        private void HandleTaskCompleteWithCount()
        {
            taskCount--;
            if (taskCount < 0)
            {
                throw new Exception($"[{nameof(TaskBus)}] Task count can't be negative.");
            }

            if (taskCount == 0)
            {
                IsBusy = false;
            }
        }
    }
}
