using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackHole/Settings", fileName = "BlackHoleSettings")]
public class BlackHoleSettings : ScriptableObject
{
    [SerializeField]
    private int stepCount = 300;
    [SerializeField]
    private float stepSize = 0.1f;
    [SerializeField]
    private float gravitationalConst = 0.4f;

    [Space]

    [SerializeField]
    private float maxEffectRadius = 5f;
    [SerializeField]
    private float effectFadeOutDist = 10f;
    [SerializeField]
    private float effectFalloff = 1f;
    [SerializeField]
    private bool debugFalloff;

    [Space]

    [SerializeField][ColorUsage(default, true)]
    private Color eventHorizonColor = Color.black;
    [SerializeField][ColorUsage(default, true)]
    private Color accretionColor;
    [SerializeField]
    private float accretionFalloff;
    [SerializeField]
    private float accretionIntensity;
    [SerializeField][MinMax(0f, 1f)]
    private Vector2 accretionRadius;
    [SerializeField]
    private float accretionWidth;
    [SerializeField]
    private NoiseLayer accretionNoise;
    [SerializeField]
    private Texture2D accretionTex;

    [Space]

    [SerializeField]
    private Vector4 testVar;

    public int StepCount { get { return stepCount; } }
    public float StepSize { get { return stepSize; } }
    public float GravitationalConst { get { return gravitationalConst; } }

    public float MaxEffectRadius { get { return maxEffectRadius; } }
    public float EffectFadeOutDist { get { return effectFadeOutDist; } }
    public float EffectFalloff { get { return effectFalloff; } }
    public bool DebugFalloff { get { return debugFalloff; } }

    public Color EventHorizonColor { get { return eventHorizonColor; } }
    public Color AccretionColor { get { return accretionColor; } }
    public float AccretionFalloff { get { return accretionFalloff; } }
    public float AccretionIntensity { get { return accretionIntensity; } }
    public float AccretionOuterRadius { get { return accretionRadius.y; } }
    public float AccretionInnerRadius { get { return accretionRadius.x; } }
    public float AccretionWidth { get { return accretionWidth; } }
    public NoiseLayer AccretionNoise { get { return accretionNoise; } }
    public Texture AccretionTex { get { return accretionTex; } }

    public Vector4 TestVar { get { return testVar; } }

    private void OnValidate()
    {
        stepCount = Mathf.Max(stepCount, 0);
        stepSize = Mathf.Max(stepSize, 0f);
        gravitationalConst = Mathf.Max(gravitationalConst, 0f);
        maxEffectRadius = Mathf.Max(maxEffectRadius, 0f);
        effectFadeOutDist = Mathf.Max(effectFadeOutDist, 0f);
        accretionWidth = Mathf.Max(accretionWidth, 0f);
        accretionNoise.ValidateConfig();
    }
}