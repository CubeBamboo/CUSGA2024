using Cysharp.Threading.Tasks;

using Shuile.Framework;
using Shuile.Persistent;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI
{
    public class SettingPanel : BasePanelWithMono
    {
        [SerializeField] private Slider bgAudioVolumeSlider;
        [SerializeField] private TMP_Text bgAudioVolumeText;
        [SerializeField] private Slider fxAudioVolumeSlider;
        [SerializeField] private TMP_Text fxAudioVolumeText;
        [SerializeField] private Slider delaySlider;
        [SerializeField] private TMP_Text delayText;
        [SerializeField] private Toggle vibrationFeelToggle;

        private Viewer<Config> configViewer;

        public override void Show()
        {
            configViewer = MainGame.Interface.CreatePersistentDataViewer<Config, MainGame>();
            configViewer.Data.OnTreePropertyChanged += OnConfigPropertyChanged;
            bgAudioVolumeSlider.onValueChanged.RemoveListener(OnBgAudioVolumeSliderValueChanged);
            fxAudioVolumeSlider.onValueChanged.RemoveListener(OnFxAudioVolumeSliderValueChanged);
            delaySlider.onValueChanged.RemoveListener(OnGlobalDelaySliderValueChanged);
            vibrationFeelToggle.onValueChanged.RemoveListener(OnVibrationFeelToggleValueChanged);

            OnConfigPropertyChanged(configViewer.Data.BgAudioVolume, nameof(Config.BgAudioVolume));
            OnConfigPropertyChanged(configViewer.Data.FxAudioVolume, nameof(Config.FxAudioVolume));
            OnConfigPropertyChanged(configViewer.Data.GlobalDelay, nameof(Config.GlobalDelay));
            OnConfigPropertyChanged(configViewer.Data.VibrationFeel, nameof(Config.VibrationFeel));
        }

        public override void Hide()
        {
            configViewer.Data.OnTreePropertyChanged -= OnConfigPropertyChanged;
            configViewer.SaveIfDirty().Forget();
            bgAudioVolumeSlider.onValueChanged.AddListener(OnBgAudioVolumeSliderValueChanged);
            fxAudioVolumeSlider.onValueChanged.AddListener(OnFxAudioVolumeSliderValueChanged);
            delaySlider.onValueChanged.AddListener(OnGlobalDelaySliderValueChanged);
            vibrationFeelToggle.onValueChanged.AddListener(OnVibrationFeelToggleValueChanged);
        }

        private void OnBgAudioVolumeSliderValueChanged(float value)
            => configViewer.Data.BgAudioVolume = (int)value;
        private void OnFxAudioVolumeSliderValueChanged(float value)
            => configViewer.Data.FxAudioVolume = (int)value;
        private void OnGlobalDelaySliderValueChanged(float value)
            => configViewer.Data.GlobalDelay = (int)value;
        private void OnVibrationFeelToggleValueChanged(bool value)
            => configViewer.Data.VibrationFeel = value;

        private void OnConfigPropertyChanged(object value, string path)
        {
            if (path == nameof(Config.BgAudioVolume))
            {
                bgAudioVolumeSlider.value = (int)value;
                bgAudioVolumeText.text = value.ToString();
            }
            else if (path == nameof(Config.FxAudioVolume))
            {
                fxAudioVolumeSlider.value = (int)value;
                fxAudioVolumeText.text = value.ToString();
            }
            else if (path == nameof(Config.GlobalDelay))
            {
                delaySlider.value = (int)value;
                delayText.text = ((int)value).ToString("+0") + "ms";
            }
            else if (path == nameof(Config.VibrationFeel))
            {
                vibrationFeelToggle.isOn = (bool)value;
            }
        }
    }
}