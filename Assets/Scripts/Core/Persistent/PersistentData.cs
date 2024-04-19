namespace Shuile.Persistent
{
    public abstract partial class PersistentData<T> : INotifyTreePropertyChanged where T : PersistentData<T>, new()
    {
        public static T Default => new T();
    }

    public abstract partial class PersistentData<T> : INotifyTreePropertyChanged where T : PersistentData<T>, new()
    {
        public event TreePropertyChangedEventHandler OnTreePropertyChanged;

        public void InvokeTreePropertyChanged(object value, string path)
        {
            OnTreePropertyChanged?.Invoke(value, path);
        }
    }
}