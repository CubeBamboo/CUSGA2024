using CbUtils;

using Shuile.Gameplay.Entity.States;

using System.Collections.Generic;

using UnityEngine;

namespace Shuile.Gameplay.Entity.Gadgets
{
    public class Pusher : Gadget
    {
        public float pushForce = 10f;
        private List<Rigidbody2D> bodies = new(2);

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
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                bodies.Add(rb);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            State = EntityStateType.Idle;
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                bodies.UnorderedRemove(rb);
        }
    }
}