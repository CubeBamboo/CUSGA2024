namespace Shuile.Rhythm.Runtime
{
    public static class SingleNoteExtension
    {
        
    }

    public static class BaseNoteDataExtension
    {
        public static float GetRealTime(this BaseNoteData note)
            => MusicRhythmManager.Instance.GetRealTime(note.rhythmTime);
    }
}
