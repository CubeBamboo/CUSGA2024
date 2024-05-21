using CbUtils.Kits.Tasks;
using Cysharp.Threading.Tasks;
using Shuile.Core.Gameplay;
using Shuile.ResourcesManagement.Loader;
using Shuile.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile
{
    public class SelectPanelController : MonoBehaviour
    {
        [SerializeField] private Button btn_Return;
        [SerializeField] private Button btn_Next;
        [SerializeField] private Button btn_Prev;
        [SerializeField] private Button btn_Play;
        [SerializeField] private AudioSpectrum spectrum;

        [SerializeField] private TextMeshProUGUI text_SongName;
        [SerializeField] private TextMeshProUGUI text_SongArtist;

        [SerializeField] private MainMenuUIStateMachine stateMachine;
        [SerializeField] private LevelSelectDataSO levelSelectData;

        private int currentIndex = 0;
        private LevelData _level;

        private void Awake()
        {
            ConfigureButtonEvent();
        }
        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            currentIndex = 0;
            RefreshSongView();
        }

        private void ConfigureButtonEvent()
        {
            btn_Return.onClick.AddListener(() => stateMachine.SwitchState(MainMenuUIStateMachine.State.Menu));
            btn_Next.onClick.AddListener(() =>
            {
                currentIndex++;
                currentIndex %= levelSelectData.levelData.Length;
                RefreshSongView();
            });
            btn_Prev.onClick.AddListener(() =>
            {
                currentIndex--;
                currentIndex = (currentIndex + levelSelectData.levelData.Length) % levelSelectData.levelData.Length;
                RefreshSongView();
            });
            btn_Play.onClick.AddListener(() => StartLevel(levelSelectData.levelData[currentIndex].levelDataLabel));

            // seealso: GameResources.Instance.levelDataMap.levelDataList
            //btn_level1.onClick.AddListener(() => StartLevel("Break"));
            //btn_level2.onClick.AddListener(() => StartLevel("Forever_loved"));
            //btn_level3.onClick.AddListener(() => StartLevel("Ginevra"));
        }

        public async void StartLevel(string label)
        {
            //var level = await TaskBus.Instance.Execute(LoadLevelResources(label).AsTask());
            await TaskBus.Instance.Execute(LoadLevelResources(label));

            LevelDataBinder.Instance.SetLevelData(_level);
            MonoGameRouter.Instance.ToLevelScene(_level.sceneName);
        }

        private async UniTask LoadLevelResources(string label)
        {
            var levels = await GameResourcesLoader.Instance.GetLevelDataMapAsync();

            _level = levels.GetLevelData(label);
            await LevelResourcesLoader.Instance.PreCacheAsync();
            await UniTask.Delay(1000);

            //return level;
        }

        private void RefreshSongView()
        {
            var curr = levelSelectData.levelData[currentIndex];
            text_SongName.text = curr.songName;
            text_SongArtist.text = curr.songArtist;
        }
    }
}
