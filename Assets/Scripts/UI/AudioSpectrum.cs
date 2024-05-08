using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class AudioSpectrum : MaskableGraphic
    {
        [SerializeField] private Texture texture = s_WhiteTexture;
        [SerializeField] private AudioSource source;
        [SerializeField] private int columnCount;
        [SerializeField] private float paddingRate;
        [SerializeField] private AnimationCurve valueMapper;
        [SerializeField] private RangeInt range;
        private float[] data = new float[256];

        public AudioSource Source
        {
            get => source;
            set
            {
                if (source == null && value != null || (source != null && value == null))
                {
                    source = value;
                    SetVerticesDirty();
                }
                source = value;
            }
        }
        public int ColumnCount
        {
            get => columnCount;
            set
            {
                columnCount = value;
                SetVerticesDirty();
            }
        }
        public float PaddingRate
        {
            get => paddingRate;
            set
            {
                paddingRate = value;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (texture != null)
                    return texture;

                if (material != null && material.mainTexture != null)
                    return material.mainTexture;

                return s_WhiteTexture;
            }
        }
        public Texture ColumnTexture
        {
            get => texture;
            set
            {
                if (texture != value)
                {
                    texture = value;
                    SetMaterialDirty();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            material = material == null ? material : Graphic.defaultGraphicMaterial;
        }

        private void Update()
        {
            if (source != null)
            {
                source.GetSpectrumData(data, 0, FFTWindow.Hanning);
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (source == null)
                return;

            Rect rect = GetPixelAdjustedRect();
            Color color = this.color;
            float width = 1f / (columnCount + (columnCount - 1) * paddingRate) * rect.width;
            float paddingWidth = width * paddingRate;
            for (int i = 0; i < columnCount; i++)
            {
                var fill = GetFillPercentage(i);
                vh.AddVert(new Vector3(rect.x + (width + paddingWidth) * i, rect.y + rect.height * fill), color, new Vector2(0f, 1f));
                vh.AddVert(new Vector3(rect.x + (width + paddingWidth) * i + width, rect.y + rect.height * fill), color, new Vector2(1f, 1f));
            }
            for (int i = 0; i < columnCount; i++)
            {
                vh.AddVert(new Vector3(rect.x + (width + paddingWidth) * i, rect.y), color, new Vector2(0f, 0f));
                vh.AddVert(new Vector3(rect.x + (width + paddingWidth) * i + width, rect.y), color, new Vector2(1f, 0f));

                vh.AddTriangle(2 * columnCount + 2 * i, 2 * i, 2 * i + 1);
                vh.AddTriangle(2 * i + 1, 2 * columnCount + 2 * i + 1, 2 * columnCount + 2 * i);
            }

            material.SetInteger(Shader.PropertyToID("_ColumnCount"), columnCount);
        }

        private float GetFillPercentage(int columnIndex)
            => valueMapper.Evaluate(data[columnIndex]);
    }
}