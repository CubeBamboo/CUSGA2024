namespace Shuile.Rhythm
{
    /* supported chart type: 
     * 1. fixed bpm timing
     * 2. loop in range
     */
    public struct ChartData
    {
        /// <summary> (unit: music bar) for loop </summary>
        public float chartLoopLength;
        /// <summary> (unit: music bar) </summary>
        public float[] chartLoopPart;

        public static ChartData Default = new ChartData
        {
            chartLoopLength = 4f,
            // integer part - beat count, decimal part - where in a beat
            chartLoopPart = new float[] { 0 + 0f / 4f, 1 + 0f / 4f, 2 + 0f / 4f, 3 + 0f / 4f }
        };
    }

    public static class ChartDataUtils
    {
        /// <summary>convert chart time from beat time to absolute time</summary>
        public static float[] BeatTime2AbsoluteTimeChart(ChartData chart, float bpmInterval)
        {
            float[] absoluteTimeChartLoopPart = new float[chart.chartLoopPart.Length];
            for (int i = 0; i < chart.chartLoopPart.Length; i++)
            {
                absoluteTimeChartLoopPart[i] = chart.chartLoopPart[i] * bpmInterval;
            }
            return absoluteTimeChartLoopPart;
        }        
    }
}
