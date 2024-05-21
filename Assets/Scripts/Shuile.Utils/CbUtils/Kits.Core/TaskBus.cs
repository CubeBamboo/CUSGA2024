using Cysharp.Threading.Tasks;
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
        private static readonly System.Lazy<TaskBus> instance = new();
        public static TaskBus Instance => instance.Value;

        private IBusyScreen busyScreen;
        private int taskCount = 0;
        private bool isBusy;

        public event System.Action OnTaskComplete;
        public event System.Action OnTaskComplete_AutoClear;

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                if (isBusy == value)
                    return;
                isBusy = value;

                OnTaskComplete?.Invoke();
                OnTaskComplete_AutoClear?.Invoke();
                OnTaskComplete_AutoClear = null;

                if(isBusy && busyScreen != null)
                    busyScreen.Show();
                else
                    busyScreen.Hide();
            }
        }

        public void Initialize(IBusyScreen busyScreen)
        {
            this.busyScreen = busyScreen;
        }

        public void ExecuteVoid(Task task)
        {
            IsBusy = true;
            taskCount++;
            if (task.IsCompleted)
                HandleTaskComplete();
            else
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(() => HandleTaskComplete());
        }
        public Task Execute(Task task)
        {
            IsBusy = true;
            taskCount++;
            if(task.IsCompleted)
                HandleTaskComplete();
            else
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(() => HandleTaskComplete());
            return task;
        }
        public Task<T> Execute<T>(Task<T> task)
        {
            IsBusy = true;
            taskCount++;
            if (task.IsCompleted)
                HandleTaskComplete();
            else
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(() => HandleTaskComplete());
            return task;
        }

        public void ExecuteVoid(UniTask unitask)
        {
            IsBusy = true;
            taskCount++;
            if (unitask.Status.IsCompleted())
                HandleTaskComplete();
            else
                unitask.ContinueWith(() => HandleTaskComplete());
        }
        public UniTask Execute(UniTask unitask)
        {
            IsBusy = true;
            taskCount++;
            if (unitask.Status.IsCompleted())
            {
                HandleTaskComplete();
                return unitask;
            }
            else
            {
                var ret = unitask.ContinueWith(() => HandleTaskComplete());
                return ret;
            }
        }

        private void HandleTaskComplete() => HandleTaskCompleteWithCount();
        private void HandleTaskComplete(Task task) => HandleTaskCompleteWithCount();
        private void HandleTaskComplete(UniTask unitask) => HandleTaskCompleteWithCount();

        private void HandleTaskCompleteWithCount()
        {
            taskCount--;
            if (taskCount < 0)
                throw new System.Exception($"[{nameof(TaskBus)}] Task count can't be negative.");

            if (taskCount == 0)
            {
                IsBusy = false;
                return;
            }
        }
    }
}
