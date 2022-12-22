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
}