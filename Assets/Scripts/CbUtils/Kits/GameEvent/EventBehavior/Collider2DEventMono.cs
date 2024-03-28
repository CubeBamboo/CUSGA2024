using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CbUtils.Event
{
    public class Collider2DEventMono : MonoBehaviour
    {
        public event System.Action<Collision2D> CollisionEntered, CollisionStayed, CollisionExited;
        public event System.Action<Collider2D> TriggerEntered, TriggerStayed, TriggerExited;

        private void OnTriggerEnter2D(Collider2D collision) => TriggerEntered?.Invoke(collision);

        private void OnTriggerStay2D(Collider2D collision) => TriggerStayed?.Invoke(collision);

        private void OnTriggerExit2D(Collider2D collision) => TriggerExited?.Invoke(collision);

        private void OnCollisionEnter2D(Collision2D collision) => CollisionEntered?.Invoke(collision);

        private void OnCollisionStay2D(Collision2D collision) => CollisionStayed?.Invoke(collision);

        private void OnCollisionExit2D(Collision2D collision) => CollisionExited?.Invoke(collision);
    }
}
