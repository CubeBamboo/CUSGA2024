using CbUtils.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CbUtils
{
    public enum UpdateType
    {
        Update,
        LateUpdate,
        FixedUpdate
    }

    public class MonoGlobalUpdater : MonoSingletons<MonoGlobalUpdater>
    {
        private readonly List<Action> cachedFixedUpdateList = new();
        private readonly List<Action> cachedLateUpdateList = new();

        private readonly List<Action> cachedUpdateList = new();
        private readonly Dictionary<string, Action> mOnFixedUpdate = new();
        private readonly Dictionary<string, Action> mOnLateUpdate = new();
        private readonly Dictionary<string, Action> mOnUpdate = new();

        private void Update()
        {
            for (var i = 0; i < cachedUpdateList.Count; i++)
            {
                cachedUpdateList[i].Invoke();
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < cachedFixedUpdateList.Count; i++)
            {
                cachedFixedUpdateList[i].Invoke();
            }
        }

        private void LateUpdate()
        {
            for (var i = 0; i < cachedLateUpdateList.Count; i++)
            {
                cachedFixedUpdateList[i].Invoke();
            }
        }

        private void RefreshCacheList()
        {
            cachedUpdateList.Clear();
            cachedFixedUpdateList.Clear();
            cachedLateUpdateList.Clear();
            mOnUpdate.ToList().ForEach(x => cachedUpdateList.Add(x.Value));
            mOnFixedUpdate.ToList().ForEach(x => cachedFixedUpdateList.Add(x.Value));
            mOnLateUpdate.ToList().ForEach(x => cachedLateUpdateList.Add(x.Value));
        }

        public void Add(string name, Action action, UpdateType updateType)
        {
            if (action == null)
            {
                return;
            }
            //UDebug.Log($"MonoGlobalUpdater.Add: name = {name}, updateType = {updateType}");

            switch (updateType)
            {
                case UpdateType.Update:
                    mOnUpdate.Add(name, action);
                    break;
                case UpdateType.FixedUpdate:
                    mOnFixedUpdate.Add(name, action);
                    break;
                case UpdateType.LateUpdate:
                    mOnLateUpdate.Add(name, action);
                    break;
            }

            RefreshCacheList();
        }

        public void Remove(string name, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    mOnUpdate.Remove(name);
                    break;
                case UpdateType.FixedUpdate:
                    mOnFixedUpdate.Remove(name);
                    break;
                case UpdateType.LateUpdate:
                    mOnLateUpdate.Remove(name);
                    break;
            }

            RefreshCacheList();
        }

        public bool CertainlyRemove(string name)
        {
            var ret1 = mOnUpdate.Remove(name);
            var ret2 = mOnFixedUpdate.Remove(name);
            var ret3 = mOnLateUpdate.Remove(name);
            RefreshCacheList();
            return ret1 || ret2 || ret3;
        }
    }
}
