using UnityEngine;

namespace Shuile.UI
{
    public class NoiseMovement : MonoBehaviour
    {
        public float noiseScale = 10f;
        public float movementSpeed = 1f;
        public float offset;
        public bool randomOffset = true;

        private Vector3 _originalPosition;

        private void Start()
        {
            if (randomOffset)
            {
                offset = Random.Range(0, 1000);
            }

            OnStart();
        }

        private void Update()
        {
            var xNoise = Mathf.PerlinNoise(offset + Time.time * movementSpeed, 0);
            var yNoise = Mathf.PerlinNoise(0, offset + Time.time * movementSpeed);
            var noiseValue = new Vector2(xNoise, yNoise) - new Vector2(0.5f, 0.5f); // to [-0.5, 0.5]

            UpdatePosition(noiseScale * noiseValue);
        }

        protected virtual void OnStart()
        {
            _originalPosition = transform.position;
        }

        protected virtual void UpdatePosition(Vector2 scaledNoiseValue)
        {
            var noisePosition = new Vector3(scaledNoiseValue.x, scaledNoiseValue.x, 0);
            transform.position = _originalPosition + noisePosition;
        }
    }
}
