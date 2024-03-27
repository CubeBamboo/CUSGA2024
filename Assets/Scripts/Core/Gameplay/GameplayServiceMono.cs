using Shuile.Framework;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class GameplayServiceMono : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        private void Awake()
        {
            GameplayService.Interface.Register(playerController);
        }

        private void OnDestroy()
        {
            GameplayService.Interface.UnRegister<PlayerController>();
            GameplayService.Interface.UnregisterNotMono();
        }
    }

    public class GameplayService : AbstractLocator<GameplayService>
    {
        protected override void OnInit()
        {
            // Register in GameplayServiceMono
            this.Register(Resources.Load<PrefabConfigSO>("GlobalPrefabConfig")); //TODO: other load
        }

        public void UnregisterNotMono()
        {
            this.UnRegister<PrefabConfigSO>();
        }
    }
}
