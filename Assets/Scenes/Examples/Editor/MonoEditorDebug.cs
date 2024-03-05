using UnityEngine;
using UnityEditor;
using CbUtils;

namespace CUSGA2024
{
    [CustomEditor(typeof(RythmInputHandler))]
    public class MonoEditorDebug : Editor
    {
        //private XX obj;

        private void OnEnable()
        {
            //obj = target as XX;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Switch Mode"))
            {
                //some code...
                var player = (RythmInputHandler)target;
                player.SwitchMode();
            }
        }
    }
}
