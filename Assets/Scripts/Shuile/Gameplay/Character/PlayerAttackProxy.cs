using CbUtils;
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
            private TryHitNoteCommand _hitNoteCommand;
            private AttackCommand _attackCommand;

            private readonly Transform _transform;
            private readonly PlayerModel _playerModel;
            private readonly MusicRhythmManager _musicRhythmManager;
            private readonly EasyEvent<bool> _weaponAttack;
            private readonly PlayerChartManager _playerChartManager;
            private readonly AttackSettings _attackSettings;

            public bool CheckRhythm
            {
                get
                {
                    _hitNoteCommand.inputTime = _musicRhythmManager.CurrentTime;
                    _hitNoteCommand.Execute();
                    _playerModel.currentHitOffset = _hitNoteCommand.result.hitOffset;
                    return _hitNoteCommand.result.isHitOn;
                }
            }

            public PlayerAttackProxy(UnityEntryPointScheduler scheduler, IReadOnlyServiceLocator dependencies) : base(scheduler, dependencies)
            {
                dependencies
                    .Resolve(out _transform)
                    .Resolve(out _playerModel)
                    .Resolve(out _musicRhythmManager)
                    .Resolve(out _playerChartManager)
                    .Resolve(out _attackSettings)
                    .Resolve(out NormalPlayerInput mPlayerInput)
                    .Resolve(out NormalPlayerCtrl playerCtrl);

                _weaponAttack = playerCtrl.OnWeaponAttack;
                mPlayerInput.OnAttackStart.Register(_ => Attack());

                scheduler.AddOnce(Start);
            }

            private void Start()
            {
                _attackCommand = new AttackCommand
                {
                    position = _transform.position, attackRadius = _attackSettings.attackRadius, attackPoint = _attackSettings.attackPoint
                };
                _hitNoteCommand = new TryHitNoteCommand
                {
                    musicRhythmManager = _musicRhythmManager,
                    playerChartManager = _playerChartManager,
                    inputTime = _musicRhythmManager.CurrentTime
                };
            }

            private void Attack()
            {
                if (LevelRoot.Instance.needHitWithRhythm && !CheckRhythm)
                {
                    return;
                }

                _attackCommand.position = _transform.position;
                _attackCommand.Execute();

                _weaponAttack.Invoke(true);
            }
        }
    }
}
