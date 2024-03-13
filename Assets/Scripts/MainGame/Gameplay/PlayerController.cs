using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CUSGA2024
{
    public class PlayerController : MonoBehaviour
    {
        public void Attack()
        {
            transform.DOScale(1.2f, 0.1f).OnComplete(
                () => transform.DOScale(1f, 0.1f));
        }

        public void Move(float xDirection)
        {
            transform.DOMoveX(transform.position.x + xDirection, 0.3f)
                     .SetEase(Ease.OutSine);
        }
    }
}
