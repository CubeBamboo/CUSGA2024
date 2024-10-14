using CbUtils.Extension;
using DG.Tweening;
using UnityEngine;

namespace Shuile.Gameplay.Entity
{
    // this partial class used to handle animation logic with unity. logic see in -> ExplodeHandler
    public partial class Bomb : MonoBehaviour
    {
        [SerializeField] private LayerMask hurtMask;
        [SerializeField] private Color explodeColor = new(238f / 255, 84f / 255, 84f / 255);
        [SerializeField] private Vector3 spriteScale;
        [SerializeField] private Vector3 explodeScale;

        private SpriteRenderer _renderer;

        private ExplodeHandler _explodeHandler;

        private Transform RendererTransform => _renderer.transform;

        private void Awake()
        {
            _explodeHandler = new ExplodeHandler
            {
                OriginPosition = transform.position,
                Radius = 4f,
                AttackPoint = 150,
                HurtMask = hurtMask
            };
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            var initColor = Color.white;
            initColor.a = 0.4f;
            _renderer.color = initColor;

            RendererTransform.localScale = Vector3.zero;
            RendererTransform.DOScale(spriteScale, 0.3f);
        }

        private void OnDestroy()
        {
            RendererTransform.DOKill();
            _renderer.DOKill();
            this.DOKill();
        }

        private void HandleExplode(int attackPoint, float radius)
        {
            _explodeHandler.AttackPoint = attackPoint;
            _explodeHandler.Radius = radius;
            _explodeHandler.Explode();
        }

        private void PlayEffectAnimWithDestroy()
        {
            // play effect
            var seq = DOTween.Sequence()
                .Append(RendererTransform.DOScale(explodeScale, 0.1f))
                .Join(_renderer.DOColor(explodeColor, 0.2f))
                .Join(_renderer.DOFade(1f, 0.2f))
                .AppendInterval(0.5f)
                .Append(RendererTransform.DOScale(0f, 0.6f))
                .Join(_renderer.DOColor(Color.white, 0.6f));

            seq.SetTarget(this);
            seq.OnComplete(gameObject.Destroy);
        }

        public void Explode(int attackPoint, float radius)
        {
            HandleExplode(attackPoint, radius);
            PlayEffectAnimWithDestroy();
        }

        public void Interrupt()
        {
            PlayEffectAnimWithDestroy();
        }
    }
}
