using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeEntity : MonoBehaviour
{
    [SerializeField]
    private float stepSize = 0.1f;
    [SerializeField]
    private int numSteps = 10;

    public float StepSize { get { return stepSize; } }
    public int NumSteps { get { return numSteps; } }
}