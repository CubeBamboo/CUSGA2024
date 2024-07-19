/***************************************
 * original source code from phigrim
 ************************************/

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shuile.Editor.EditorTool
{
    [InitializeOnLoad]
    public static class EditorToolkit
    {
        private const string AutoReturnToFirstScenePrefsKey = "__Editor_AutoReturnToFirstScene";

        static EditorToolkit()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        public static bool AutoReturnToFirstScene
        {
            get => PlayerPrefs.GetInt(AutoReturnToFirstScenePrefsKey, 1) != 0;
            set => PlayerPrefs.SetInt(AutoReturnToFirstScenePrefsKey, value ? 1 : 0);
        }

        private static void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (!AutoReturnToFirstScene) return;
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                {
                    if (SceneManager.GetActiveScene().buildIndex != 0)
                        SceneManager.LoadScene(0);

                    Debug.ClearDeveloperConsole();
                    break;
                }
            }
        }
    }
}
