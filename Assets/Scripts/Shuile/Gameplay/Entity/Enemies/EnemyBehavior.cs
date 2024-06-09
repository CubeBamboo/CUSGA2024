using CbUtils.Extension;
using Shuile.Gameplay.Move;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public interface IBehavior
    {
        void Do();
    }

    public interface IMonoBehavior
    {
        void FixedUpdate();
        void Update();
        void OnDestroy();
    }


    /// <summary>
    /// call it on FixedUpdate
    /// </summary>
    public class ZakoPatrolBehavior : IBehavior
    {
        private GameObject target;
        private SmoothMoveCtrl moveCtrl;
        private float checkWallDist;

        private Vector2 leftPoint, rightPoint;

        float faceDir;

        bool canMove = true;

        public ZakoPatrolBehavior(GameObject target, SmoothMoveCtrl moveCtrl, float patrolDistance, float checkWallDistance = 0.8f)
        {
            this.target = target;
            this.moveCtrl = moveCtrl;
            this.checkWallDist = checkWallDistance;

            leftPoint = (Vector2)target.transform.position - new Vector2(patrolDistance, 0);
            rightPoint = (Vector2)target.transform.position + new Vector2(patrolDistance, 0);

            faceDir = 1;
        }

        public void Do()
        {
            //if(Random.Range(0, 1000) < 5)
            //    canMove = !canMove; // ai (in chinese), shit code
            if (!canMove) return;

            if (EnemyBehaviorAction.XRayCastWall(moveCtrl.Position, faceDir, checkWallDist))
                faceDir = -faceDir;
            var posX = target.transform.position.x;
            if (posX < leftPoint.x)
                faceDir = 1;
            else if (posX > rightPoint.x)
                faceDir = -1;

            if (faceDir != 0) moveCtrl.XMove(faceDir);
        }

        public float FaceDir => faceDir;
    }

    /// <summary>
    /// call it on fixedupdate
    /// </summary>
    public class ZakoChaseBehavior : IBehavior
    {
        private GameObject toChase;
        private SmoothMoveCtrl moveCtrl;

        private float faceDir;

        public void Bind(GameObject toChase, SmoothMoveCtrl moveCtrl)
        {
            this.toChase = toChase;
            this.moveCtrl = moveCtrl;
        }

        public void Do()
        {
            faceDir = Mathf.Sign(toChase.transform.position.x - moveCtrl.Position.x);
            moveCtrl.XMove(faceDir);
        }

        public float FaceDir => faceDir;
        public bool CloseEnoughToTarget(float threshold)
            => (toChase.transform.position - moveCtrl.Position).sqrMagnitude < threshold * threshold;
        public bool XCloseEnoughToTarget(float threshold)
            => Mathf.Abs(toChase.transform.position.x - moveCtrl.Position.x) < threshold;
    }

    public static class EnemyBehaviorAction
    {
        public static RaycastHit2D XRayCastPlayer(Vector2 startPosition, float direction, float maxDistance)
            => Physics2D.Raycast(startPosition, new Vector2(direction, 0), maxDistance, LayerMask.GetMask("Player"));
        public static RaycastHit2D XRayCastWall(Vector2 startPosition, float direction, float maxDistance)
            => Physics2D.Raycast(startPosition, new Vector2(direction, 0), maxDistance, LayerMask.GetMask("Ground"));

        public static void CheckWallAndJump(SmoothMoveCtrl moveCtrl, float faceDir, float checkDistance = 0.5f, bool showDebugLine = false)
        {
            if (!moveCtrl.IsOnGround) return;
            var hit = XRayCastWall(moveCtrl.Position, faceDir, checkDistance);
            if(showDebugLine) UnityAPIExtension.DebugLineForRayCast2D(moveCtrl.Position, Vector2.right * faceDir, checkDistance, LayerMask.GetMask("Ground"));
            if (hit)
                moveCtrl.SimpleJump(1f);
        }
    }
}
