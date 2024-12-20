using DG.Tweening;
using System.Linq;
using UnityEngine;

namespace Shuile
{
    [RequireComponent(typeof(Transform))]
    public class PathMoveAnim : MonoBehaviour
    {
        [SerializeField] private Transform[] pointObjects;
        [SerializeField] private float speed = 4f;

        public bool autoDestroyPointObjects = true;

        private Vector3[] positions;

        private void Start()
        {
            Initialize();
            Play();
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        private void Initialize()
        {
            if (pointObjects.Length == 0)
            {
                positions = new Vector3[0];
                return;
            }


            positions = pointObjects.Skip(1).Select(v => v.position).ToArray();
            //positions = pointObjects.Select(p => p.position).ToArray();
            transform.position = pointObjects[0].position;

            if (autoDestroyPointObjects)
            {
                pointObjects.ToList().ForEach(p => Destroy(p.gameObject));
            }
        }

        private void Play()
        {
            if (positions.Length == 0)
            {
                return;
            }

            transform.DOLocalPath(positions, speed, PathType.CatmullRom, PathMode.Ignore, 10, Color.green)
                .SetSpeedBased(true)
                .SetOptions(true)
                .SetLookAt(0.01f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}
