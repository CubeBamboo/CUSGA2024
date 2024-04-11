using TMPro;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Shuile.Gameplay;

namespace CbUtils
{
    public class CbFoo : MonoBehaviour
    {
        public GameObject enemy;

        private void Start()
        {
            EntityManager.Instance.SpawnEnemy(enemy, transform.position);
        }
    }
}
