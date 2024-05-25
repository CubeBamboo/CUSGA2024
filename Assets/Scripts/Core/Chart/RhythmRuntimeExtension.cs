using Shuile.Core;
using Shuile.Gameplay;

namespace Shuile.Rhythm.Runtime
{
    public static class SingleNoteExtension
    {

    }

    public static class BaseNoteDataExtension
    {
        public static float GetRealTime(this BaseNoteData note, LevelTimingManager levelTimingManager)
            => levelTimingManager.GetRealTime(note.rhythmTime);
    }
}
