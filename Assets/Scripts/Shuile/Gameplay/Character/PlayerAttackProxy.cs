using CbUtils;
using Shuile.Core.Gameplay.Common;
using Shuile.Framework;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public partial class NormalPlayerCtrl
    {
        private class PlayerAttackProxy : BaseProxy
        {
            private readonly bool _needHitWithRhythm;

            private readonly Transform _transform;
            private readonly PlayerModel _playerModel;
            private readonly MusicRhythmManager _musicRhythmManager;
            private readonly EasyEvent<bool> _weaponAttack;
            private readonly PlayerChartManager _playerChartManager;
            private readonly AttackSettings _attackSettings;
            private readonly GamePlayScene.GameplayStatics _statics;

            public PlayerAttackProxy(UnityEntryPointScheduler scheduler, IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                dependencies
                    .Resolve(out _transform)
                    .Resolve(out _playerModel)
                    .Resolve(out _musicRhythmManager)
                    .Resolve(out _playerChartManager)
                    .Resolve(out _attackSettings)
                    .Resolve(out _statics)
                    .Resolve(out NormalPlayerInput mPlayerInput)
                    .Resolve(out NormalPlayerCtrl playerCtrl);

                _weaponAttack = playerCtrl.OnWeaponAttack;
                mPlayerInput.OnAttackStart.Register(_ => Attack());

                scheduler.AddOnce(Start);

                _needHitWithRhythm = GameApplication.BuiltInData.levelConfig.needHitWithRhythm;
            }

            private void Start()
            {
            }

            private bool CheckRhythm()
            {
                if (_playerChartManager.TryHitNoteNow())
                {
                    _playerModel.currentHitOffset = _playerChartManager.LastHitNote - _musicRhythmManager.CurrentTime;
                    return true; // minimal code so not handle statics logic here
                }

                return false;
            }

            private void Attack()
            {
                _statics.TotalHit++;

                if (_needHitWithRhythm && !CheckRhythm())
                {
                    return;
                }

                _statics.HitOnRhythm++;

                var attackRadius = _attackSettings.attackRadius;
                var attackPoint = _attackSettings.attackPoint;
                var hits = Physics2D.OverlapCircleAll(_transform.position, attackRadius, LayerMask.GetMask("Enemy"));
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<IHurtable>(out var hurt))
                    {
                        hurt.OnHurt(attackPoint);
                    }
                }

                _weaponAttack.Invoke(true);
            }
        }
    }
}
