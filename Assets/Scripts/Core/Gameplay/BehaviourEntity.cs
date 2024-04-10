using CbUtils;

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
        Gadget,  // 机关
        Prop,  // 道具
        Enemy,  // 敌人
    }

    public abstract class BehaviourEntity : MonoBehaviour, IJudgeable
    {
        protected readonly EntityType type;
        protected int lastJudgeFrame;
        protected FSM<EntityStateType> fsm;

        protected BehaviourEntity(EntityType type)
        {
            this.type = type;
        }

        public EntityStateType State
        {
            get => fsm.CurrentStateId;
            set
            {
                if (value == fsm.CurrentStateId)
                    return;

                fsm.SwitchState(value);
            }
        }
        
        public EntityType Type => type;

        public int LastJudgeFrame => lastJudgeFrame;

        protected virtual void Awake()
        {
            fsm = new();
            RegisterState(fsm);
        }

        protected abstract void RegisterState(FSM<EntityStateType> fsm);

        public virtual void Judge(int frame, bool force)
        {
            if (frame == lastJudgeFrame && !force)
                return;

            lastJudgeFrame = frame;
            fsm.Custom();
        }

        protected virtual void Update()
        {
            fsm.Update();
        }

        protected virtual void FixedUpdate()
        {
            fsm.FixedUpdate();
        }

        protected virtual void OnGUI()
        {
            fsm.OnGUI();
        }
    }
}
