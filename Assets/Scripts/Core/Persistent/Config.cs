namespace Shuile.Persistent
{
    public class Config : PersistentData<Config>
    {
        private AudioConfig audio;

        public AudioConfig Audio
        {
            get => audio;
            set => this.UpdateClassProperty(ref audio, value);
        }

        public Config()
        {
            audio = new AudioConfig();
            audio.OnTreePropertyChanged += INotifyTreePropertyChangedExtension.BuildEventLinker(this, nameof(Audio));
        }


        public class AudioConfig : INotifyTreePropertyChanged
        {
            private int globalDelay = 0;

            /// <summary>
            /// (in ms) time need to delay when judge rhythm
            /// </summary>
            public int GlobalDelay
            {
                get => globalDelay;
                set => this.UpdateStructProperty(ref globalDelay, value);
            }

            public event TreePropertyChangedEventHandler OnTreePropertyChanged;

            public void InvokeTreePropertyChanged(object value, string path)
                => OnTreePropertyChanged?.Invoke(value, path);
        }
    }
}