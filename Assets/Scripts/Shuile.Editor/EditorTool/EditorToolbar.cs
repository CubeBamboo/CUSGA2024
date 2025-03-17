/***************************************
 * original source code from phigrim
 ************************************/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Shuile.Editor.EditorTool
{
    [InitializeOnLoad]
    public static class EditorToolbar
    {
        private static int _currentResolutionIndex;

        private static string _lastSceneName;

        private static void OnLeftZoneGUI()
        {
            GUILayout.BeginHorizontal();

            if (!EditorApplication.isPlaying && GUILayout.Button("Save Scene")) EditorSceneManager.SaveOpenScenes();

            if (GUILayout.Button("Edit GUI"))
            {
                var view = SceneView.sceneViews[0] as SceneView;
                view.in2DMode = true;
                view.orthographic = true;
                Selection.activeGameObject = GameObject.Find("Canvas");
                SceneView.FrameLastActiveSceneView();
            }


            if (EditorGUILayout.DropdownButton(new GUIContent("Scenes"), FocusType.Passive)) ScenesMenu.ShowAsContext();

            GUILayout.EndHorizontal();
        }

        private static void OnRightZoneGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Switch 3D/2D"))
            {
                var view = SceneView.sceneViews[0] as SceneView;
                view.in2DMode = !view.in2DMode;
                view.orthographic = view.in2DMode;
            }

            if (GUILayout.Button("Toolbox")) EditorWindow.GetWindow<EditorToolboxWindow>();

            GUILayout.EndHorizontal();
        }

        #region Toolbar Inject

        private static readonly Type ToolbarType;
        private static ScriptableObject _lastToolbar;
        private static readonly GenericMenu ScenesMenu;

        static EditorToolbar()
        {
            EditorApplication.update += OnUpdate;
            ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

            ScenesMenu = new GenericMenu();

            var scenePath = Path.Join(Application.dataPath, "Scenes");
            if (!Directory.Exists(scenePath))
            {
                return;
            }

            foreach (var se in Directory.EnumerateFiles(scenePath)
                         .Where(x => Path.GetExtension(x) == ".unity")
                         .Where(x => !Path.GetFileName(x).StartsWith('_'))
                         .OrderBy(x => x[0]))
            {
                var name = Path.GetFileNameWithoutExtension(se);
                ScenesMenu.AddItem(new GUIContent(name), false, () => LoadScene(name));
            }

            var examplesPath = Path.Join(scenePath, "Examples");
            if (Directory.Exists(examplesPath))
            {
                var examplesScenes = Directory.EnumerateFiles(examplesPath, "*.unity")
                    //.Where(x => Path.GetExtension(x) == ".unity")
                    .Where(x => !Path.GetFileName(x).StartsWith('_'))
                    .OrderBy(x => x[0]).ToArray();

                if (examplesScenes.Any())
                {
                    ScenesMenu.AddSeparator("");

                    foreach (var se in examplesScenes)
                    {
                        var name = "Examples/" + Path.GetFileNameWithoutExtension(se);
                        ScenesMenu.AddItem(new GUIContent(name), false, () => LoadScene(name));
                    }

                    ScenesMenu.AddItem(new GUIContent("Explore Examples In Project"), false, () =>
                    {
                        var folderObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Scenes/Examples");
                        EditorGUIUtility.PingObject(folderObject);
                        Selection.activeObject = folderObject;
                    });
                }
            }
        }

        private static void LoadScene(string sceneName)
        {
            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        private static void OnUpdate()
        {
            var toolbar = LocateToolbar();
            if (toolbar == _lastToolbar) return;
            if (toolbar != null)
                InjectToolbarEvent(toolbar);
            _lastToolbar = toolbar;
        }

        private static void InjectToolbarEvent(ScriptableObject toolbar)
        {
            var root = ToolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var concreteRoot = root.GetValue(toolbar) as VisualElement;

            {
                var toolbarRightZone = concreteRoot.Q("ToolbarZoneRightAlign");
                var parent = new VisualElement
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row
                    }
                };
                var container = new IMGUIContainer();
                container.onGUIHandler += OnRightZoneGUI;
                parent.Add(container);
                toolbarRightZone.Add(parent);
            }
            {
                var toolbarLeftZone = concreteRoot.Q("ToolbarZoneLeftAlign");
                var parent = new VisualElement
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Column,
                        alignItems = Align.FlexEnd
                    }
                };
                var container = new IMGUIContainer();
                container.onGUIHandler += OnLeftZoneGUI;
                parent.Add(container);

                toolbarLeftZone.Add(parent);
            }
        }

        private static ScriptableObject LocateToolbar()
        {
            var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            return toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
        }

        #endregion
    }
}
