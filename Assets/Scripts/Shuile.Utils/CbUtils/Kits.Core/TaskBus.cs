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

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                if (isBusy == value)
                    return;
                isBusy = value;
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

        public UniTask Execute(UniTask unitask)
        {
            IsBusy = true;
            taskCount++;
            if (unitask.Status.IsCompleted())
                HandleTaskComplete();
            else
                unitask.ContinueWith(() => HandleTaskComplete());
                //unitask.GetAwaiter().OnCompleted(() => HandleTaskComplete(unitask));
            return unitask;
        }

        public UniTask<T> Execute<T>(UniTask<T> unitask)
        {
            IsBusy = true;
            taskCount++;
            if (unitask.Status.IsCompleted())
                HandleTaskComplete();
            else
                unitask.ContinueWith(v => HandleTaskComplete());
                //unitask.GetAwaiter().OnCompleted(() => HandleTaskComplete(unitask));
            return unitask;
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
