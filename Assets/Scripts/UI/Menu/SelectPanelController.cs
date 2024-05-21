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
            var level = await TaskBus.Instance.Execute(LoadLevelResources(label).AsTask());
            //var level = await TaskBus.Instance.Execute(LoadLevelResources(label));
            // TODO: [!] orgin unitask has some propblem that lead to program end after "await" keyword.
            // this code will leads to bugs: "var level = await TaskBus.Instance.Execute(LoadLevelResources(label));"
            // code below can run successfully:
            // - "var level = await TaskBus.Instance.Execute(LoadLevelResources(label).AsTask());"
            // - "var level = await TaskBus.Instance.Execute(LoadLevelResourcesUseTask(label).AsUniTask());"  // ??????

            LevelDataBinder.Instance.SetLevelData(level);
            MonoGameRouter.Instance.ToLevelScene(level.sceneName);
        }

        private async UniTask<LevelData> LoadLevelResources(string label)
        {
            var levels = await GameResourcesLoader.Instance.GetLevelDataMapAsync();

            var level = levels.GetLevelData(label);
            await LevelResourcesLoader.Instance.PreCacheAsync();
            await UniTask.Delay(1000);
            return level;
        }

        private void RefreshSongView()
        {
            var curr = levelSelectData.levelData[currentIndex];
            text_SongName.text = curr.songName;
            text_SongArtist.text = curr.songArtist;
        }
    }
}
