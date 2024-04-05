using Shuile.Framework;

using UnityEngine;

namespace Shuile.Gameplay
{
    /*public class GameplayServiceMono : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        private void Awake()
        {
            GameplayService.Interface.Register(playerController);
        }

        private void OnDestroy()
        {
            GameplayService.Interface.UnRegister<PlayerController>();
            GameplayService.Interface.OnDeInit();
        }
    }*/

    public class GameplayService : AbstractLocator<GameplayService>
    {
        public override bool IsGlobal => false;

        public override void OnInit()
        {
            this.Register<PrefabConfigSO>(Resources.Load<PrefabConfigSO>("Gameplay/GlobalPrefabConfig")); //TODO: other load
            this.Register<Player>(Object.FindObjectOfType<Player>());
            this.Register<PlayerModel>(new PlayerModel());
            this.Register<IRouteFinder>(new SimpleRouteFinder());
        }

        public override void OnDeInit()
        {
            this.UnRegister<PrefabConfigSO>();
            this.UnRegister<Player>();
            this.UnRegister<PlayerModel>();
            this.UnRegister<IRouteFinder>();
        }
    }
}
