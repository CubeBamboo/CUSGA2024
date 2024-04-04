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
        protected EntityState currentState = EmptyState.instance;
        private Vector3Int gridPosition;
        protected int lastJudgeFrame;

        public event OnEntityStateChanged OnStateChanged;

        public EntityStateType State => state;

        public Vector3Int GridPosition
        {
            get => gridPosition;
            set
            {
                if (value == gridPosition)
                    return;

                LevelGrid.Instance.grid.Move(GridPosition, value);
                gridPosition = value;
                transform.DOMove(LevelGrid.Instance.grid.CellToWorld(value), 0.1f);
            }
        }
        internal int LastJudgeFrame => lastJudgeFrame;

        protected virtual void Awake()
        {
            var gridPos = LevelGrid.Instance.grid.WorldToCell(transform.position);
            LevelGrid.Instance.grid.Add(gridPos, this.gameObject);
            gridPosition = gridPos;
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
                currentState = EmptyState.instance;

            var from = state;
            state = newState;
            if (newStateBehaviour != currentState)
            {
                currentState.ExitState();
                currentState = newStateBehaviour;
                currentState.EnterState();
            }
            OnStateChanged?.Invoke(from, state);
        }

        public void Judge(int frame, bool force)
        {
            if (lastJudgeFrame == frame && !force)
                return;

            lastJudgeFrame = frame;
            currentState.Judge();
        }
    }
}