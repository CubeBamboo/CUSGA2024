using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifter : MonoBehaviour
    {
        private Transform _transform;
        private readonly ViewEntityProperty<Vector2> targetPosition = new();

        public Vector2 TargetPosition
        {
            get => targetPosition.TargetValue;
            set => targetPosition.TargetValue = value;
        }

        public Vector2 originPosition { get; set; }
        public float moveScale { get; set; } = 0.1f;
        public float moveSpeed { get; set; } = 0.01f;
        public float moveRadius { get; set; } = 0.1f;

        private void Awake()
        {
            targetPosition.DirtyCheck = (a, b) => (a - b).sqrMagnitude > 1e-12f;
            targetPosition.OnValueDirty(OnValueDirty);

            _transform = transform;
            originPosition = _transform.position;
        }
        private Vector2 OnValueDirty(Vector2 target)
        {
            return Vector2.Lerp(transform.position, target, moveSpeed);
        }

        public Vector2 GetUsingTargetValue(Vector2 target)
        {
            // camera will move in a circle
            var offset = target - originPosition;
            var originPos = originPosition;
            offset = Vector2.Lerp(originPos, offset, moveScale);

            var useOffset = offset;
            if (offset.sqrMagnitude > moveRadius * moveRadius)
                useOffset = useOffset.normalized * moveRadius;
            return originPos + useOffset;
        }

        private void LateUpdate()
        {
            targetPosition.TryUpdateDirtValue();
            _transform.position = new Vector3(targetPosition.RawValue.x, targetPosition.RawValue.y, _transform.position.z);
        }
    }
}
