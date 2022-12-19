using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    public float Min;
    public float Max;
    public bool HasMinMax;

    public MinMaxAttribute() { }
    public MinMaxAttribute(float Min, float Max)
    {
        this.Min = Min;
        this.Max = Max;
        HasMinMax = true;
    }
}