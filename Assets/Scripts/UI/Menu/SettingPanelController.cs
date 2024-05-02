using Shuile.Framework;
using Shuile.Persistent;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

namespace Shuile
{
    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_Return;
        [SerializeField] private GameObject returnPanel;

        [SerializeField] private Button btn_LatencyAdd;
        [SerializeField] private Button btn_LatencyMinus;
        [SerializeField] private TextMeshProUGUI text_Latency;

        private Viewer<Config> configViewer;

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
            configViewer = null;
        }

        private void Return()
        {
            gameObject.SetActive(false);
            returnPanel.SetActive(true);
        }

        private void UpdateLatency(int value)
        {
            if (Mathf.Abs(value) > MAX_LATENCY) return;

            configViewer.Data.Audio.GlobalDelay = value;
            text_Latency.text = $"Latency: {configViewer.Data.Audio.GlobalDelay} ms";
        }

        private void InitializeDependency()
        {
            configViewer = MainGame.Interface.CreatePersistentDataViewer<Config, MainGame>();
            configViewer.AutoSave = true;
        }

        private void InitializeState()
        {
            text_Latency.text = $"Latency: {configViewer.Data.Audio.GlobalDelay} ms";
        }

        private void ConfigureButtonEvent()
        {
            btn_Return.onClick.AddListener(Return);
            btn_LatencyAdd.onClick.AddListener(()=> UpdateLatency(configViewer.Data.Audio.GlobalDelay + LATENCY_LARGE_STEP));
            btn_LatencyMinus.onClick.AddListener(()=> UpdateLatency(configViewer.Data.Audio.GlobalDelay - LATENCY_LARGE_STEP));
        }



    }
}
