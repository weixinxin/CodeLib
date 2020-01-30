using System;
using UnityEngine;

public static class EasingFunction
{
    public static float ExpoEaseOut(float t)
    {
        return -Mathf.Pow(2, -10 * t) + 1;
    }
    public static float ExpoEaseIn(float t)
    {
        return Mathf.Pow(2, 10 * (t - 1));
    }
    public static float ExpoEaseInOut(float t)
    {
        if ((t = 2 * t - 1) < 0)
            return 0.5f * Mathf.Pow(2, 10 * t);
        return 0.5f * (-Mathf.Pow(2, -10 * t) + 2);
    }

    public static float ExpoEaseOutIn(float t)
    {
        if (t < 0.5f)
            return -0.5f * Mathf.Pow(2, -20 * t) + 0.5f;
        return 0.5f * Mathf.Pow(2, 10 * (2 * t - 2)) + 0.5f;
    }
    public static float CircEaseOut(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        return Mathf.Sqrt(1 - (--t) * t);
    }

    public static float CircEaseIn(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        return -(Mathf.Sqrt(1 - t * t) - 1);
    }

    public static float CircEaseInOut(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        if ((t *= 2) < 1)
            return -0.5f * (Mathf.Sqrt(1 - t * t) - 1);

        return 0.5f * (Mathf.Sqrt(1 - (t -= 2) * t) + 1);
    }

    public static float CircEaseOutIn(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        if ((t = 2 * t - 1) < 0)
            return 0.5f * Mathf.Sqrt(1 - t * t);

        return -0.5f * (Mathf.Sqrt(1 - t * t) - 1) + 0.5f;
    }
    public static float QuadEaseOut(float t)
    {
        return -t * (t - 2);
    }
    public static float QuadEaseIn(float t)
    {
        return t * t;
    }
    public static float QuadEaseInOut(float t)
    {
        if ((t *= 2) < 1)
            return 0.5f * t * t;

        return -0.5f * ((t - 1) * (t - 3) - 1);
    }
    public static float QuadEaseOutIn(float t)
    {
        if (t < 0.5f)
            return -t * (2 * t - 2);
        return 0.5f * (t = t * 2 - 1) * t + 0.5f;
    }
    public static float SineEaseOut(float t)
    {
        return Mathf.Sin(0.5f * t * Mathf.PI);
    }
    public static float SineEaseIn(float t)
    {
        return -Mathf.Cos(0.5f * t * Mathf.PI) + 1;
    }

    public static float SineEaseInOut(float t)
    {
        if ((t *= 2) < 1)
            return 0.5f * Mathf.Sin(0.5f * t * Mathf.PI);

        return -0.5f * (Mathf.Cos(0.5f * (t - 1) * Mathf.PI) - 2);
    }
    public static float SineEaseOutIn(float t)
    {
        if (t < 0.5f)
            return 0.5f * Mathf.Sin(t * Mathf.PI);

        return -0.5f * Mathf.Cos((t - 0.5f) * Mathf.PI) + 1;
    }
    public static float CubicEaseOut(float t)
    {
        return Mathf.Pow(t- 1, 3) + 1;
    }
    public static float CubicEaseIn(float t)
    {
        return Mathf.Pow(t , 3);
    }
    public static float CubicEaseInOut(float t)
    {
        if ((t *= 2) < 1)
            return 0.5f * t * t * t;

        return 0.5f * ((t -= 2) * t * t + 2);
    }
    public static float CubicEaseOutIn(float t)
    {
        if ((t = t * 2 - 1) < 0)
            return 0.5f * (Mathf.Pow(t, 3) + 1);

        return 0.5f * Mathf.Pow(t, 3) + 0.5f;
    }
    public static float QuartEaseOut(float t)
    {
        return -Mathf.Pow(t - 1, 4) + 1;
    }
    public static float QuartEaseIn(float t)
    {
        return Mathf.Pow(t, 4);
    }
    public static float QuartEaseInOut(float t)
    {
        if ((t *= 2) < 1)
            return 0.5f * Mathf.Pow(t, 4);

        return -0.5f * Mathf.Pow(t - 2, 4) + 1;
    }
    public static float QuartEaseOutIn(float t)
    {
        if (t < 0.5f)
            return -0.5f * Mathf.Pow(2 * t - 1, 4) + 0.5f;

        return 0.5f * Mathf.Pow(2 * t - 1, 4) + 0.5f;
    }
    public static float QuintEaseOut(float t)
    {
        return Mathf.Pow(t - 1, 5) + 1;
    }
    public static float QuintEaseIn(float t)
    {
        return Mathf.Pow(t, 5);
    }
    public static float QuintEaseInOut(float t)
    {
        if (t < 0.5f)
            return 0.5f * Mathf.Pow(2 * t, 5);
        return 0.5f * Mathf.Pow(2 * t - 2, 5) + 1;
    }
    public static float QuintEaseOutIn(float t)
    {
        if (t < 0.5f)
            return 0.5f * Mathf.Pow(2 * t - 1, 5) + 0.5f;
        return 0.5f * Mathf.Pow(2 * t - 1, 5) + 0.5f;
    }

    public static float ElasticEaseOut(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        return (Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) * 3.33333333f) + 1);
    }
    public static float ElasticEaseIn(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;

        return -(Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) * 3.33333333f));
    }
    public static float ElasticEaseInOut(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        t *= 2;

        if (t < 1)
            return -0.5f * (Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t - 0.1125f) * (2 * Mathf.PI) * 2.22222222f));
        return Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t - 0.1125f) * (2 * Mathf.PI) * 2.22222222f) * 0.5f + 1;
    }
    public static float ElasticEaseOutIn(float t)
    {
        if (t <= 0)
            return 0;
        if (t >= 1)
            return 1;
        t *= 2;
        if (t < 1)
            return 0.5f * (Mathf.Pow(2, -10 * t) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) * 3.33333333f) + 1);
        return -0.5f * (Mathf.Pow(2, 10 * (t -= 2)) * Mathf.Sin((t - 0.075f) * (2 * Mathf.PI) * 3.33333333f)) + 0.5f;
    }
    public static float BounceEaseOut(float t)
    {
        if (t < 0.36363636f)
            return (7.5625f * t * t);
        else if (t < 0.72727273f)
            return (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f);
        else if (t < 0.90909091f)
            return (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f);
        else
            return (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
    }
    public static float BounceEaseIn(float t)
    {
        t = 1 - t;
        if (t < 0.36363636f)
            return 1 - (7.5625f * t * t);
        else if (t < 0.72727273f)
            return 1 - (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f);
        else if (t < 0.90909091f)
            return 1 - (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f);
        else
            return 1 - (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
    }
    public static float BounceEaseInOut(float t)
    {
        if (t < 0.5f)
            return BounceEaseIn(t * 2) * 0.5f;
        else
            return BounceEaseOut(t * 2 - 1) * 0.5f + 0.5f;
    }
    public static float BounceEaseOutIn(float t)
    {
        if (t < 0.5f)
            return BounceEaseOut(t * 2) * 0.5f;
        else
            return BounceEaseIn(t * 2 - 1) * 0.5f + 0.5f;
    }

    public static float BackEaseOut(float t)
    {
        return --t * t * ((1.70158f + 1) * t + 1.70158f) + 1;
    }

    public static float BackEaseIn(float t)
    {
        return t * t * ((1.70158f + 1) * t - 1.70158f);
    }
    public static float BackEaseInOut(float t)
    {
        if ((t *= 2) < 1)
            return 0.5f * (t * t * ((1.70158f + 1) * t - 1.70158f));
        else
            return 0.5f * ((t-=2) * t * ((1.70158f + 1) * t + 1.70158f)) + 1;
    }
    public static float BackEaseOutIn(float t)
    {
        if ((t = 2 * t - 1) < 0)
            return 0.5f * t * t * ((1.70158f + 1) * t + 1.70158f) + 0.5f;
        else
            return 0.5f * t * t * ((1.70158f + 1) * t - 1.70158f) + 0.5f;
    }

}
