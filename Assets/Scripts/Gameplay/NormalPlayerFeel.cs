using Shuile.Framework;
using UnityEngine;

namespace Shuile.Gameplay
{
    // player feedback
    public class NormalPlayerFeel : MonoBehaviour
    {
        private Player player;
        private NormalPlayerCtrl playerCtrl;
        private NormalPlayerInput playerInput;
        private PlayerAnimCtrl animCtrl;
        private Rigidbody2D _rb;

        private void Awake()
        {
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
            player.OnDie.Register(() => animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Die))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            player.CurrentHp.Register((val) => animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Hurt))
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            playerCtrl.attackCommand.OnCommandAfter(() =>
            {
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Attack);
                //MonoAudioCtrl.Instance.PlayOneShot("Player_Attack");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            playerCtrl.jumpCommand.OnCommandAfter(() =>
            {
                animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Jump);
                //MonoAudioCtrl.Instance.PlayOneShot("Player_Jump");
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            playerCtrl.moveCommand.OnCommandAfter(() =>
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
