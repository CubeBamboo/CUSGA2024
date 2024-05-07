using Shuile.Gameplay.Event;
using UnityEngine;

namespace Shuile.Gameplay.Feel
{
    public class CameraParallaxMoveCtrl : MonoBehaviour
    {
        private Transform origin;
        private Transform player;
        private float moveScale = 0.1f;
        private float moveSpeed = 0.02f;
        private float moveRadius = 0.1f;
        //private Vector2 moveRange = Vector2.one * 0.1f;

        private void Awake() => LevelLoadEndEvent.Register(OnInit);
        private void OnDestroy() => LevelLoadEndEvent.UnRegister(OnInit);

        private void OnInit(string sceneName)
        {
            var data = LevelDataGetter.Instance.cameraParallaxMove;
            origin = data.origin;
            player = data.player;
            moveScale = data.moveScale;
            moveSpeed = data.moveSpeed;
            moveRadius = data.moveRadius;
        }

        private void LateUpdate()
        {
            if (player == null) return;

            var offset = player.position - origin.position;
            var originPos = origin.position;
            offset = Vector2.Lerp(originPos, offset, moveScale);

            var useOffset = offset;
            if(offset.sqrMagnitude > moveRadius * moveRadius)
                useOffset = useOffset.normalized * moveRadius;

            var endPos = originPos + useOffset;
            endPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, endPos, moveSpeed);
            
            OnInit(""); // for debug
        }
    }
}
