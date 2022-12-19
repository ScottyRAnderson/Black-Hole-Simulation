using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PostProcessing/ShapeEffect", fileName = "ShapeEffect")]
public class ShapeEffect : ImageEffect
{
    private List<ShapeEntity> instances;

    public override void Render(RenderTexture source, RenderTexture destination)
    {
        if (instances == null || instances.Count == 0 || Application.isEditor){
            instances = new List<ShapeEntity>(FindObjectsOfType<ShapeEntity>());
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
            ShapeEntity instance = instances[i];
            Material material = new Material(effectShader);

            // Update material to match settings
            material.SetFloat("_StepSize", instance.StepSize);
            material.SetInt("_NumSteps", instance.NumSteps);
            material.SetVector("_Position", instance.transform.position);

            materials.Add(material);
        }

        return materials;
    }
}