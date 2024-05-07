namespace Shuile.Persistent
{
    public class Config : PersistentData<Config>
    {
        private int globalDelay = 0;
        private int bgAudioVolume = 100;
        private int fxAudioVolume = 100;
        private bool vibrationFeel = true;

        public int GlobalDelay
        {
            get => globalDelay;
            set => this.UpdateStructProperty(ref globalDelay, value);
        }
        public int BgAudioVolume
        {
            get => bgAudioVolume;
            set => this.UpdateStructProperty(ref bgAudioVolume, value);
        }
        public int FxAudioVolume
        {
            get => fxAudioVolume;
            set => this.UpdateStructProperty(ref fxAudioVolume, value);
        }
        public bool VibrationFeel
        {
            get => vibrationFeel;
            set => this.UpdateStructProperty(ref vibrationFeel, value);
        }

        public Config()
        {
        }


        /*public class AudioConfig : INotifyTreePropertyChanged
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
        }*/
    }
}