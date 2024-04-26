using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [RequireComponent(typeof(Canvas))]
    public class WorldCanvasSelfFiller : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "Character";
            canvas.sortingOrder = 0;
        }
    }
}
