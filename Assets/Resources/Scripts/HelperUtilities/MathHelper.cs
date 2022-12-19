using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public const float miles2kilometres = 1.60934f;
    public const float kilometres2miles = 0.621371f;
    public const float astronomicalUnit2kilometre = 1.496e+8f;
    public const float kilometre2astronomicalUnit = 6.6846e-9f;

    public static float WrapAngle(float angle)
    {
        angle %= 360f;
        if (angle <= -180f){
            angle += 360f;
        }
        else if (angle > 180f){
            angle -= 360f;
        }
        return angle;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360){
            angle += 360;
        }
        if (angle > 360){
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary> Normalizes a value between 0 - 1 given the minimum and maximum of the given value. E.g. Normalize(0, -1, 1) = 0.5 </summary>
    public static float NormalizeValue(float value, float min, float max){
        return (value - min) / (max - min);
    }

    // Ellipse defintion reference (https://en.wikipedia.org/wiki/Ellipse#Parameters)
    public static Vector3[] SampleEllipse(float semiMajor, float semiMinor, Vector3 center, int resolution = 1000, float theta = 0f)
    {
        Vector3[] points = new Vector3[resolution + 1];
        Quaternion rot = Quaternion.AngleAxis(theta, Vector3.up);
        for (int i = 0; i <= resolution; i++)
        {
            /// 0<=t<=2π
            /// (i / Resoution) gives us the percent along the ellipse. * 2π gives us the desired sample angle
            float angle = ((float)i / (float)resolution) * Mathf.PI * 2f;

            float x = semiMajor * Mathf.Cos(angle);
            float y = semiMinor * Mathf.Sin(angle);
            points[i] = rot * new Vector3(x, 0f, y) + center;
        }
        return points;
    }
}