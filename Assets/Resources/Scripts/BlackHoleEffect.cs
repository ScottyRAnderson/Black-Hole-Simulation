using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PostProcessing/BlackHoleEffect", fileName = "BlackHoleEffect")]
public class BlackHoleEffect : ImageEffect
{
    [SerializeField]
    private BlackHoleSettings settings;

    private List<Singularity> instances;

    public override void Render(RenderTexture source, RenderTexture destination)
    {
        if(settings == null){
            settings = (BlackHoleSettings)CreateInstance("BlackHoleSettings");
        }
        if (instances == null || instances.Count == 0 || Application.isEditor){
            instances = new List<Singularity>(FindObjectsOfType<Singularity>());
        }

        List<Material> materials = BuildMaterials();
        EffectRenderer.RenderMaterials(source, destination, materials);
    }

    protected override List<Material> BuildMaterials()
    {
        base.BuildMaterials();
        materials.Clear();

        for (int i = 0; i < instances.Count; i++)
        {
            Singularity instance = instances[i];
            Material material = new Material(effectShader);

            // Update material to match settings
            material.SetColor("_ShadowColor", settings.ShadowColor);

            material.SetInt("_StepCount", settings.StepCount);
            material.SetFloat("_StepSize", settings.StepSize);
            material.SetFloat("_GravitationalConst", settings.GravitationalConst);
            material.SetFloat("_Attenuation", settings.Attenuation);

            material.SetFloat("_MaxEffectRadius", settings.MaxEffectRadius);
            material.SetFloat("_EffectFadeOutDist", settings.EffectFadeOutDist);
            material.SetFloat("_EffectFalloff", settings.EffectFalloff);
            if (settings.DebugFalloff){
                material.EnableKeyword("DEBUGFALLOFF");
            }
            else{
                material.DisableKeyword("DEBUGFALLOFF");
            }

            material.SetFloat("_BlueShiftPower", settings.BlueShiftPower);

            material.SetInt("_AccretionQuality", !settings.RenderAccretion ? -1 : (int)settings.AccretionQualityLevel);
            material.SetColor("_AccretionMainColor", settings.AccretionMainColor);
            material.SetColor("_AccretionInnerColor", settings.AccretionInnerColor);
            material.SetFloat("_AccretionColorShift", settings.AccretionColorShift);
            material.SetFloat("_AccretionFalloff", settings.AccretionFalloff);
            material.SetFloat("_AccretionIntensity", settings.AccretionIntensity);
            material.SetFloat("_AccretionOuterRadius", settings.MaxEffectRadius * settings.AccretionOuterRadius);
            material.SetFloat("_AccretionInnerRadius", settings.MaxEffectRadius * settings.AccretionInnerRadius);
            material.SetFloat("_AccretionWidth", settings.AccretionWidth);
            material.SetFloat("_AccretionSlope", settings.AccretionSlope);
            material.SetVector("_AccretionDir", instance.transform.up);
            material.SetTexture("_AccretionNoiseTex", settings.AccretionNoiseTex);

            int noiseLayerCount = 0;
            float[] sampleScales = new float[4];
            float[] scrollRates = new float[4];
            for (int j = 0; j < settings.NoiseLayers.Length; j++)
            {
                NoiseLayer noiseLayer = settings.NoiseLayers[j];
                if(!noiseLayer.Enabled){
                    continue;
                }

                sampleScales[noiseLayerCount] = noiseLayer.SampleScale;
                scrollRates[noiseLayerCount] = noiseLayer.ScrollRate;
                noiseLayerCount++;
            }

            material.SetFloat("_NoiseLayerCount", noiseLayerCount);
            material.SetFloatArray("_SampleScales", sampleScales);
            material.SetFloatArray("_ScrollRates", scrollRates);

            material.SetFloat("_GasCloudThreshold", settings.GasCloudThreshold);
            material.SetFloat("_TransmittancePower", settings.TransmittancePower);
            material.SetFloat("_DensityPower", settings.DensityPower);

            material.SetVector("_Position", instance.transform.position);
            material.SetFloat("_SchwarzschildRadius", instance.SchwarzschildRadius);

            materials.Add(material);
        }
        return materials;
    }
}