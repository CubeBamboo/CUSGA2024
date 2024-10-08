using CbUtils.ActionKit;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Framework;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Move;
using Shuile.Model;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    // player feedback and other event
    public class NormalPlayerFeel : PlainContainer
    {
        private const float HurtXForce = 6f;
        private const float HurtYForce = 0.2f;
        private LevelFeelManager _levelFeelManager;
        private LevelModel _levelModel;
        private SceneTransitionManager _sceneTransitionManager;

        private SmoothMoveCtrl _moveController;
        private MusicRhythmManager _musicRhythmManager;
        private PlayerModel _playerModel;
        private Rigidbody2D _rb;
        private PlayerAnimCtrl animCtrl;

        private Player player;
        private NormalPlayerCtrl playerCtrl;
        private GameObject gameObject;

        private Transform transform;

        public NormalPlayerFeel(UnityEntryPointScheduler scheduler)
        {
            scheduler.AddFixedUpdate(FixedUpdate);
        }

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            base.LoadFromParentContext(context);
            context
                .Resolve(out _levelFeelManager)
                .Resolve(out _moveController)
                .Resolve(out player)
                .Resolve(out _playerModel)
                .Resolve(out _musicRhythmManager)
                .Resolve(out transform)
                .Resolve(out playerCtrl)
                .Resolve(out _rb)
                .Resolve(out _levelModel)
                .Resolve(out _sceneTransitionManager);

            ConfigureFeelEvent();
        }

        public override void BuildSelfContext(RuntimeContext context)
        {
            base.BuildSelfContext(context);
            context.Inject(animCtrl = new PlayerAnimCtrl());
        }

        private void FixedUpdate()
        {
            animCtrl.XVelocity = _rb.velocity.x;
            if (Abs(_playerModel.faceDir) > 1e-6)
            {
                animCtrl.FlipX = _playerModel.faceDir < 0;
            }

            return;

            float Abs(float x) => x > 0 ? x : -x;
        }

        private void ConfigureFeelEvent()
        {
            player.OnDie.Register(() =>
            {
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Die);
                //MonoAudioCtrl.Instance.PlayOneShot("Player_Death");
                _musicRhythmManager.FadeOutAndStop(); // 当前音乐淡出
            });

            player.OnHurted.Register(() =>
            {
                _moveController.Velocity = new Vector2(_playerModel.faceDir * HurtXForce, HurtYForce);
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Hurt);
                _levelFeelManager.CameraShake(token: _sceneTransitionManager.SceneChangedToken);
                //MonoAudioCtrl.Instance.PlayOneShot("Player_Hurt");
                _levelModel.DangerScore -= DangerLevelConfigClass.PlayerHurtReduction;

                if (_playerModel.canInviciable)
                {
                    _playerModel.isInviciable = true;
                    animCtrl.Inviciable = true;

                    ActionCtrl.Delay(1.5f).OnComplete(() =>
                        {
                            animCtrl.Inviciable = false;
                            _playerModel.isInviciable = false;
                        })
                        .SetDebounce("PlayerHurt")
                        .Start(gameObject);
                }

                _levelFeelManager.VignettePulse();
            });

            playerCtrl.OnWeaponAttack.Register(enable =>
            {
                //animCtrl.TriggerAttackWithType(enable, playerCtrl.CurrentWeapon.Type);
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Run);
                if (enable)
                {
                    _levelFeelManager.PlayParticle("SwordSlash", transform.position,
                        new Vector2(_playerModel.faceDir, 0), transform);
                }

                if (enable)
                {
                    _levelModel.DangerScore += DangerLevelConfigClass.PlayerAttackAddition;
                }
            });
        }
    }
}
