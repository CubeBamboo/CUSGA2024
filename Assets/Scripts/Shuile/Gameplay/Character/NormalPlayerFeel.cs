using CbUtils.ActionKit;
using Plugins.Framework;
using Shuile.Core.Framework;
using Shuile.Core.Global;
using Shuile.Core.Global.Config;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Move;
using Shuile.Model;
using Shuile.Rhythm;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    // player feedback and other event
    public class NormalPlayerFeel : GameObjectContainer
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

        public override void ResolveContext(IReadOnlyServiceLocator context)
        {
            base.ResolveContext(context);
            context.Resolve(out _levelFeelManager);
        }

        protected override void Awake()
        {
            var services = GameApplication.GlobalService;
            _sceneTransitionManager = services.Get<SceneTransitionManager>();

            var scope = LevelScope.Interface;
            _levelModel = scope.GetImplementation<LevelModel>();
            _playerModel = scope.GetImplementation<PlayerModel>();

            _musicRhythmManager = scope.GetImplementation<MusicRhythmManager>();
            _moveController = GetComponent<SmoothMoveCtrl>();
            player = GetComponent<Player>();
            playerCtrl = GetComponent<NormalPlayerCtrl>();
            _rb = GetComponent<Rigidbody2D>();

            animCtrl = new PlayerAnimCtrl(gameObject, _playerModel);
            ConfigureFeelEvent();
        }

        private void FixedUpdate()
        {
            animCtrl.XVelocity = _rb.velocity.x;
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

            playerCtrl.OnMoveStart.Register(v =>
            {
                animCtrl.FlipX = v < 0;
            });
        }
    }
}
