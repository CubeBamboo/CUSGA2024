using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EditorObjectQueryWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private StyleSheet m_StyleSheet;
    private List<Transform> _queryResults = new();

    private TextField _headGameObjectNameField;
    private TextField _componentTypeField;

    private void OnEnable()
    {
        Query();
        m_StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Shuile.Editor/EditorObjectQueryWindow.uss")  ?? throw new FileNotFoundException();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        root.Clear();

        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        var listView = root.Q<ListView>();

        listView.itemsSource = _queryResults;
        listView.fixedItemHeight = 20;
        listView.makeItem = () =>
        {
            var button = new Button();
            button.styleSheets.Add(m_StyleSheet);
            button.AddToClassList("query-button");
            return button;
        };
        listView.bindItem = (e, i) =>
        {
            var button = (Button)e;
            var x = _queryResults[i];
            button.text = x.name;
            button.clicked += () => Selection.activeGameObject = x.gameObject;
        };

        _headGameObjectNameField = root.Q<TextField>("HeadGameObject");
        _headGameObjectNameField.Q<Label>().text = "HeadGameObject";
        _headGameObjectNameField.value = "";
        _headGameObjectNameField.RegisterValueChangedCallback((x) =>
        {
            Query();
        });

        _componentTypeField = root.Q<TextField>("ComponentType");
        _componentTypeField.Q<Label>().text = "ComponentType";
        _componentTypeField.value = "";
        _componentTypeField.RegisterValueChangedCallback((x) =>
        {
            Query();
        });
    }

    private void OnValidate()
    {
        CreateGUI();
    }

    private void Query()
    {
        var headGameObjectName = _headGameObjectNameField.value;
        var enableHeadGoName = !string.IsNullOrEmpty(headGameObjectName);

        var componentType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsSubclassOf(typeof(Component)))
            .FirstOrDefault(x => x.Name == _componentTypeField.value);
        var enableComponentType = !string.IsNullOrEmpty(_componentTypeField.value);

        var allTransforms = SceneManager
            .GetActiveScene()
            .GetRootGameObjects()
            .SelectMany(x => x.GetComponentsInChildren<Transform>(true))
            .Where(Filter);

        _queryResults = allTransforms.ToList();
        rootVisualElement.Q<ListView>().itemsSource = _queryResults;

        return;

        bool Filter(Transform x)
        {
            if (enableHeadGoName && !string.IsNullOrEmpty(headGameObjectName))
            {
                var tr = x;
                while (tr != null)
                {
                    if (CompareName(tr.name, headGameObjectName))
                    {
                        return true;
                    }
                    tr = tr.parent;
                }
                return false;
            }

            if (enableComponentType && componentType != null && x.GetComponent(componentType) == null)
            {
                return false;
            }

            return true;
        }

        bool CompareName(string origin, string key)
        {
            origin = origin.Replace(" ", string.Empty).ToUpper();
            key = key.Replace(" ", string.Empty).ToUpper();
            return origin.Contains(key);
        }
    }

    [MenuItem("Shuile/EditorObjectQuery")]
    public static void ShowWindow()
    {
        EditorObjectQueryWindow wnd = GetWindow<EditorObjectQueryWindow>();
        wnd.titleContent = new GUIContent("EditorObjectQuery");
    }
}
