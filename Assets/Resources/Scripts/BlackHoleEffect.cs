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
            material.SetColor("_EventHorizonColor", settings.EventHorizonColor);
            material.SetFloat("_StepSize", settings.StepSize);

            material.SetFloat("_MaxDistortRadius", settings.MaxDistortRadius);
            material.SetFloat("_DistortFadeOutDistance", settings.DistortFadeOutDistance);
            material.SetFloat("_FadePower", settings.FadePower);
            if(settings.DebugFade){
                material.EnableKeyword("DEBUGFADE");
            }
            else{
                material.DisableKeyword("DEBUGFADE");
            }

            material.SetVector("_Position", instance.transform.position);
            material.SetFloat("_SchwarzschildRadius", instance.SchwarzschildRadius);

            materials.Add(material);
        }

        return materials;
    }
}