using CbUtils.Extension;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraDrifterView : MonoBehaviour
    {
        private CameraDrifterController _controller;

        private Vector2 _targetPosition = new();
        private Vector2 _rawPosition;

        public Vector2 originPosition { get; set; }
        public float moveScale { get; set; } = 0.1f;
        public float moveSpeed { get; set; } = 0.01f;
        public float moveRadius { get; set; } = 0.1f;

        private void Awake()
        {
            _controller = gameObject.GetOrAddComponent<CameraDrifterController>();

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
            if (!_controller.HasTargetPosition) return;
            _targetPosition = GetUsingTargetValue(_controller.TargetPosition);
            
            if ((_targetPosition - _rawPosition).sqrMagnitude < 1e-12f) return;
            _rawPosition = Vector2.Lerp(transform.position, _targetPosition, moveSpeed);
            
            transform.position = new Vector3(_rawPosition.x, _rawPosition.y, transform.position.z);
        }
    }
}
