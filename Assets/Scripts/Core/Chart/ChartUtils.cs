using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Shuile.Rhythm.Runtime;
using UDebug = UnityEngine.Debug;

namespace Shuile.Rhythm
{
    public static class ChartUtils
    {
        /* version last update in 2024.4.6
         * now support:
         * - eventdata from .mc -> .json["column"]
         * - single note (not support long note)
         * - get resource directly from scriptable obejct
         */
        public static ChartData LoadChart(string chartName)
        {
            var op1 = Addressables.LoadAssetAsync<TextAsset>($"Chart/{chartName}");
            var res = op1.WaitForCompletion(); // TODO: async

            ChartData chartData = InternalLoadChartWithoutResourceLoad(res.text, out var songKey);
            var op2 = Addressables.LoadAssetAsync<AudioClip>(songKey);
            chartData.audioClip = op2.WaitForCompletion();
            return chartData;
        }

        public static ChartData LoadChart(AssetReference assetReference)
        {
            var op1 = Addressables.LoadAssetAsync<TextAsset>(assetReference);
            var res = op1.WaitForCompletion(); // TODO: async

            ChartData chartData = InternalLoadChartWithoutResourceLoad(res.text, out var songKey);
            var op2 = Addressables.LoadAssetAsync<AudioClip>(songKey);
            chartData.audioClip = op2.WaitForCompletion(); // TODO: async
            return chartData;
        }

        public static ChartData LoadChart(ChartSO chartSO)
        {
            ChartData chartData = InternalLoadChartWithoutResourceLoad(chartSO.chartFile.text, out var songKey);
            chartData.audioClip = chartSO.clip;
            return chartData;
        }

        private static ChartData InternalLoadChartWithoutResourceLoad(string json, out string songAssetsKey)
        {
            // to NoteData
            JObject jobj = JObject.Parse(json); // the json is based on Chart_MC.Root
            ChartData chartData = new();

            // timing (only record first timing point)
            float bpm = jobj["time"][0]["bpm"].ToObject<float>();
            JToken lastNote = jobj["note"].Last;
            float offset = lastNote["offset"] != null ? lastNote["offset"].ToObject<float>() : 0;
            chartData.time = new ChartData.TimingPoint[1]
            {
                new ChartData.TimingPoint
                {
                    bpm = bpm,
                    offset = offset,
                }
            };

            // song
            string songName = lastNote["sound"].ToObject<string>();
            songAssetsKey = $"Song/{songName}";

            // note
            List<BaseNoteData> tempList = new();
            JToken notes = jobj["note"];
            foreach (JToken note in notes)
            {
                int[] beat = note["beat"].ToObject<int[]>();

                if (note["column"] == null) continue; // TODO: ...
                int column = note["column"].ToObject<int>();

                BaseNoteData noteData;
                if (column != 1 && column != 2) continue;
                noteData = column switch
                {
                    1 => new SpawnSingleEnemyNoteData(),
                    2 => new SpawnLaserNoteData(),
                    _ => throw new System.Exception("invalid column data"),
                };

                noteData.targetTime = beat[0] + (float)beat[1] / beat[2];
                tempList.Add(noteData);
            }
            chartData.note = tempList.ToArray();

            return chartData;
        }

        public static string Chart2Text(Chart_MC.Root chart)
        {
            string res = JsonUtility.ToJson(chart);
            return res;
        }
    }

    /*public class FooChart
    {
        public static void Do()
        {
            ChartUtils.LoadChart("testchart.json");
        }
    }*/
}
