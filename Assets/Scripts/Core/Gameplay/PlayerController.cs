using DG.Tweening;
using Shuile.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    public class PlayerController : MonoBehaviour, IAttackable
    {
        [SerializeField] private PlayerPropertySO property;

        private ISceneLoader sceneLoader;

        private void Start()
        {
            MainGame.Interface.TryGet(out sceneLoader);
            property.currentHealthPoint = property.maxHealthPoint;
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 40;
            GUILayout.Label($"Health:{property.currentHealthPoint}");
        }

#endif

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

        public void OnAttack(int attackPoint)
        {
            property.currentHealthPoint -= attackPoint;
            if (property.currentHealthPoint <= 0)
            {
                property.currentHealthPoint = 0;
                // 触发死亡事件
                sceneLoader.LoadSceneAsync(new SceneInfo() { SceneName = "MainGame"}, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}
