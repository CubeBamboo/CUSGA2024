using Cysharp.Threading.Tasks;

using System.Collections.Generic;

namespace Shuile.Persistent
{
    public class Viewer<T> where T : PersistentData<T>, new()
    {
        public delegate void BoolStateChangedHandler(bool value);

        private T data;
        private IAccessor<T> accessor;
        private bool _isDirty;
        
        public event BoolStateChangedHandler OnDirtyStateChanged;

        public Viewer(T data, IAccessor<T> accessor)
        {
            this.data = data;
            this.accessor = accessor;
            this.data.OnTreePropertyChanged += OnTreePropertyChanged;
        }

        public T Data => data;
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value)
                    return;

                _isDirty = value;
                OnDirtyStateChanged?.Invoke(IsDirty);
            }
        }
        public bool AutoSave { get; set; } = false;

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
            await accessor.SaveAsync(data);
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