using UnityEngine;

namespace CbUtils.LinkedValue
{
    /// <summary>
    ///     tweening by lerping itself
    /// </summary>
    public class LerpFloatTweeningValue : TweeningValue<float>
    {
        public override void OnUpdate()
        {
            TweenResult = Mathf.Lerp(TweenResult, Value, 0.01f);
        }
    }

    /// <summary>
    ///     tweening by velocity
    /// </summary>
    public class VelFloatTweeningValue : TweeningValue<float>
    {
        public float maxDelta = 4f; // value - tweenValue
        public float maxVelocity = 0.05f; // to tween

        public override void OnUpdate()
        {
            var delta = Mathf.Clamp01(Mathf.Abs(Value - TweenResult) / maxDelta);
            var absVelocity = Mathf.Lerp(0, maxVelocity, delta);
            TweenResult = Mathf.MoveTowards(TweenResult, Value, absVelocity);
        }
    }

    public class Vec3TweeningValue : TweeningValue<Vector3>
    {
        public float maxDelta = 10f; // value - tweenValue
        public float maxVelocity = 0.08f; // to tween

        public override void OnUpdate()
        {
            var delta = Mathf.Clamp01((Value - TweenResult).magnitude / maxDelta);
            var absVelocity = Mathf.Lerp(0, maxVelocity, delta);
            TweenResult = Vector3.MoveTowards(TweenResult, Value, absVelocity);
        }
    }

    /* ----as an example----
    public class TweeningValueExample : MonoBehaviour, IPointerMoveHandler
    {
        private Vec3TweeningValue mTweenPos = new();

        private void Start()
        {
            mTweenPos.AddToGlobalUpdater(UpdateType.Update);
        }

        private void LateUpdate()
        {
            transform.position = mTweenPos.TweenResult;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
            mTweenPos.Value = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }
    */
}
