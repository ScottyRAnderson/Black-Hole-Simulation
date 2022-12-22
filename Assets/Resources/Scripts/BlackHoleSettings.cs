using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackHole/Settings", fileName = "BlackHoleSettings")]
public class BlackHoleSettings : ScriptableObject
{
    [SerializeField]
    private Color eventHorizonColor = Color.black;
    [SerializeField]
    private float stepSize = 0.1f;

    [Space]

    [SerializeField]
    private float maxDistortRadius = 100f;
    [SerializeField]
    private float distortFadeOutDistance = 50f;
    [SerializeField]
    private float fadePower = 1f;
    [SerializeField]
    private bool debugFade;

    public Color EventHorizonColor { get { return eventHorizonColor; } }
    public float StepSize { get { return stepSize; } }
    public float MaxDistortRadius { get { return maxDistortRadius; } }
    public float DistortFadeOutDistance { get { return distortFadeOutDistance; } }
    public float FadePower { get { return fadePower; } }
    public bool DebugFade { get { return debugFade; } }
}