using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button btn_Exit;
        [SerializeField] private Button btn_level1;
        [SerializeField] private Button btn_level2;
        [SerializeField] private Button btn_level3;

        private void Awake()
        {
            ConfigureButtonEvent();
        }

        private void ConfigureButtonEvent()
        {
            btn_Exit.onClick.AddListener(ExitGame);

            // seealso: GameResources.Instance.levelDataMap.levelDataList
            btn_level1.onClick.AddListener(() => StartLevel("Break"));
            btn_level2.onClick.AddListener(() => StartLevel("Forever_loved"));
            btn_level3.onClick.AddListener(() => StartLevel("Ginevra"));
        }

        private void StartLevel(string label)
        {
            var level = GameResources.Instance.levelDataMap.GetLevelData(label);
            LevelDataBinder.Instance.SetLevelData(level);
            MonoGameRouter.Instance.ToLevelScene(level.sceneName);
        }

        private void ExitGame()
        {
#if !UNITY_EDITOR
            Application.Quit();
#else
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
