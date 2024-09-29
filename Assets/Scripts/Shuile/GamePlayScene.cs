using Shuile.Core.Gameplay;
using Shuile.Framework;
using Shuile.Gameplay.Character;
using Shuile.Gameplay.Feel;
using UnityEngine;

namespace Shuile
{
    public class GamePlayScene : MonoContainer
    {
        [SerializeField] private Player player;

        public override void BuildContext(ServiceLocator context)
        {
            context.RegisterInstance(this);

            context.RegisterFactory(() => new LevelFeelManager());
            context.RegisterFactory(() => new LevelStateMachine());
        }

        public bool TryGetPlayer(out Player instance)
        {
            if (player != null)
            {
                instance = player;
                return true;
            }

            instance = player = GetComponentInChildren<Player>();
            return player == null;
        }
    }
}
