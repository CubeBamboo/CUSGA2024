using Cysharp.Threading.Tasks;

namespace Shuile.Persistent
{
    public class Viewer<T> where T : PersistentData<T>, new()
    {
        public delegate void BoolStateChangedHandler(bool value);

        private bool _isDirty;
        private readonly IAccessor<T> accessor;

        public Viewer(T data, IAccessor<T> accessor)
        {
            Data = data;
            this.accessor = accessor;
            Data.OnTreePropertyChanged += OnTreePropertyChanged;
        }

        public T Data { get; }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value)
                {
                    return;
                }

                _isDirty = value;
                OnDirtyStateChanged?.Invoke(IsDirty);
            }
        }

        public bool AutoSave { get; set; } = false;

        public event BoolStateChangedHandler OnDirtyStateChanged;

        private async void OnTreePropertyChanged(object value, string path)
        {
            IsDirty = true;

            if (AutoSave && accessor.IsRandomRWSupported)
            {
                await accessor.SaveAsync(path, value);
                IsDirty = false;
            }
        }

        public async UniTask SaveAsync()
        {
            await accessor.SaveAsync(Data);
            IsDirty = false;
        }
    }

    public static class ViewerExtension
    {
        public static UniTask SaveIfDirty<T>(this Viewer<T> viewer) where T : PersistentData<T>, new()
        {
            return viewer.IsDirty ? viewer.SaveAsync() : UniTask.CompletedTask;
        }
    }
}
