using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CbUtils.Example
{
    
    [CustomEditor(typeof(CbFoo))]
    public class InspectorExtExample : Editor
    {
        //private XX obj;
        
        private void OnEnable()
        {
            //obj = target as XX;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("NormalPlay"))
            {
                //some code...
                CbFoo foo = (CbFoo)target;
                foo.Play();
            }

            if (GUILayout.Button("PlayDelay"))
            {
                //some code...
                CbFoo foo = (CbFoo)target;
                foo.PlayDelayed();
            }

            if (GUILayout.Button("PlayScheduled"))
            {
                //some code...
                CbFoo foo = (CbFoo)target;
                foo.PlayScheduled();
            }
        }
    }
    
    
    /*
    public class EditorExt : MonoBehaviour
    {
        [MenuItem("Custom/Foo")]
        public static void InitMap()
        {
            Debug.Log("Foo");
        }
    }
    */
}
