using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Framework
{
    /// <summary>
    /// to access the Unity's entry point from the outside of the MonoBehaviour.
    /// </summary>
    public class UnityEntryPointScheduler : MonoBehaviour
    {
        private readonly Queue<Action> taskQueue = new();
        private readonly List<SchedulerTask> updateTasks = new();
        private readonly List<SchedulerTask> fixedUpdateTasks = new();
        private readonly List<SchedulerTask> lateUpdateTasks = new();
        private readonly List<Action> onDestroyTasks = new();

        private readonly List<Action> onDrawGizmosSelectedTasks = new();
        private readonly List<Action> onGUITasks = new();

        public GameObject LinkedObject { get; private set; }

        public static UnityEntryPointScheduler Create(GameObject linkedObject)
        {
            var scheduler = linkedObject.AddComponent<UnityEntryPointScheduler>();
            scheduler.LinkedObject = linkedObject;
            return scheduler;
        }

        #region UnityEntryPoint

        private void OnDrawGizmosSelected()
        {
            InvokeList(onDrawGizmosSelectedTasks);
        }

        private void OnGUI()
        {
            InvokeList(onGUITasks);
        }

        private void Update()
        {
            while(taskQueue.Count > 0)
            {
                SafeInvoke(taskQueue.Dequeue());
            }

            InvokeList(updateTasks);
        }

        private void FixedUpdate()
        {
            InvokeList(fixedUpdateTasks);
        }

        private void LateUpdate()
        {
            InvokeList(lateUpdateTasks);
        }

        private void OnDestroy()
        {
            InvokeList(onDestroyTasks);
        }

        #endregion

        /// <summary>
        /// will be executed in the next frame. can be used as Start() if called during MonoBehaviour.Awake()
        /// </summary>
        public void AddOnce(Action action)
        {
            taskQueue.Enqueue(action);
        }

        public SchedulerTask AddDelayed(Action action, float delay)
        {
            throw new NotImplementedException();
        }

        public SchedulerTask AddUpdate(Action action)
        {
            var schedulerTask = new SchedulerTask(action, this);
            updateTasks.Add(schedulerTask);
            return schedulerTask;
        }

        public SchedulerTask AddFixedUpdate(Action action)
        {
            var schedulerTask = new SchedulerTask(action, this);
            fixedUpdateTasks.Add(schedulerTask);
            return schedulerTask;
        }

        public SchedulerTask AddLateUpdate(Action action)
        {
            var schedulerTask = new SchedulerTask(action, this);
            lateUpdateTasks.Add(schedulerTask);
            return schedulerTask;
        }

        public void AddCallOnDestroy(Action action)
        {
            onDestroyTasks.Add(action);
        }

        public void AddOnDrawGizmosSelected(Action action)
        {
            onDrawGizmosSelectedTasks.Add(action);
        }

        public void AddOnGUI(Action action)
        {
            onGUITasks.Add(action);
        }

        private static void InvokeList(List<Action> tasks)
        {
            for (var i = 0; i < tasks.Count; i++)
            {
                SafeInvoke(tasks[i]);
            }
        }

        /// <summary>
        /// contains disable logic
        /// </summary>
        /// <param name="tasks"></param>
        private static void InvokeList(List<SchedulerTask> tasks)
        {
            for (var i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if(!task.IsEnabled) continue;
                SafeInvoke(task.OriginAction);
            }
        }

        private static void SafeInvoke(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// it's recommend to use SchedulerTask.Disabled = true instead of this method
        /// </summary>
        /// <param name="task"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveTask(SchedulerTask task)
        {
            for (var i = 0; i < updateTasks.Count; i++)
            {
                if (updateTasks[i] == task)
                {
                    updateTasks.Remove(task);
                    return;
                }
            }

            for (var i = 0; i < fixedUpdateTasks.Count; i++)
            {
                if (fixedUpdateTasks[i] == task)
                {
                    fixedUpdateTasks.Remove(task);
                    return;
                }
            }

            for (var i = 0; i < lateUpdateTasks.Count; i++)
            {
                if (lateUpdateTasks[i] == task)
                {
                    lateUpdateTasks.Remove(task);
                    return;
                }
            }

            throw new Exception("task not found");
        }

        public class SchedulerTask : IDisposable
        {
            private readonly UnityEntryPointScheduler _scheduler;
            public Action OriginAction { get; }
            public bool IsAlive { get; private set; }

            /// <summary>
            /// will not be invoked by scheduler if set to false
            /// </summary>
            public bool IsEnabled { get; set; } = true;

            // action maintained by the scheduler class
            public SchedulerTask(Action originAction, UnityEntryPointScheduler scheduler)
            {
                _scheduler = scheduler;
                OriginAction = originAction;
                IsAlive = true;
            }

            public void Dispose()
            {
                if (!IsAlive) throw new ObjectDisposedException(nameof(SchedulerTask));
                _scheduler.RemoveTask(this);
                IsAlive = false;
            }
        }
    }
}
