using CbUtils;

using Shuile.Gameplay.Entity.States;

using System.Collections.Generic;

using UnityEngine;

namespace Shuile.Gameplay.Entity.Gadgets
{
    /*public class Pusher : Gadget
    {
        public float pushForce = 10f;
        [SerializeField] private List<Rigidbody2D> bodies = new(2);

        protected override void RegisterState(FSM<EntityStateType> fsm)
        {
            fsm.AddState(EntityStateType.Spawn, new SpawnState(this));
            fsm.AddState(EntityStateType.Idle, EmptyState.instance);
            fsm.NewEventState(EntityStateType.Attack)
                .OnCustom(() =>
                {
                    foreach (var rb in bodies)
                        rb.velocity += new Vector2(Mathf.Sign(rb.velocity.x) * pushForce, 0f);
                });
            fsm.AddState(EntityStateType.Dead, new DeadState(this));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            State = EntityStateType.Attack;
            bodies.Add(other.attachedRigidbody);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            bodies.UnorderedRemove(other.attachedRigidbody);
            State = bodies.Count == 0 ? EntityStateType.Idle : EntityStateType.Attack;
        }
    }*/
}
