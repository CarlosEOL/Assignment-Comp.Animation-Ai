using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SphereHolder : MonoBehaviour
{
    [SerializeField] Interpolation interpolation = Interpolation.LINEAR;
    [SerializeField] Easing easing = Easing.SINE;
    public Transform[] SpherePosition;

    public Vector3 plrPos;

    [Range(0.0f, 1.0f)]
    public float tt;
    
    [SerializeField] AnimationCurve curve;
    
    enum Interpolation
    {
        LINEAR,
        SMOOTH,
        EASE,
        CURVE
    }

    enum Easing
    {
        SINE,
        QUADRATIC,
        CUBIC,
        QUARTIC,
        QUINTIC,
        EXPONENTIAL,
        CIRCLE,
        BACK,
        ELASTIC
    }

    static float EaseSine(float t)
    {
        return -(Mathf.Cos(Mathf.PI * t) - 1.0f) / 2.0f;
    }

    static float EaseQuadratic(float t)
    {
        return t < 0.5f ? 2.0f * t * t : 1 - Mathf.Pow(-2.0f * t + 2.0f, 2.0f) / 2.0f;
    }

    static float EaseCubic(float t)
    {
        return t < 0.5f ? 4.0f * t * t * t : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 3.0f) / 2.0f;
    }

    static float EaseQuartic(float t)
    {
        return t < 0.5f ? 8.0f * t * t * t * t : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 4.0f) / 2.0f;
    }

    static float EaseQuintic(float t)
    {
        return t < 0.5f ? 16.0f * t * t * t * t * t : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 5.0f) / 2.0f;
    }

    static float EaseExponential(float t)
    {
        return t == 0.0f
          ? 0.0f
          : t == 1.0f
          ? 1.0f
          : t < 0.5f ? Mathf.Pow(2.0f, 20.0f * t - 10.0f) / 2.0f
          : (2.0f - Mathf.Pow(2.0f, -20.0f * t + 10.0f)) / 2.0f;
    }

    static float EaseCircle(float t)
    {
        return t < 0.5f
          ? (1.0f - Mathf.Sqrt(1.0f - Mathf.Pow(2.0f * t, 2.0f))) / 2.0f
          : (Mathf.Sqrt(1.0f - Mathf.Pow(-2.0f * t + 2.0f, 2.0f)) + 1.0f) / 2.0f;
    }

    static float EaseBack(float t)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        return t < 0.5
          ? (Mathf.Pow(2.0f * t, 2.0f) * ((c2 + 1.0f) * 2.0f * t - c2)) / 2.0f
          : (Mathf.Pow(2.0f * t - 2.0f, 2.0f) * ((c2 + 1.0f) * (t * 2.0f - 2.0f) + c2) + 2.0f) / 2.0f;
    }

    static float EaseElastic(float t)
    {
        const float c5 = (2.0f * Mathf.PI) / 4.5f;

        return t == 0.0f
          ? 0.0f
          : t == 1.0f
          ? 1.0f
          : t < 0.5f
          ? -(Mathf.Pow(2.0f, 20.0f * t - 10.0f) * Mathf.Sin((20.0f * t - 11.125f) * c5)) / 2.0f
          : (Mathf.Pow(2.0f, -20.0f * t + 10.0f) * Mathf.Sin((20.0f * t - 11.125f) * c5)) / 2.0f + 1.0f;
    }
    
    delegate float EasingMethod(float t);
    EasingMethod[] easings = new EasingMethod[9]
    {
        EaseSine,
        EaseQuadratic,
        EaseCubic,
        EaseQuartic,
        EaseQuintic,
        EaseExponential,
        EaseCircle,
        EaseBack,
        EaseElastic
    };
    
    private float elapsedTime = 0.0f;
    private Vector3 playerPos;
    private int i = 0;
    private float velocity = 2;
    
    protected Vector3 Lerp(Vector3 playerPos, Vector3 SpherePos, float t)
    {
        return (1-t) * playerPos + (t) * SpherePos;
    }

    public void InterpolationMethods(Vector3 plrPos, Vector3 TargetPos, float t)
    {
        switch (interpolation)
        {
            case Interpolation.LINEAR:
            {
                transform.position = Vector3.Lerp(plrPos, TargetPos, t);
            }
                break;

            case Interpolation.SMOOTH:
            {
                transform.position = Vector3.SlerpUnclamped(plrPos, TargetPos, t);
            }
                break;

            case Interpolation.EASE:
            {
                float t1 = easings[(int) easing](t);
                transform.position = Vector3.LerpUnclamped(plrPos, TargetPos, t1);
            }
                break;

            case Interpolation.CURVE:
            {
                float t1 = curve.Evaluate(t);
                transform.position = Vector3.LerpUnclamped(plrPos, TargetPos, t1);
            }
                break;
        }
    }
    
    public void InterpolationForScale(Vector3 plrPos, Vector3 TargetPos, float t)
    {
        switch (interpolation)
        {
            case Interpolation.LINEAR:
            {
                transform.localScale = Vector3.Lerp(plrPos, TargetPos, t);
            }
                break;

            case Interpolation.SMOOTH:
            {
                transform.localScale = Vector3.SlerpUnclamped(plrPos, TargetPos, t);
            }
                break;

            case Interpolation.EASE:
            {
                float t1 = easings[(int) easing](t);
                transform.localScale = Vector3.LerpUnclamped(plrPos, TargetPos, t1);
            }
                break;

            case Interpolation.CURVE:
            {
                float t1 = curve.Evaluate(t);
                transform.localScale = Vector3.LerpUnclamped(plrPos, TargetPos, t1);
            }
                break;
        }
    }
}
