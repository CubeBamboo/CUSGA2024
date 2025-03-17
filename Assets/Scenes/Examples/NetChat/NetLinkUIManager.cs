using Shuile;
using Shuile.Core.Gameplay.Data;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Examples.NetChat
{
    public class NetLinkUIManager : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button guestButton;

        [SerializeField] private TMP_InputField ipInput;

        private void Start()
        {
            hostButton.onClick.AddListener(() => StartLink(true));
            guestButton.onClick.AddListener(() => StartLink(false));
        }

        public void StartLink(bool isServer)
        {
            var config = new NetChatConfig();

            if (isServer)
            {
                config.IsServer = true;
                config.ServerIp = "127.0.0.1";
            }
            else
            {
                config.IsServer = false;
                var input = ipInput.text;
                if (string.IsNullOrEmpty(input))
                {
                    Debug.LogError("Please input the server ip.");
                    return;
                }

                config.ServerIp = input;
            }

            /*var context = new ServiceLocator();
            context.RegisterInstance(config);
            MonoGameRouter.Instance.LoadScene(new MonoGameRouter.NestedSceneMeta("NetChatExamples", context));*/

            var data = GameApplication.BuiltInData.levelDataMap.FirstByLabel("Ginevra");
            var load = new LevelData
            {
                label = data.label, sceneName = "NetLevel2", enemyData = data.enemyData, chartFiles = data.chartFiles,
                songName = data.songName,
                composer = data.composer
            };

            var serviceLocator = new ServiceLocator();
            serviceLocator.RegisterInstance(config);
            var levelMeta = new LevelSceneMeta(new SingleLevelData(load));
            MonoGameRouter.Instance.LoadScene(new MonoGameRouter.NestedSceneMeta(levelMeta, serviceLocator));
        }
    }
}
