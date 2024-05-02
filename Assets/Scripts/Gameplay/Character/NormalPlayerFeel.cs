using CbUtils.ActionKit;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay
{
    // player feedback and other event
    public class NormalPlayerFeel : MonoBehaviour
    {
        private Player player;
        private NormalPlayerCtrl playerCtrl;
        private NormalPlayerInput playerInput;
        private PlayerAnimCtrl animCtrl;
        private Rigidbody2D _rb;

        private PlayerModel playerModel;
        private SmoothMoveCtrl _moveController;
        private LevelModel levelModel;

        private const float HurtXForce = 6f;
        private const float HurtYForce = 0.2f;

        private void Awake()
        {
            levelModel = GameplayService.Interface.Get<LevelModel>();
            playerModel = GameplayService.Interface.Get<PlayerModel>();

            _moveController = GameplayService.Interface.Get<PlayerModel>().moveCtrl;
            player = GetComponent<Player>();
            playerCtrl = GetComponent<NormalPlayerCtrl>();
            playerInput = GetComponent<NormalPlayerInput>();
            _rb = GetComponent<Rigidbody2D>();

            animCtrl = new(gameObject);
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
                MusicRhythmManager.Instance.FadeOutAndStop(); // 当前音乐淡出
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            player.OnHurted.Register(() =>
            {
                _moveController.Velocity = new Vector2(playerModel.faceDir * HurtXForce, HurtYForce);
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Hurt);
                LevelFeelManager.Instance.CameraShake();
                MonoAudioCtrl.Instance.PlayOneShot("Player_Hurt");
                levelModel.DangerScore -= DangerLevelConfigClass.PlayerHurtReduction;

                playerModel.isInviciable = true;
                ActionCtrl.Delay(0.5f).OnComplete(() => playerModel.isInviciable = false).Start(gameObject);

                LevelFeelManager.Instance.VignettePulse();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            playerCtrl.attackCommand.RegisterCommandAfter(() =>
            {
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Attack);
                //MonoAudioCtrl.Instance.PlayOneShot("Player_Attack");

                levelModel.DangerScore += DangerLevelConfigClass.PlayerAttackAddition;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            playerCtrl.jumpCommand.RegisterCommandAfter(() =>
            {
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Jump);
                //MonoAudioCtrl.Instance.PlayOneShot("Player_Jump");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            playerCtrl.moveCommand.RegisterCommandAfter(() =>
            {
                animCtrl.FlipX = playerInput.XInput < 0;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            playerCtrl.OnTouchGround.Register(() =>
            {
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Land);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}
