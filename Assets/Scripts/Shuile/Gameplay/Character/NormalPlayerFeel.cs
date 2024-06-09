using CbUtils.ActionKit;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global.Config;
using Shuile.Gameplay.Feel;
using Shuile.Gameplay.Move;
using Shuile.Model;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    // player feedback and other event
    public class NormalPlayerFeel : MonoBehaviour, IEntity
    {
        private LevelModel _levelModel;
        private PlayerModel _playerModel;
        private MusicRhythmManager _musicRhythmManager;
        private LevelFeelManager _levelFeelManager;

        private Player player;
        private NormalPlayerCtrl playerCtrl;
        private PlayerAnimCtrl animCtrl;
        private Rigidbody2D _rb;

        private SmoothMoveCtrl _moveController;
        private const float HurtXForce = 6f;
        private const float HurtYForce = 0.2f;

        private void Awake()
        {
            _levelModel = this.GetModel<LevelModel>();
            _playerModel = this.GetModel<PlayerModel>();
            _levelFeelManager = this.GetUtility<LevelFeelManager>();

            _musicRhythmManager = MusicRhythmManager.Instance;
            _moveController = GetComponent<SmoothMoveCtrl>();
            player = GetComponent<Player>();
            playerCtrl = GetComponent<NormalPlayerCtrl>();
            _rb = GetComponent<Rigidbody2D>();

            animCtrl = new(gameObject, _playerModel);
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
                _levelFeelManager.CameraShake();
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
                if (enable) _levelFeelManager.PlayParticle("SwordSlash", transform.position, new Vector2(_playerModel.faceDir, 0), transform);
                if (enable) _levelModel.DangerScore += DangerLevelConfigClass.PlayerAttackAddition;
            });

            playerCtrl.OnMoveStart.Register(v =>
            {
                animCtrl.FlipX = v < 0;
            });
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}