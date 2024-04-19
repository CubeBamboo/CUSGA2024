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

        private void Awake()
        {
            ConfigureButtonEvent();
        }

        private void ConfigureButtonEvent()
        {
            btn_Exit.onClick.AddListener(ExitGame);
            btn_level1.onClick.AddListener(() => StartLevel(1));
            //btn_level2.onClick.AddListener(() => StartLevel(2));
        }

        private void StartLevel(int index)
        {
            MonoGameRouter.Instance.ToLevelScene(index);
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
