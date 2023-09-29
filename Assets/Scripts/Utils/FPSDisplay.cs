using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FPSDisplay : MonoBehaviour
{
    public float updateInterval = 0.5f;

    public bool showMedian = false;
    public float medianLearnrate = 0.05f;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private float currentFPS = 0;

    private float median = 0;
    private float average = 0;

    public float CurrentFPS => currentFPS;

    public float FPSMedian => median;

    public float FPSAverage => average;

    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        timeleft = updateInterval;
    }

    private void Update()
    {
        // Timing inside the editor is not accurate. Only use in actual build.

        //#if !UNITY_EDITOR

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update Text and start new interval
        if (timeleft <= 0.0)
        {
            currentFPS = accum / frames;

            average += (Mathf.Abs(currentFPS) - average) * 0.1f;
            median += Mathf.Sign(currentFPS - median) * Mathf.Min(average * medianLearnrate, Mathf.Abs(currentFPS - median));

            // display two fractional digits (f2 format)
            float fps = showMedian ? median : currentFPS;
            text.text = string.Format("{0:F2} FPS ({1:F1} ms)", fps, 1000.0f / fps);

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
        //#endif
    }

    public void ResetMedianAndAverage()
    {
        median = 0;
        average = 0;
    }
}