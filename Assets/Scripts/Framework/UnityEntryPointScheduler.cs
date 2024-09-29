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
        private GameObject linkedObject;
        private readonly Queue<Action> taskQueue = new();
        private readonly List<Action> updateTasks = new();
        private readonly List<Action> fixedUpdateTasks = new();
        private readonly List<Action> lateUpdateTasks = new();
        private readonly List<Action> onDestroyTasks = new();

        public static UnityEntryPointScheduler Create(GameObject linkedObject)
        {
            var scheduler = linkedObject.AddComponent<UnityEntryPointScheduler>();
            scheduler.linkedObject = linkedObject;
            return scheduler;
        }

        private void Update()
        {
            while(taskQueue.Count > 0)
            {
                SafeInvoke(taskQueue.Dequeue());
            }

            for (var i = 0; i < updateTasks.Count; i++)
            {
                var task = updateTasks[i];
                SafeInvoke(task);
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < fixedUpdateTasks.Count; i++)
            {
                var task = fixedUpdateTasks[i];
                SafeInvoke(task);
            }
        }

        private void LateUpdate()
        {
            for (var i = 0; i < lateUpdateTasks.Count; i++)
            {
                var task = lateUpdateTasks[i];
                SafeInvoke(task);
            }
        }

        private void OnDestroy()
        {
            for (var i = 0; i < onDestroyTasks.Count; i++)
            {
                var task = onDestroyTasks[i];
                SafeInvoke(task);
            }
        }

        /// <summary>
        /// will be executed in the next frame. can be used as Start() if called during MonoBehaviour.Awake()
        /// </summary>
        /// <param name="action"></param>
        public void AddOnce(Action action)
        {
            taskQueue.Enqueue(action);
        }

        public void AddUpdate(Action action)
        {
            updateTasks.Add(action);
        }

        public void AddFixedUpdate(Action action)
        {
            fixedUpdateTasks.Add(action);
        }

        public void AddLateUpdate(Action action)
        {
            lateUpdateTasks.Add(action);
        }

        public void AddCallOnDestroy(Action action)
        {
            onDestroyTasks.Add(action);
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
    }
}
