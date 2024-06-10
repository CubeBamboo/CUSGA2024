using CbUtils.Extension;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterView : MonoBehaviour, IEntity
    {
        private CameraDrifterController _controller;

        private readonly ViewEntityProperty<Vector2> targetPosition = new();

        public Vector2 originPosition { get; set; }
        public float moveScale { get; set; } = 0.1f;
        public float moveSpeed { get; set; } = 0.01f;
        public float moveRadius { get; set; } = 0.1f;

        private void Awake()
        {
            _controller = gameObject.GetOrAddComponent<CameraDrifterController>();

            targetPosition.DirtyCheck = (a, b) => (a - b).sqrMagnitude > 1e-12f;
            targetPosition.OnValueDirty(OnValueDirty);

            originPosition = transform.position;
        }
        private void Start()
        {
            // params
            var data = LevelDataGetter.Instance.cameraParallaxMove;

            originPosition = data.origin.position;
            moveScale = data.moveScale;
            moveRadius = data.moveRadius;
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
            transform.position = new Vector3(targetPosition.RawValue.x, targetPosition.RawValue.y, transform.position.z);

            if (!_controller.HasTargetPosition) return;
            targetPosition.TargetValue = GetUsingTargetValue(_controller.TargetPosition);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
