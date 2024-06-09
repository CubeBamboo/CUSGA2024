using Newtonsoft.Json.Linq;
using Shuile.Core.Gameplay.Data;
using Shuile.Rhythm.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile.Chart
{
    public static class ChartUtils
    {
        /* version last update in 2024.4.6
         * now support:
         * - eventdata from .mc -> .json["column"]
         * - single note (not support long note)
         * - get resource directly from scriptable obejct
         * - only can get music length from ChartSO
         */
        public static ChartData LoadChartSync(string chartName)
        {
            var op1 = Addressables.LoadAssetAsync<TextAsset>($"Chart/{chartName}");
            var res = op1.WaitForCompletion();

            ChartData chartData = InternalLoadChartWithoutResourceLoad(res.text, out var songKey);
            var op2 = Addressables.LoadAssetAsync<AudioClip>(songKey);
            chartData.audioClip = op2.WaitForCompletion();
            return chartData;
        }

        public static ChartData LoadChartSync(AssetReference assetReference)
        {
            var op1 = Addressables.LoadAssetAsync<TextAsset>(assetReference);
            var res = op1.WaitForCompletion();

            ChartData chartData = InternalLoadChartWithoutResourceLoad(res.text, out var songKey);
            var op2 = Addressables.LoadAssetAsync<AudioClip>(songKey);
            chartData.audioClip = op2.WaitForCompletion();
            return chartData;
        }

        public static ChartData LoadChartSync(ChartSO chartSO)
        {
            ChartData chartData = InternalLoadChartWithoutResourceLoad(chartSO.chartFile.text, out var songKey);
            chartData.audioClip = chartSO.clip;
            chartData.musicLength = chartSO.musicLength;
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
                if (column != 0) continue;
                noteData = column switch
                {
                    //1 => new SpawnSingleEnemyNoteData(),
                    0 => new SpawnLaserNoteData(),
                    _ => throw new System.Exception("invalid column data"),
                };

                noteData.rhythmTime = beat[0] + (float)beat[1] / beat[2];
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
