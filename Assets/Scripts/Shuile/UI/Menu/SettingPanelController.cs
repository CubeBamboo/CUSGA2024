using Shuile.Core;
using Shuile.Framework;
using Shuile.Persistent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Menu
{
    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_Return;
        [SerializeField] private GameObject returnPanel;

        [SerializeField] private Button btn_LatencyAdd;
        [SerializeField] private Button btn_LatencyMinus;
        [SerializeField] private TextMeshProUGUI text_Latency;

        private Viewer<Config> _configViewer;

        private const int LATENCY_LARGE_STEP = 10;
        private const int MAX_LATENCY = 1000;

        private void Start()
        {
            InitializeDependency();
            ConfigureButtonEvent();
            InitializeState();
        }
        private void OnDestroy()
        {
            _configViewer = null;
        }

        private void Return()
        {
            gameObject.SetActive(false);
            returnPanel.SetActive(true);
        }

        private void UpdateLatency(int value)
        {
            if (Mathf.Abs(value) > MAX_LATENCY) return;

            _configViewer.Data.GlobalDelay = value;
            text_Latency.text = $"Latency: {_configViewer.Data.GlobalDelay} ms";
        }

        private void InitializeDependency()
        {
            _configViewer = MainGame.Interface.CreatePersistentDataViewer<Config, MainGame>();
            _configViewer.AutoSave = true;
        }

        private void InitializeState()
        {
            text_Latency.text = $"Latency: {_configViewer.Data.GlobalDelay} ms";
        }

        private void ConfigureButtonEvent()
        {
            btn_Return.onClick.AddListener(Return);
            btn_LatencyAdd.onClick.AddListener(()=> UpdateLatency(_configViewer.Data.GlobalDelay + LATENCY_LARGE_STEP));
            btn_LatencyMinus.onClick.AddListener(()=> UpdateLatency(_configViewer.Data.GlobalDelay - LATENCY_LARGE_STEP));
        }



    }
}
