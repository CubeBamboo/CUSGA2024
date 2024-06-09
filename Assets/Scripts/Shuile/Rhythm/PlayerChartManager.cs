using Shuile.Chart;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Core.Global.Config;
using Shuile.Gameplay;
using Shuile.ResourcesManagement.Loader;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    // manage chart of player, convert chart to runtime note object noteContainer
    public class PlayerChartManager : ISystem, IInitializeable, IFixedTickable
    {
        private System.Lazy<ChartPlayer> chartPlayer;

        // chart part
        private readonly ChartData chart = ChartDataCreator.CreatePlayerDefault();

        private float notePreShowInterval = 0.4f;
        private LevelConfigSO _levelConfig;
        private IFixedTickable _fixedTickableImplementation;
        private MusicRhythmManager _musicRhythmManager;

        public event System.Action OnPlayerHitOn;
        public NoteContainer noteContainer { get; private set;}
        
        public void Initialize()
        {
            LevelScope scope = LevelScope.Interface;

            _musicRhythmManager = MusicRhythmManager.Instance;
            var levelTimingManager = this.GetSystem<LevelTimingManager>();

            _levelConfig = LevelResourcesLoader.Instance.SyncContext.levelConfig;
            notePreShowInterval = _levelConfig.playerNotePreShowTime;

            noteContainer = new();
            chartPlayer = new(() => new ChartPlayer(chart,
                note => note.GetNotePlayTime(scope) - notePreShowInterval));
            ChartPlayer.OnNotePlay += (note, _) => noteContainer.AddNote(note.GetNotePlayTime(scope));
        }

        public void FixedTick()
        {
            if (!LevelRoot.Instance.IsStart) return;

            ChartPlayer.PlayUpdate(_musicRhythmManager.CurrentTime);
            noteContainer.CheckRelease(_musicRhythmManager.CurrentTime);
        }

        public NoteContainer NoteContainer => noteContainer;
        public ChartPlayer ChartPlayer => chartPlayer.Value;
        public int Count => noteContainer.Count;
        public SingleNote TryGetNearestNote(float currentTime) => noteContainer.TryGetNearestNote(currentTime);
        public void HitNote(SingleNote note)
        {
            OnPlayerHitOn?.Invoke();
            noteContainer.ReleaseNote(note);
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
