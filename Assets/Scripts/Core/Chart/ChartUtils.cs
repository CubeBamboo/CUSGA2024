using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile.Rhythm
{
    public static class ChartUtils
    {
        /* version last update in 2024.4.6
         * now support:
         * - eventdata from .mc -> .json["column"]
         * - single note (not support long note)
         */
        // TODO: return ChartData, MusicReference, Offset, Bpm(timing)
        public static ChartData LoadChart(string chartName)
        {
            var op = Addressables.LoadAssetAsync<TextAsset>($"Chart/{chartName}");
            var res = op.WaitForCompletion();

            return InternalLoadChart(res.text);
        }

        public static ChartData LoadChart(AssetReference assetReference)
        {
            var op = Addressables.LoadAssetAsync<TextAsset>(assetReference);
            var res = op.WaitForCompletion();

            return InternalLoadChart(res.text);
        }

        private static ChartData InternalLoadChart(string json)
        {
            // to NoteData
            JObject jobj = JObject.Parse(json); // the json is based on Chart_MC.Root
            ChartData chartData = new();

            List<NoteData> tempList = new();
            JToken notes = jobj["note"];
            foreach (JToken note in notes)
            {
                int[] beat = note["beat"].ToObject<int[]>();

                if (note["column"] == null) continue; // TODO: ...
                int column = note["column"].ToObject<int>();
                NoteData noteData = new();
                noteData.targetTime = beat[0] + (float)beat[1] / beat[2];

                if (column != 1 && column != 2) continue;
                noteData.eventType = column switch
                {
                    1 => NoteEventType.SingleEnemySpawn,
                    2 => NoteEventType.LaserSpawn,
                    _ => throw new System.Exception("invalid column data"),
                };
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
