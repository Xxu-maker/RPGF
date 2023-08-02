using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 测FPS
/// </summary>
public class FpsCounter : MonoBehaviour
{
    [SerializeField] Text text;
    const long FPS_SAMPLE_PERIOD = 500;
    string displayFormat    = "{0} FPS\n{1} ms";
    int framesSinceLast;
    Stopwatch stopwatch;
    void Start()
    {
        stopwatch = Stopwatch.StartNew();
        displayFormat = text.text;
    }
    void Update()
    {
        framesSinceLast++;
        var elapsedMs = stopwatch.ElapsedMilliseconds;
        if (elapsedMs < FPS_SAMPLE_PERIOD) { return; }

        float elapsedSec = elapsedMs * 0.001f;//float elapsedSec = elapsedMs / 1000f;

        float fps         = framesSinceLast / elapsedSec;
        float frameTimeMs = elapsedMs / (float)framesSinceLast;
        text.text         = string.Format(displayFormat, fps, frameTimeMs);
        framesSinceLast = 0;
        stopwatch.Restart();
    }
}