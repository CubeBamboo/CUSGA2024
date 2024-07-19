/***************************************
 * original source code from phigrim
 ************************************/

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shuile.Editor.EditorTool
{
    public class EditorToolboxWindow : EditorWindow
    {
        // private readonly BindingFlags _privateBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        // private string _drawNoteHeightMapMax = "100";

        #region Methods: GUI

        private void DrawTimeScaleSetting()
        {
            GUILayout.Label("Time Scale: " + Time.timeScale);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.05x"))
                Time.timeScale = 0.05f;
            if (GUILayout.Button("0.25x"))
                Time.timeScale = 0.25f;
            if (GUILayout.Button("0.5x"))
                Time.timeScale = 0.5f;
            if (GUILayout.Button("1x"))
                Time.timeScale = 1f;
            if (GUILayout.Button("2x"))
                Time.timeScale = 2f;
            if (GUILayout.Button("4x"))
                Time.timeScale = 4f;
            if (GUILayout.Button("10x"))
                Time.timeScale = 10f;
            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            DrawTimeScaleSetting();

            if (GUILayout.Button("Find all UI material")) FindAllUIMaterial();
            if (GUILayout.Button("Re-save All Asset")) ReSaveAllAsset();

            var autoReturnToFirstSceneEnabled = EditorToolkit.AutoReturnToFirstScene;
            if (GUILayout.Button("Return to scene(index-0) when play: " + (autoReturnToFirstSceneEnabled ? "On" : "Off")))
                EditorToolkit.AutoReturnToFirstScene = !autoReturnToFirstSceneEnabled;
        }

        #endregion

        #region Methods: Actions

        private static void FindAllUIMaterial()
        {
            var list = new List<Image>();
            foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
                list.AddRange(obj.GetComponentsInChildren<Image>());

            foreach (var obj in list)
            {
                if (ReferenceEquals(obj.material, null)) continue;
                EditorGUIUtility.PingObject(obj.gameObject);
            }
        }

        private static void ReSaveAllAsset()
        {
            var assets = AssetDatabase.FindAssets("");

            Debug.Log($"Re-saving {assets.Length} assets");
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorUtility.SetDirty(asset);
            }

            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}
