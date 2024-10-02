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

        private readonly List<Action> onDrawGizmosSelectedTasks = new();
        private readonly List<Action> onGUITasks = new();

        public static UnityEntryPointScheduler Create(GameObject linkedObject)
        {
            var scheduler = linkedObject.AddComponent<UnityEntryPointScheduler>();
            scheduler.linkedObject = linkedObject;
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
