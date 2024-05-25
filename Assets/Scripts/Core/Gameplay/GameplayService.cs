using CbUtils.Kits.Tasks;
using Shuile.Framework;
using Shuile.ResourcesManagement.Loader;
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
        public override bool InitOnApplicationAwake => false;

        public override void OnInit()
        {
            this.Register<PlayerModel>(new PlayerModel());
            this.Register<LevelModel>(new LevelModel());
            this.Register<IRouteFinder>(new SimpleRouteFinder());
        }

        public override void OnDeInit()
        {
            this.UnRegister<PlayerModel>();
            this.UnRegister<LevelModel>();
            this.UnRegister<IRouteFinder>();
            levelModel.SetValueWithoutEvent(null);
        }

        private CustomLoadObject<LevelModel> levelModel = new(() => Interface.Get<LevelModel>());
        public LevelModel LevelModel => levelModel.Value;
    }
}
