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

    public abstract class BehaviourEntity : MonoBehaviour
    {
        protected EntityStateType state = EntityStateType.Idle;
        protected Dictionary<EntityStateType, EntityState> states = new();
        private EntityState currentState = EmptyState.instance;
        private Vector3Int gridPosition;

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

            if (newStateBehaviour != currentState)
            {
                currentState.ExitState();
                state = newState;
                currentState = newStateBehaviour;
                currentState.EnterState();
            }
        }
        
        public void Judge()
        {
            currentState.Judge();
        }
    }
}