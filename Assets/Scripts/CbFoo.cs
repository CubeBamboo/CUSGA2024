/*using Shuile.Gameplay.Entity;
using Shuile;
using Shuile.Rhythm.Runtime;
using UnityEditor;
using UnityEngine;

namespace CbUtils
{
    public class CbFoo : MonoBehaviour
    {
        public GameObject bombPrefab;
        public int attackPoint = 100;
        public float explodeRadius = 2f;

        private void Start()
        {
            
        }
    }
    
    [CustomEditor(typeof(CbFoo))]
    public class InspectorExtExample : Editor
    {
        private CbFoo obj;
        private Bomb bomb;

        private void OnEnable()
        {
            obj = target as CbFoo;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("spawn laser"))
            {
                new SpawnLaserNoteData().Process();
            }
            if (GUILayout.Button("spawn bomb"))
            {
                var pos = LevelZoneManager.Instance.RandomValidPosition();
                bomb = Instantiate(obj.bombPrefab, pos, Quaternion.identity).GetComponent<Bomb>();
            }
            if (GUILayout.Button("explode bomb"))
            {
                bomb.Explode(obj.attackPoint, obj.explodeRadius);
            }
        }
    }
}
*/
