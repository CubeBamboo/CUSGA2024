namespace CbUtils
{
    public interface ILinkedValue
    {
        void OnUpdate();
    }

    public abstract class TweeningValue<T> : ILinkedValue
    {
        public T Value { get; set; }

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
