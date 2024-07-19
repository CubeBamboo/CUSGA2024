using System;
using UnityEngine;

namespace CbUtils.Event
{
    public class Collider2DEventMono : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEntered?.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            CollisionExited?.Invoke(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            CollisionStayed?.Invoke(collision);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TriggerEntered?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            TriggerExited?.Invoke(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            TriggerStayed?.Invoke(collision);
        }

        public event Action<Collision2D> CollisionEntered, CollisionStayed, CollisionExited;
        public event Action<Collider2D> TriggerEntered, TriggerStayed, TriggerExited;
    }
}
