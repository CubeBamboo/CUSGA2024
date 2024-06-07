using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI
{
    [ExecuteAlways]
    public class ShitUIVertexAnim : MaskableGraphic
    {
        [SerializeField] private List<Transform> vertices = new();
        [SerializeField] private List<Vector3Int> triangles = new();

        protected override void Awake()
        {
            base.Awake();
            material = material == null ? defaultGraphicMaterial : material;
        }

        private void Update()
        {
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            foreach (var vert in vertices)
                vh.AddVert(vert.localPosition, color, Vector4.zero);
            foreach (var t in triangles)
                vh.AddTriangle(t.x, t.y, t.z);
        }
    }
}