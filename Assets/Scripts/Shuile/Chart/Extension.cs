using Shuile.Core.Framework.Unity;
using Shuile.Rhythm;
using Shuile.Rhythm.Runtime;

namespace Shuile.Chart
{
    public static class BaseNoteDataExtension
    {
        public static void ProcessNote(this BaseNoteData note, NoteDataProcessor noteDataProcessor) => noteDataProcessor.ProcessNote(note);
        public static void ProcessNote(this BaseNoteData note, IGetableScope scope) => scope.GetImplementation<NoteDataProcessor>().ProcessNote(note);
        public static float GetNotePlayTime(this BaseNoteData note, NoteDataProcessor noteDataProcessor) => noteDataProcessor.GetNotePlayTime(note);
        public static float GetNotePlayTime(this BaseNoteData note, IGetableScope scope) => scope.GetImplementation<NoteDataProcessor>().GetNotePlayTime(note);
    }
}
