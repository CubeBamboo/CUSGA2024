using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CbUtils
{
    public class CbFoo : MonoBehaviour
    {
        [SerializeField] private Button button1;
        [SerializeField] private Button button2;
        [SerializeField] private Button cancelButton;

        private CancellationTokenSource _cts;

        private async void Start()
        {
            _cts = new CancellationTokenSource();

            button2.onClick.AddListener(async ()=>
            {
                await Call2();
                throw new Exception("but");
            });

            cancelButton.onClick.AddListener(() =>
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            });
        }

        private async Task Call2()
        {
            await Task1(_cts.Token);
        }

        private async Task Task1(CancellationToken token = default)
        {
            Debug.Log("Task1 start");
            await Task.Delay(3000, token);
            Debug.Log("Task1 end");
        }

        private async UniTask Task2(CancellationToken token = default)
        {
            Debug.Log("Task2 start");
            await UniTask.Delay(3000, cancellationToken: token);
            Debug.Log("Task2 end");
        }
    }

    /*[CustomEditor(typeof(CbFoo))]
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
    }*/
}
