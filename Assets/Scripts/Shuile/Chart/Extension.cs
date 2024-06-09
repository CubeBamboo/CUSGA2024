using Shuile.Rhythm;

namespace Shuile.Chart
{
    public static class BaseNoteDataExtension
    {
        public static float GetRealTime(this BaseNoteData note, LevelTimingManager levelTimingManager)
            => levelTimingManager.GetRealTime(note.rhythmTime);
    }
}
