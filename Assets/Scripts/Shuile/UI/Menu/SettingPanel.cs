using Cysharp.Threading.Tasks;
using Shuile.Persistent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Menu
{
    public class SettingPanel : MonoBehaviour
    {
        [SerializeField] private Slider bgAudioVolumeSlider;
        [SerializeField] private TMP_Text bgAudioVolumeText;
        [SerializeField] private Slider fxAudioVolumeSlider;
        [SerializeField] private TMP_Text fxAudioVolumeText;
        [SerializeField] private Slider delaySlider;
        [SerializeField] private TMP_Text delayText;
        [SerializeField] private Toggle vibrationFeelToggle;

        [Header("Other")] [SerializeField] private Button btn_Return;

        [SerializeField] private MainMenuUIStateMachine stateMachine;

        private Viewer<Config> _configViewer;

        private void Awake()
        {
            ConfigEventListener();
        }

        //public override void Show()
        private void OnEnable()
        {
            _configViewer = MainGame.Interface.CreatePersistentDataViewer<Config, MainGame>();
            _configViewer.Data.OnTreePropertyChanged += OnConfigPropertyChanged;
            _configViewer.AutoSave = true;

            bgAudioVolumeSlider.onValueChanged.AddListener(OnBgAudioVolumeSliderValueChanged);
            fxAudioVolumeSlider.onValueChanged.AddListener(OnFxAudioVolumeSliderValueChanged);
            delaySlider.onValueChanged.AddListener(OnGlobalDelaySliderValueChanged);
            vibrationFeelToggle.onValueChanged.AddListener(OnVibrationFeelToggleValueChanged);

            OnConfigPropertyChanged(_configViewer.Data.BgAudioVolume, nameof(Config.BgAudioVolume));
            OnConfigPropertyChanged(_configViewer.Data.FxAudioVolume, nameof(Config.FxAudioVolume));
            OnConfigPropertyChanged(_configViewer.Data.GlobalDelay, nameof(Config.GlobalDelay));
            OnConfigPropertyChanged(_configViewer.Data.VibrationFeel, nameof(Config.VibrationFeel));
        }

        //public override void Hide()
        private void OnDisable()
        {
            _configViewer.Data.OnTreePropertyChanged -= OnConfigPropertyChanged;
            _configViewer.SaveIfDirty().Forget();
            bgAudioVolumeSlider.onValueChanged.RemoveListener(OnBgAudioVolumeSliderValueChanged);
            fxAudioVolumeSlider.onValueChanged.RemoveListener(OnFxAudioVolumeSliderValueChanged);
            delaySlider.onValueChanged.RemoveListener(OnGlobalDelaySliderValueChanged);
            vibrationFeelToggle.onValueChanged.RemoveListener(OnVibrationFeelToggleValueChanged);
        }

        // TODO: change logic // TODO: use settings when enter the game.
        private void OnBgAudioVolumeSliderValueChanged(float value)
        {
            _configViewer.Data.BgAudioVolume = (int)value;
        }

        private void OnFxAudioVolumeSliderValueChanged(float value)
        {
            _configViewer.Data.FxAudioVolume = (int)value;
        }

        private void OnGlobalDelaySliderValueChanged(float value)
        {
            _configViewer.Data.GlobalDelay = (int)value;
        }

        private void OnVibrationFeelToggleValueChanged(bool value)
        {
            _configViewer.Data.VibrationFeel = value;
        }

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

        private void ConfigEventListener()
        {
            btn_Return.onClick.AddListener(() => stateMachine.SwitchState(MainMenuUIStateMachine.State.Menu));
        }
    }
}
