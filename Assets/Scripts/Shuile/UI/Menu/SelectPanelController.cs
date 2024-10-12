using CbUtils.Kits.Tasks;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay;
using Shuile.Gameplay.Model;
using Shuile.UI.Data;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.UI.Menu
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

        private int _currentIndex;
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
            _currentIndex = 0;
            RefreshSongView();
        }

        private void ConfigureButtonEvent()
        {
            btn_Return.onClick.AddListener(() => stateMachine.SwitchState(MainMenuUIStateMachine.State.Menu));
            btn_Next.onClick.AddListener(() =>
            {
                _currentIndex++;
                _currentIndex %= levelSelectData.levelData.Length;
                RefreshSongView();
            });
            btn_Prev.onClick.AddListener(() =>
            {
                _currentIndex--;
                _currentIndex = (_currentIndex + levelSelectData.levelData.Length) % levelSelectData.levelData.Length;
                RefreshSongView();
            });
            btn_Play.onClick.AddListener(() => StartLevel(levelSelectData.levelData[_currentIndex].levelDataLabel));
        }

        public async void StartLevel(string label)
        {
            await TaskBus.Instance.Run(LoadLevelResources(label));
            MonoGameRouter.Instance.LoadScene(new LevelSceneMeta(new LevelContext(_level)));
        }

        private async Task LoadLevelResources(string label)
        {
            var levels = GameApplication.BuiltInData.levelDataMap;
            _level = levels.FirstByLabel(label);
            await Task.Delay(1000); // show our cool loading screen // 展示我们牛逼的加载界面
        }

        private void RefreshSongView()
        {
            var curr = levelSelectData.levelData[_currentIndex];
            text_SongName.text = curr.songName;
            text_SongArtist.text = curr.songArtist;
        }
    }
}
