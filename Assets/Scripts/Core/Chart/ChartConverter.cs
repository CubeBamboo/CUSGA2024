//[WIP]

using System.Collections.Generic;

namespace Shuile.Rhythm
{
    public class ChartConverter
    {

    }

    #region Chart_MC

    public class Song
    {
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string artist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
    }

    public class Mode_ext
    {
        /// <summary>
        /// 
        /// </summary>
        public int column { get; set; }
    }

    public class Meta
    {
        /// <summary>
        /// 
        /// </summary>
        public int ver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string background { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int mode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Song song { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Mode_ext mode_ext { get; set; }
    }

    public class TimeItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<int> beat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int bpm { get; set; }
    }

    public class NoteItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<int> beat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int column { get; set; }
    }

    public class Test
    {
        /// <summary>
        /// 
        /// </summary>
        public int divide { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int speed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int save { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int @lock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int edit_mode { get; set; }
    }

    public class Extra
    {
        /// <summary>
        /// user settings
        /// </summary>
        public Test test { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// record the chart's information (song name, mode, length...)
        /// </summary>
        public Meta meta { get; set; }
        /// <summary>
        /// timing settings
        /// </summary>
        public List<TimeItem> time { get; set; }
        /// <summary>
        /// notes
        /// </summary>
        public List<NoteItem> note { get; set; }
        /// <summary>
        /// other (user settings)
        /// </summary>
        public Extra extra { get; set; }
    }

    #endregion
}
