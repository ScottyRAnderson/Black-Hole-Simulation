using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackHole/Settings", fileName = "BlackHoleSettings")]
public class BlackHoleSettings : ScriptableObject
{
    public enum AccretionQuality
    {
        low = 1,
        high = 0
    }

    [SerializeField][ColorUsage(default, true)]
    private Color shadowColor = Color.black;

    [SerializeField]
    private int stepCount = 300;
    [SerializeField]
    private float stepSize = 0.1f;
    [SerializeField]
    private float gravitationalConst = 0.4f;
    [SerializeField][Tooltip("Affects how close rays travel from the event horizon.")]
    private float attenuation = 1.8f;

    [SerializeField]
    private float maxEffectRadius = 5f;
    [SerializeField]
    private float effectFadeOutDist = 10f;
    [SerializeField]
    private float effectFalloff = 1f;
    [SerializeField]
    private bool debugFalloff;

    [SerializeField]
    private float blueShiftPower;

    [SerializeField]
    private bool renderAccretion;
    [SerializeField]
    private AccretionQuality accretionQuality;
    [SerializeField][ColorUsage(default, true)]
    private Color accretionMainColor;
    [SerializeField][ColorUsage(default, true)]
    private Color accretionInnerColor;
    [SerializeField]
    private float accretionColorShift = 80f;
    [SerializeField]
    private float accretionFalloff;
    [SerializeField]
    private float accretionIntensity;
    [SerializeField][MinMax(0f, 1f)]
    private Vector2 accretionRadius;
    [SerializeField]
    private float accretionWidth;
    [SerializeField][Range(0f, 1f)][Tooltip("Controls how sloped the edges of the accretion disc are.")]
    private float accretionSlope;
    [SerializeField]
    private Texture3D accretionNoiseTex;
    [SerializeField]
    private NoiseLayer[] noiseLayers;

    [SerializeField][Range(0f, 1f)][Tooltip("Sample values lower than the threshold will be rejected.")]
    private float gasCloudThreshold;
    [SerializeField][Tooltip("How well light passes through the gas.")]
    private float transmittancePower;
    [SerializeField][Tooltip("How distinct the gas samples are.")]
    private float densityPower;

    public Color ShadowColor { get { return shadowColor; } }

    public int StepCount { get { return stepCount; } }
    public float StepSize { get { return stepSize; } }
    public float GravitationalConst { get { return gravitationalConst; } }
    public float Attenuation { get { return attenuation; } }

    public float MaxEffectRadius { get { return maxEffectRadius; } }
    public float EffectFadeOutDist { get { return effectFadeOutDist; } }
    public float EffectFalloff { get { return effectFalloff; } }
    public bool DebugFalloff { get { return debugFalloff; } }

    public float BlueShiftPower { get { return blueShiftPower; } }

    public bool RenderAccretion { get { return renderAccretion; } }
    public AccretionQuality AccretionQualityLevel { get { return accretionQuality; } }
    public Color AccretionMainColor { get { return accretionMainColor; } }
    public Color AccretionInnerColor { get { return accretionInnerColor; } }
    public float AccretionColorShift { get { return accretionColorShift; } }
    public float AccretionFalloff { get { return accretionFalloff; } }
    public float AccretionIntensity { get { return accretionIntensity; } }
    public float AccretionOuterRadius { get { return accretionRadius.y; } }
    public float AccretionInnerRadius { get { return accretionRadius.x; } }
    public float AccretionWidth { get { return accretionWidth; } }
    public float AccretionSlope { get { return accretionSlope; } }
    public Texture3D AccretionNoiseTex { get { return accretionNoiseTex; } }
    public NoiseLayer[] NoiseLayers { get { return noiseLayers; } }

    public float GasCloudThreshold { get { return gasCloudThreshold; } }
    public float TransmittancePower { get { return transmittancePower; } }
    public float DensityPower { get { return densityPower; } }

    private void OnValidate()
    {
        stepCount = Mathf.Max(stepCount, 0);
        stepSize = Mathf.Max(stepSize, 0f);
        gravitationalConst = Mathf.Max(gravitationalConst, 0f);
        attenuation = Mathf.Max(attenuation, 0f);
        blueShiftPower = Mathf.Max(blueShiftPower, 0f);

        maxEffectRadius = Mathf.Max(maxEffectRadius, 0f);
        effectFadeOutDist = Mathf.Max(effectFadeOutDist, 0f);
        accretionWidth = Mathf.Max(accretionWidth, 0f);
    }
}