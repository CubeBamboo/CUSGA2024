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

        [SerializeField] private TextMeshProUGUI text_SongName;
        [SerializeField] private TextMeshProUGUI text_SongArtist;

        [SerializeField] private MainMenuUIStateMachine stateMachine;
        [SerializeField] private LevelSelectDataSO levelSelectData;

        private int currentIndex = 0;

        private void Awake()
        {
            ConfigureButtonEvent();
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

        public void StartLevel(string label)
        {
            var level = GameResources.Instance.levelDataMap.GetLevelData(label);
            LevelDataBinder.Instance.SetLevelData(level);
            MonoGameRouter.Instance.ToLevelScene(level.sceneName);
        }

        private void RefreshSongView()
        {
            var curr = levelSelectData.levelData[currentIndex];
            text_SongName.text = curr.songName;
            text_SongArtist.text = curr.songArtist;
        }
    }
}
