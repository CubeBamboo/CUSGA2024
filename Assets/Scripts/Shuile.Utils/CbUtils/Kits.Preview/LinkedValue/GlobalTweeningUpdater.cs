using System.Collections.Generic;

namespace CbUtils
{
    public class GlobalTweeningUpdater : MonoSingletons<GlobalTweeningUpdater>
    {
        private readonly List<ILinkedValue> mTweeningOnUpdate = new();
        private readonly List<ILinkedValue> mTweeningOnFixedUpdate = new();
        private readonly List<ILinkedValue> mTweeningOnLateUpdate = new();

        private void Update()
        {
            for (int i = 0; i < mTweeningOnUpdate.Count; i++)
            {
                mTweeningOnUpdate[i].OnUpdate();
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < mTweeningOnFixedUpdate.Count; i++)
            {
                mTweeningOnFixedUpdate[i].OnUpdate();
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < mTweeningOnLateUpdate.Count; i++)
            {
                mTweeningOnLateUpdate[i].OnUpdate();
            }
        }

        public void Add(ILinkedValue tweeningValue, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    mTweeningOnUpdate.Add(tweeningValue);
                    break;
                case UpdateType.FixedUpdate:
                    mTweeningOnFixedUpdate.Add(tweeningValue);
                    break;
                case UpdateType.LateUpdate:
                    mTweeningOnLateUpdate.Add(tweeningValue);
                    break;
            }
        }
        public void Remove(ILinkedValue tweeningValue, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.Update:
                    mTweeningOnUpdate.Remove(tweeningValue);
                    break;
                case UpdateType.FixedUpdate:
                    mTweeningOnFixedUpdate.Remove(tweeningValue);
                    break;
                case UpdateType.LateUpdate:
                    mTweeningOnLateUpdate.Remove(tweeningValue);
                    break;
            }
        }
    }
}
