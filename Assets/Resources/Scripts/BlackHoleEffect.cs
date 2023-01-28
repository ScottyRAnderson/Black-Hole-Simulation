using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
            material.SetInt("_StepCount", settings.StepCount);
            material.SetFloat("_StepSize", settings.StepSize);
            material.SetFloat("_GravitationalConst", settings.GravitationalConst);

            material.SetFloat("_MaxEffectRadius", settings.MaxEffectRadius);
            material.SetFloat("_EffectFadeOutDist", settings.EffectFadeOutDist);
            material.SetFloat("_EffectFalloff", settings.EffectFalloff);
            if (settings.DebugFalloff){
                material.EnableKeyword("DEBUGFALLOFF");
            }
            else{
                material.DisableKeyword("DEBUGFALLOFF");
            }

            material.SetColor("_EventHorizonColor", settings.EventHorizonColor);
            material.SetColor("_AccretionColor", settings.AccretionColor);
            material.SetFloat("_AccretionFalloff", settings.AccretionFalloff);
            material.SetFloat("_AccretionIntensity", settings.AccretionIntensity);
            material.SetFloat("_AccretionOuterRadius", settings.MaxEffectRadius * settings.AccretionOuterRadius);
            material.SetFloat("_AccretionInnerRadius", settings.MaxEffectRadius * settings.AccretionInnerRadius);
            material.SetFloat("_AccretionWidth", settings.AccretionWidth);
            material.SetVector("_AccretionDir", instance.transform.up);

            material.SetFloat("_NoiseScale", settings.AccretionNoise.NoiseScale);
            material.SetInt("_Octaves", settings.AccretionNoise.Octaves);
            material.SetFloat("_Persistance", settings.AccretionNoise.Persistance);
            material.SetFloat("_Lacunarity", settings.AccretionNoise.Lacunarity);
            material.SetFloat("_HeightScalar", settings.AccretionNoise.HeightScalar);
            material.SetFloat("_ScrollRate", settings.AccretionNoise.ScrollRate);
            material.SetTexture("_AccretionTex", settings.AccretionTex);

            material.SetVector("_Position", instance.transform.position);
            material.SetFloat("_SchwarzschildRadius", instance.SchwarzschildRadius);

            material.SetVector("_TestVar", settings.TestVar);

            materials.Add(material);
        }
        return materials;
    }
}