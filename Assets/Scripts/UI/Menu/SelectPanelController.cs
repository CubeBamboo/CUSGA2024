using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    public class SelectPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_level1;
        [SerializeField] private Button btn_level2;
        [SerializeField] private Button btn_level3;
        [SerializeField] private Button btn_Return;

        [SerializeField] private MainMenuUIStateMachine stateMachine;

        private void Awake()
        {
            ConfigureButtonEvent();
        }

        private void ConfigureButtonEvent()
        {
            btn_Return.onClick.AddListener(() => stateMachine.SwitchState(MainMenuUIStateMachine.State.Menu));

            // seealso: GameResources.Instance.levelDataMap.levelDataList
            btn_level1.onClick.AddListener(() => StartLevel("Break"));
            btn_level2.onClick.AddListener(() => StartLevel("Forever_loved"));
            btn_level3.onClick.AddListener(() => StartLevel("Ginevra"));
        }

        public void StartLevel(string label)
        {
            var level = GameResources.Instance.levelDataMap.GetLevelData(label);
            LevelDataBinder.Instance.SetLevelData(level);
            MonoGameRouter.Instance.ToLevelScene(level.sceneName);
        }
    }
}
