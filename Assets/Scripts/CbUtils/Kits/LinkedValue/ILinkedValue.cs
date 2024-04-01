using UnityEngine;

namespace CbUtils.LinkedValue
{
    public enum UpdateType
    {
        Update,
        LateUpdate,
        FixedUpdate,
    }

    public interface ILinkedValue
    {
        void OnUpdate();
    }

    public abstract class TweeningValue<T> : ILinkedValue
    {
        private T _Value;
        public T Value
        {
            get => _Value;
            set
            {
                _Value = value;
            }
        }
        public T TweenResult { get; set; }

        public virtual void OnUpdate()
        {
            TweenResult = TweenResult;
        }
    }

    public static class TweeningValueExt
    {
        public static void AddToGlobalUpdater<T>(TweeningValue<T> self, UpdateType updateType = UpdateType.Update)
        {
            GlobalTweeningUpdater.Instance.Add(self, updateType);
        }
    }
}
