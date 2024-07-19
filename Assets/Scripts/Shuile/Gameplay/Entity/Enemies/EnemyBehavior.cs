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
    ///     call it on FixedUpdate
    /// </summary>
    public class ZakoPatrolBehavior : IBehavior
    {
        private readonly bool canMove = true;
        private readonly float checkWallDist;

        private readonly Vector2 leftPoint;
        private readonly Vector2 rightPoint;
        private readonly SmoothMoveCtrl moveCtrl;
        private readonly GameObject target;

        public ZakoPatrolBehavior(GameObject target, SmoothMoveCtrl moveCtrl, float patrolDistance,
            float checkWallDistance = 0.8f)
        {
            this.target = target;
            this.moveCtrl = moveCtrl;
            checkWallDist = checkWallDistance;

            leftPoint = (Vector2)target.transform.position - new Vector2(patrolDistance, 0);
            rightPoint = (Vector2)target.transform.position + new Vector2(patrolDistance, 0);

            FaceDir = 1;
        }

        public float FaceDir { get; private set; }

        public void Do()
        {
            //if(Random.Range(0, 1000) < 5)
            //    canMove = !canMove; // ai (in chinese), shit code
            if (!canMove)
            {
                return;
            }

            if (EnemyBehaviorAction.XRayCastWall(moveCtrl.Position, FaceDir, checkWallDist))
            {
                FaceDir = -FaceDir;
            }

            var posX = target.transform.position.x;
            if (posX < leftPoint.x)
            {
                FaceDir = 1;
            }
            else if (posX > rightPoint.x)
            {
                FaceDir = -1;
            }

            if (FaceDir != 0)
            {
                moveCtrl.XMove(FaceDir);
            }
        }
    }

    /// <summary>
    ///     call it on fixedupdate
    /// </summary>
    public class ZakoChaseBehavior : IBehavior
    {
        private SmoothMoveCtrl moveCtrl;
        private GameObject toChase;

        public float FaceDir { get; private set; }

        public void Do()
        {
            FaceDir = Mathf.Sign(toChase.transform.position.x - moveCtrl.Position.x);
            moveCtrl.XMove(FaceDir);
        }

        public void Bind(GameObject toChase, SmoothMoveCtrl moveCtrl)
        {
            this.toChase = toChase;
            this.moveCtrl = moveCtrl;
        }

        public bool CloseEnoughToTarget(float threshold)
        {
            return (toChase.transform.position - moveCtrl.Position).sqrMagnitude < threshold * threshold;
        }

        public bool XCloseEnoughToTarget(float threshold)
        {
            return Mathf.Abs(toChase.transform.position.x - moveCtrl.Position.x) < threshold;
        }
    }

    public static class EnemyBehaviorAction
    {
        public static RaycastHit2D XRayCastPlayer(Vector2 startPosition, float direction, float maxDistance)
        {
            return Physics2D.Raycast(startPosition, new Vector2(direction, 0), maxDistance,
                LayerMask.GetMask("Player"));
        }

        public static RaycastHit2D XRayCastWall(Vector2 startPosition, float direction, float maxDistance)
        {
            return Physics2D.Raycast(startPosition, new Vector2(direction, 0), maxDistance,
                LayerMask.GetMask("Ground"));
        }

        public static void CheckWallAndJump(SmoothMoveCtrl moveCtrl, float faceDir, float checkDistance = 0.5f,
            bool showDebugLine = false)
        {
            if (!moveCtrl.IsOnGround)
            {
                return;
            }

            var hit = XRayCastWall(moveCtrl.Position, faceDir, checkDistance);
            if (showDebugLine)
            {
                UnityAPIExtension.DebugLineForRayCast2D(moveCtrl.Position, Vector2.right * faceDir, checkDistance,
                    LayerMask.GetMask("Ground"));
            }

            if (hit)
            {
                moveCtrl.SimpleJump(1f);
            }
        }
    }
}
