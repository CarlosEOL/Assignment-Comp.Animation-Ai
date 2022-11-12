using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
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

    [SerializeField] GameObject target;
    [SerializeField] const float fov = 60.0f;
    [SerializeField] const float length = 10.0f;
    [SerializeField] const float rotationAmount = 90.0f;
    [SerializeField] const float rotationDuration = 3.0f;
    [SerializeField] Interpolation interpolation = Interpolation.LINEAR;
    [SerializeField] Easing easing = Easing.SINE;
    [SerializeField] AnimationCurve curve;

    private float rotSrc, rotDst;
    private float elapsedTime = 0.0f;
    private bool right = true;

    void Start()
    {
        rotSrc = transform.rotation.eulerAngles.y;
        rotDst = rotSrc + rotationAmount;
    }

    void Update()
    {
        float y = 0.0f;
        float t = right ? elapsedTime / rotationDuration : 1.0f - elapsedTime / rotationDuration;
        switch (interpolation)
        {
            case Interpolation.LINEAR:
                {
                    y = Mathf.Lerp(rotSrc, rotDst, t);
                }
                break;

            case Interpolation.SMOOTH:
                {
                    y = Mathf.SmoothStep(rotSrc, rotDst, t);
                }
                break;

            case Interpolation.EASE:
                {
                    float t1 = easings[(int)easing](t);
                    y = Mathf.LerpUnclamped(rotSrc, rotDst, t1);
                }
                break;

            case Interpolation.CURVE:
                {
                    float t1 = curve.Evaluate(t);
                    y = Mathf.LerpUnclamped(rotSrc, rotDst, t1);
                }
                break;
        }

        Vector3 euler = transform.eulerAngles;
        euler.y = y;
        transform.eulerAngles = euler;
        Utility.InRangeDebug(transform, target.transform, length, fov);

        elapsedTime += Time.smoothDeltaTime;
        if (elapsedTime > rotationDuration)
        {
            elapsedTime = 0.0f;
            right = !right;
        }
    }
}
