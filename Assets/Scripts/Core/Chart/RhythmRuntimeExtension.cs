using Shuile.Core;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    public static class SingleNoteExtension
    {
        
    }

    public static class BaseNoteDataExtension
    {
        static LevelTimingManager _levelTimingManager;
        static LevelTimingManager levelTimingManager => _levelTimingManager ??= GameplayService.Interface.Get<LevelTimingManager>();

        public static float GetRealTime(this BaseNoteData note)
            => levelTimingManager.GetRealTime(note.rhythmTime);
    }
}
