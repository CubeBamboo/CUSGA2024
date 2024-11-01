using CbUtils.Extension;
using DG.Tweening;
using Shuile.Framework;
using Shuile.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI
{
    public class EndStaticsPanel : MonoContainer
    {
        [SerializeField] private AudioClip endBgm;
        private BackgroundMusicChannel _musicChannel;

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            base.LoadFromParentContext(context);

            context.Resolve(out Data data);

            this.GetChildByName<TextMeshProUGUI>("Title").text = data.SongName;
            this.GetChildByName<TextMeshProUGUI>("Composer").text = data.Composer;
            this.GetChildByName<InfoNumberText>("TotalHit").Number.text = data.TotalHit.ToString();
            this.GetChildByName<InfoNumberText>("HitOnRhythm").Number.text = data.HitOnRhythm.ToString();
            this.GetChildByName<InfoNumberText>("TotalKillEnemy").Number.text = data.TotalKillEnemy.ToString();
            this.GetChildByName<InfoNumberText>("Score").Number.text = data.Score.ToString();
            this.GetChildByName<InfoNumberText>("HealthLoss").Number.text = data.HealthLoss.ToString();

            if (context.TryGetValue<LevelSceneMeta>(out var prevLevel))
            {
                this.GetChildByName<Button>("Retry").onClick.AddListener(() =>
                {
                    OnExit();
                    MonoGameRouter.Instance.LoadScene(prevLevel);
                });
            }

            this.GetChildByName<Button>("Exit").onClick.AddListener(() =>
            {
                OnExit();
                MonoGameRouter.Instance.LoadFromName("MainMenu");
            });

            context.Resolve(out _musicChannel);
            _musicChannel.Play(endBgm);

            OnEnter();
        }

        private void OnEnter()
        {
            // enter animation
            var upElement = this.GetChildByName<RectTransform>("Up");
            upElement.GetComponent<RectNoiseMovement>().enabled = false;
            upElement.DOAnchorPosY(upElement.anchoredPosition.y + 300, 1.0f).From().SetEase(Ease.OutCubic).OnComplete(
                () => upElement.GetComponent<RectNoiseMovement>().enabled = true);

            var downElement = this.GetChildByName<RectTransform>("Down");
            downElement.GetComponent<RectNoiseMovement>().enabled = false;
            downElement.DOAnchorPosY(downElement.anchoredPosition.y - 300, 1.0f).From().SetEase(Ease.OutCubic).OnComplete(
                () => downElement.GetComponent<RectNoiseMovement>().enabled = true);
        }

        private void OnExit()
        {
            _musicChannel.Source.Stop();
        }

        public struct Data
        {
            public string SongName;
            public string Composer;

            public int TotalHit;
            public int HitOnRhythm;
            public int TotalKillEnemy;
            public int Score;
            public int HealthLoss;

            public static Data FromGameplayStatics(GamePlayScene.GameplayStatics statics)
            {
                return new Data
                {
                    Composer = statics.Composer,
                    SongName = statics.SongName,
                    HealthLoss = statics.HealthLoss,
                    HitOnRhythm = statics.HitOnRhythm,
                    Score = statics.Score,
                    TotalHit = statics.TotalHit,
                    TotalKillEnemy = statics.TotalKillEnemy
                };
            }
        }
    }
}
