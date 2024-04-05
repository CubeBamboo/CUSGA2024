// [WIP]

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shuile.Rhythm
{
    public static class ChartConverter
    {
        public static Chart_MC.Root LoadChart(string chartName)
        {
            Chart_MC.Root root = null;
            var op = Addressables.LoadAssetAsync<TextAsset>($"Assets/Chart/{chartName}");
            var res = op.WaitForCompletion();
            root = JsonUtility.FromJson<Chart_MC.Root>(res.text);

            return root;
        }

        public static string Chart2Text(Chart_MC.Root chart)
        {
            string res = JsonUtility.ToJson(chart);
            return res;
        }
    }

    public class FooChart
    {
        public static void Do()
        {
            ChartConverter.LoadChart("testchart.json");
        }
    }
}
