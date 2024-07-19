using UnityEngine;

public class AudioClipUtility : MonoBehaviour
{
    public static Texture2D BakeAudioWaveform(AudioClip _clip, int resolution = 60, int width = 1920, int height = 200)
    {
        resolution = _clip.frequency / resolution;

        var samples = new float[_clip.samples * _clip.channels];
        _clip.GetData(samples, 0);

        var waveForm = new float[samples.Length / resolution];

        float min = 0;
        float max = 0;
        var inited = false;

        for (var i = 0; i < waveForm.Length; i++)
        {
            waveForm[i] = 0;

            for (var j = 0; j < resolution; j++)
            {
                waveForm[i] += Mathf.Abs(samples[(i * resolution) + j]);
            }

            if (!inited)
            {
                min = waveForm[i];
                max = waveForm[i];
                inited = true;
            }
            else
            {
                if (waveForm[i] < min)
                {
                    min = waveForm[i];
                }

                if (waveForm[i] > max)
                {
                    max = waveForm[i];
                }
            }
            //waveForm[i] /= resolution;
        }


        var backgroundColor = Color.black;
        var waveformColor = Color.green;
        var blank = new Color[width * height];
        var texture = new Texture2D(width, height);

        // init background color
        for (var i = 0; i < blank.Length; ++i)
        {
            blank[i] = backgroundColor;
        }

        texture.SetPixels(blank, 0);

        // draw waveform
        var xScale = width / (float)waveForm.Length;

        var tMid = (int)(height / 2.0f);
        float yScale = 1;

        if (max > tMid)
        {
            yScale = tMid / max;
        }

        for (var i = 0; i < waveForm.Length; ++i)
        {
            var x = (int)(i * xScale);
            var yOffset = (int)(waveForm[i] * yScale);
            var startY = tMid - yOffset;
            var endY = tMid + yOffset;

            for (var y = startY; y <= endY; ++y)
            {
                texture.SetPixel(x, y, waveformColor);
            }
        }

        texture.Apply();
        return texture;
    }
}
