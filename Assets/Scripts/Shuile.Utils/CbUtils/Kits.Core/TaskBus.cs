using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

        private readonly List<Task> _activeTasks = new();

        private IBusyScreen _busyScreen;
        private bool _isBusy;
        public static TaskBus Instance => instance.Value;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy == value)
                {
                    return;
                }

                _isBusy = value;

                OnTaskComplete?.Invoke();

                if (_isBusy && _busyScreen != null)
                {
                    _busyScreen.Show();
                }
                else
                {
                    _busyScreen.Hide();
                }
            }
        }

        // if you can call Execute() to solve, don't use this event.
        public event Action OnTaskComplete;

        public void Initialize(IBusyScreen busyScreen)
        {
            this._busyScreen = busyScreen;
        }

        public void Execute(Task task)
        {
            IsBusy = true;
            _activeTasks.Add(task);
            if (task.IsCompleted)
            {
                HandleTaskComplete(task);
            }
            else
            {
                task.ConfigureAwait(false)
                    .GetAwaiter().OnCompleted(() => HandleTaskComplete(task));
            }
        }

        public Task Run(Task task)
        {
            Execute(task);
            return task;
        }

        public Task<T> Run<T>(Task<T> task)
        {
            Execute(task);
            return task;
        }

        private void HandleTaskComplete(Task task)
        {
            _activeTasks.Remove(task);
            CheckoutBusy();

            if (task.IsFaulted)
            {
                var exception = task.Exception!.InnerException ?? task.Exception;
                Debug.LogException(exception);
            }
        }

        private void CheckoutBusy()
        {
            if (_activeTasks.Count == 0)
            {
                IsBusy = false;
            }
        }
    }
}
