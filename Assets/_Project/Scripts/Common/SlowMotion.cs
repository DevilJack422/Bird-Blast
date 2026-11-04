using System.Collections;
using UnityEngine;

public static class SlowMotion
{
    public static bool Active { get; private set; }

    private static float baseFixedDelta = 0.02f;

    public static IEnumerator Routine(float targerScale = 0.25f, float hold = 1.2f, float easeIn = 0.1f, float easeOut = 0.5f)
    {
        if (!Active) baseFixedDelta = Time.fixedDeltaTime;
        Active = true;

        yield return Ramp(1f, targerScale, easeIn);
        for (float t = 0; t < hold; t += Time.unscaledDeltaTime) yield return null;
        yield return Ramp(targerScale, 1f, easeOut);

        ForceReset();
    }

    public static IEnumerator Ramp(float from, float to, float duration)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            Apply(Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        Apply(to);
    }

    public static void Apply(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = baseFixedDelta * scale;
    }

    public static void ForceReset()
    {
        Active = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = baseFixedDelta;
    }
}
