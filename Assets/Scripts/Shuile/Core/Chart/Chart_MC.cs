// [WIP]

using System.Collections.Generic;

namespace Shuile.Rhythm
{
    #region Chart_MC
    public class Chart_MC
    {
        [System.Serializable]
        public class Root
        {
            /// <summary>
            /// record the chart's information (song name, mode, length...)
            /// </summary>
            public Meta meta;
            /// <summary>
            /// timing settings
            /// </summary>
            public List<Time> time;
            /// <summary>
            /// notes
            /// </summary>
            public List<object> note;
            /// <summary>
            /// other (user settings)
            /// </summary>
            public Extra extra;
        }

        [System.Serializable]
        public class Meta
        {
            /// <summary>
            /// default 0
            /// </summary>
            public int ver;
            /// <summary>
            /// chart name
            /// </summary>
            public string version;
            /// <summary>
            /// id in database (default 0)
            /// </summary>
            public int id;
            /// <summary>
            /// chart length
            /// </summary>
            public int time;
            /// <summary>
            /// song data
            /// </summary>
            public Song song;
            /// <summary>
            /// about chart mode (column and start bar)
            /// </summary>
            public Mode_ext mode_ext;
        }

        [System.Serializable]
        /// <summary>
        /// single timing
        /// </summary>
        public class Time
        {
            /// <summary>
            /// [a, b, c] to represent beat in bar: (a + b / c), which means A bar's B/C progress
            /// </summary>
            public List<int> beat;
            /// <summary>
            /// bpm data in this note
            /// </summary>
            public int bpm;
        }

        [System.Serializable]
        /// <summary>
        /// single note
        /// </summary>
        public class SingleNote
        {
            /// <summary>
            /// time data, [a, b, c] to represent beat in bar: (a + b / c), which means A bar's B/C progress
            /// </summary>
            public List<int> beat;
            /// <summary>
            /// column data, we may use a switch-case to convert it to other data type
            /// </summary>
            public int column;
        }

        [System.Serializable]
        /// <summary>
        /// long note
        /// </summary>
        public class LongNote
        {
            public List<int> beat;
            public List<int> endBeat;
            public int column;
        }

        [System.Serializable]
        public class Song
        {
            public string title;
            public string artist;
            /// <summary>
            /// id in database (default 0)
            /// </summary>
            public int id;
        }

        [System.Serializable]
        public class Mode_ext
        {
            /// <summary>
            /// column count
            /// </summary>
            public int column;
            /// <summary>
            /// default 0
            /// </summary>
            public int start_bar;
        }

        [System.Serializable]
        public class Extra
        {
            //public Test test;
        }

        // nouse
        /*public class Test
        {
            /// <summary>
            /// 
            /// </summary>
            public int divide;
            /// <summary>
            /// 
            /// </summary>
            public int speed;
            /// <summary>
            /// 
            /// </summary>
            public int save;
            /// <summary>
            /// 
            /// </summary>
            public int @lock;
            /// <summary>
            /// 
            /// </summary>
            public int edit_mode;
        }*/
    }

    #endregion
}
