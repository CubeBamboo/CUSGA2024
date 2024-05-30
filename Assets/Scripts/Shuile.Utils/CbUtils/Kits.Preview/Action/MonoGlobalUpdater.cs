using CbUtils.Unity;
using System.Collections.Generic;
using System.Linq;
using UDebug = UnityEngine.Debug;

namespace CbUtils
{
    public enum UpdateType
    {
        Update,
        LateUpdate,
        FixedUpdate,
    }

    public class MonoGlobalUpdater : MonoSingletons<MonoGlobalUpdater>
    {
        private readonly Dictionary<string, System.Action> mOnUpdate = new();
        private readonly Dictionary<string, System.Action> mOnFixedUpdate = new();
        private readonly Dictionary<string, System.Action> mOnLateUpdate = new();

        private readonly List<System.Action> cachedUpdateList = new();
        private readonly List<System.Action> cachedFixedUpdateList = new();
        private readonly List<System.Action> cachedLateUpdateList = new();

        private void RefreshCacheList()
        {
            cachedUpdateList.Clear();
            cachedFixedUpdateList.Clear();
            cachedLateUpdateList.Clear();
            mOnUpdate.ToList().ForEach(x => cachedUpdateList.Add(x.Value));
            mOnFixedUpdate.ToList().ForEach(x => cachedFixedUpdateList.Add(x.Value));
            mOnLateUpdate.ToList().ForEach(x => cachedLateUpdateList.Add(x.Value));
        }

        private void Update()
        {
            for (int i = 0; i < cachedUpdateList.Count; i++)
            {
                cachedUpdateList[i].Invoke();
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < cachedFixedUpdateList.Count; i++)
            {
                cachedFixedUpdateList[i].Invoke();
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < cachedLateUpdateList.Count; i++)
            {
                cachedFixedUpdateList[i].Invoke();
            }
        }

        public void Add(string name, System.Action action, UpdateType updateType)
        {
            if (action == null) return;
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
            bool ret1 = mOnUpdate.Remove(name);
            bool ret2 = mOnFixedUpdate.Remove(name);
            bool ret3 = mOnLateUpdate.Remove(name);
            RefreshCacheList();
            return ret1 || ret2 || ret3;
        }
    }
}
