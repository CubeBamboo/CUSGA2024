using DG.Tweening;

using Shuile.Gameplay.Entity;

using System.Collections.Generic;

using UnityEngine;

namespace Shuile.Gameplay
{
    public enum EntityStateType
    {
        Spawn,  // 出生
        Idle,  // 摸鱼/移动
        Attack,  // 攻击
        Dead,  // 寄了
    }

    public enum EntityType
    {
        Trap,  // 机关
        Gadget,  // 道具
        Enemy,  // 敌人
    }

    public abstract class BehaviourEntity : MonoBehaviour, IJudgeable
    {
        protected EntityStateType state = EntityStateType.Idle;
        protected Dictionary<EntityStateType, EntityState> states = new();
        protected EntityState stateBehaviour = EmptyState.instance;
        protected int lastJudgeFrame;
        protected IMoveController moveController;

        public event OnEntityStateChanged OnStateChanged;

        public EntityStateType State => state;

        internal int LastJudgeFrame => lastJudgeFrame;
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public IMoveController MoveController => moveController;

        protected virtual void Awake()
        {
            moveController = GetComponent<IMoveController>();
            RegisterState();
        }

        protected virtual void Start()
        {
            state = (EntityStateType)(-1);
            GotoState(EntityStateType.Spawn);
        }

        protected abstract void RegisterState();

        public void GotoState(EntityStateType newState)
        {
            if (state == newState)
                return;

            if (!states.TryGetValue(newState, out var newStateBehaviour))
                stateBehaviour = EmptyState.instance;

            var from = state;
            state = newState;
            if (newStateBehaviour != stateBehaviour)
            {
                stateBehaviour.ExitState();
                stateBehaviour = newStateBehaviour;
                stateBehaviour.EnterState();
            }
            OnStateChanged?.Invoke(from, state);
        }

        public void Judge(int frame, bool force)
        {
            if (lastJudgeFrame == frame && !force)
                return;

            lastJudgeFrame = frame;
            stateBehaviour.Judge();
        }
    }
}
