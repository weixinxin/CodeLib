using System;
using System.Collections.Generic;
using UnityEngine;

public enum EaseMethod
{
    ExpoEaseOut,
    ExpoEaseIn,
    ExpoEaseInOut,
    ExpoEaseOutIn,
    CircEaseOut,
    CircEaseIn,
    CircEaseInOut,
    CircEaseOutIn,
    QuadEaseOut,
    QuadEaseIn,
    QuadEaseInOut,
    QuadEaseOutIn,
    SineEaseOut,
    SineEaseIn,
    SineEaseInOut,
    SineEaseOutIn,
    CubicEaseOut,
    CubicEaseIn,
    CubicEaseInOut,
    CubicEaseOutIn,
    QuartEaseOut,
    QuartEaseIn,
    QuartEaseInOut,
    QuartEaseOutIn,
    QuintEaseOut,
    QuintEaseIn,
    QuintEaseInOut,
    QuintEaseOutIn,
    ElasticEaseOut,
    ElasticEaseIn,
    ElasticEaseInOut,
    ElasticEaseOutIn,
    BounceEaseOut,
    BounceEaseIn,
    BounceEaseInOut,
    BounceEaseOutIn,
    BackEaseOut,
    BackEaseIn,
    BackEaseInOut,
    BackEaseOutIn,
}
/*
public class EaseTest:MonoBehaviour
{
    public EaseMethod method;

    float Ease(float t)
    {
        switch (method)
        {
            case EaseMethod.ExpoEaseOut:
                return EasingFunction.ExpoEaseOut(t);
            case EaseMethod.ExpoEaseIn:
                return EasingFunction.ExpoEaseIn(t);
            case EaseMethod.ExpoEaseInOut:
                return EasingFunction.ExpoEaseInOut(t);
            case EaseMethod.ExpoEaseOutIn:
                return EasingFunction.ExpoEaseOutIn(t);
            case EaseMethod.CircEaseOut:
                return EasingFunction.CircEaseOut(t);
            case EaseMethod.CircEaseIn:
                return EasingFunction.CircEaseIn(t);
            case EaseMethod.CircEaseInOut:
                return EasingFunction.CircEaseInOut(t);
            case EaseMethod.CircEaseOutIn:
                return EasingFunction.CircEaseOutIn(t);
            case EaseMethod.QuadEaseOut:
                return EasingFunction.QuadEaseOut(t);
            case EaseMethod.QuadEaseIn:
                return EasingFunction.QuadEaseIn(t);
            case EaseMethod.QuadEaseInOut:
                return EasingFunction.QuadEaseInOut(t);
            case EaseMethod.QuadEaseOutIn:
                return EasingFunction.QuadEaseOutIn(t);
            case EaseMethod.SineEaseOut:
                return EasingFunction.SineEaseOut(t);
            case EaseMethod.SineEaseIn:
                return EasingFunction.SineEaseIn(t);
            case EaseMethod.SineEaseInOut:
                return EasingFunction.SineEaseInOut(t);
            case EaseMethod.SineEaseOutIn:
                return EasingFunction.SineEaseOutIn(t);
            case EaseMethod.CubicEaseOut:
                return EasingFunction.CubicEaseOut(t);
            case EaseMethod.CubicEaseIn:
                return EasingFunction.CubicEaseIn(t);
            case EaseMethod.CubicEaseInOut:
                return EasingFunction.CubicEaseInOut(t);
            case EaseMethod.CubicEaseOutIn:
                return EasingFunction.CubicEaseOutIn(t);
            case EaseMethod.QuartEaseOut:
                return EasingFunction.QuartEaseOut(t);
            case EaseMethod.QuartEaseIn:
                return EasingFunction.QuartEaseIn(t);
            case EaseMethod.QuartEaseInOut:
                return EasingFunction.QuartEaseInOut(t);
            case EaseMethod.QuartEaseOutIn:
                return EasingFunction.QuartEaseOutIn(t);
            case EaseMethod.QuintEaseOut:
                return EasingFunction.QuintEaseOut(t);
            case EaseMethod.QuintEaseIn:
                return EasingFunction.QuintEaseIn(t);
            case EaseMethod.QuintEaseInOut:
                return EasingFunction.QuintEaseInOut(t);
            case EaseMethod.QuintEaseOutIn:
                return EasingFunction.QuintEaseOutIn(t);
            case EaseMethod.ElasticEaseOut:
                return EasingFunction.ElasticEaseOut(t);
            case EaseMethod.ElasticEaseIn:
                return EasingFunction.ElasticEaseIn(t);
            case EaseMethod.ElasticEaseInOut:
                return EasingFunction.ElasticEaseInOut(t);
            case EaseMethod.ElasticEaseOutIn:
                return EasingFunction.ElasticEaseOutIn(t);
            case EaseMethod.BounceEaseOut:
                return EasingFunction.BounceEaseOut(t);
            case EaseMethod.BounceEaseIn:
                return EasingFunction.BounceEaseIn(t);
            case EaseMethod.BounceEaseInOut:
                return EasingFunction.BounceEaseInOut(t);
            case EaseMethod.BounceEaseOutIn:
                return EasingFunction.BounceEaseOutIn(t);
            case EaseMethod.BackEaseOut:
                return EasingFunction.BackEaseOut(t);
            case EaseMethod.BackEaseIn:
                return EasingFunction.BackEaseIn(t);
            case EaseMethod.BackEaseInOut:
                return EasingFunction.BackEaseInOut(t);
            case EaseMethod.BackEaseOutIn:
                return EasingFunction.BackEaseOutIn(t);

                

        }
        return t;
    }
    private void OnDrawGizmos()
    {
        Vector3 last = Vector3.zero;
        for(int i = 1;i < 1000;i++)
        {

            Vector3 p = new Vector3(i * 0.01f,0, Ease(i * 0.001f) * 10f);
            Gizmos.DrawLine(last, p);
            last = p;
        }
    }


}
*/