using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackHole/Settings", fileName = "BlackHoleSettings")]
public class BlackHoleSettings : ScriptableObject
{
    [SerializeField]
    private float stepSize = 0.1f;
    [SerializeField]
    private int numSteps = 10;
    [SerializeField]
    private float maxDistortRadius = 100f;
    [SerializeField]
    private float distortFadeOutDistance = 50f;

    public float StepSize { get { return stepSize; } }
    public int NumSteps { get { return numSteps; } }
    public float MaxDistortRadius { get { return maxDistortRadius; } }
    public float DistortFadeOutDistance { get { return distortFadeOutDistance; } }
}