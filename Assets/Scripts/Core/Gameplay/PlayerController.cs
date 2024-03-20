using DG.Tweening;
using Shuile.Framework;
using Shuile.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public class PlayerController : MonoBehaviour, IAttackable
    {
        [SerializeField] private PlayerPropertySO property;
        [SerializeField] private SpriteRenderer mRenderer;

        private ISceneLoader sceneLoader;

        private void Start()
        {
            MainGame.Interface.TryGet(out sceneLoader);
            property.currentHealthPoint = property.maxHealthPoint;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            //GUI.skin.label.fontSize = 40;
            //GUILayout.Label($"Health:{property.currentHealthPoint}");
        }

#endif

        public void Attack()
        {
            // 效果反馈
            transform.DOScale(1.2f, 0.1f).OnComplete(
                () => transform.DOScale(1f, 0.1f));

            // 搜索敌人
            if (EnemyManager.Instance.TryGetEnemyAtPosition(Mathf.RoundToInt(transform.position.x), out var enemy))
            {
                enemy.OnAttack(property.attackPoint);
            }
        }

        public void Move(float xDirection)
        {
            transform.DOMoveX(transform.position.x + xDirection, 0.2f)
                     .SetEase(Ease.OutSine);
            //transform.position += Vector3.right * xDirection;
        }

        public void OnAttack(int attackPoint)
        {
            property.currentHealthPoint -= attackPoint;

            // 受伤反馈
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f/255f, 73f/255f, 73f/255f), 0.2f).OnComplete(()=>
                mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(()=>
                    transform.position = initPos);

            // 检测死亡
            if (property.currentHealthPoint <= 0)
            {
                property.currentHealthPoint = 0;
                // 触发死亡事件
                sceneLoader.LoadSceneAsync(new SceneInfo() { SceneName = "MainGame"}, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}
