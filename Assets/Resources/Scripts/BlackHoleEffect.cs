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
            material.SetVector("_Position", instance.transform.position);
            material.SetFloat("_Mass", instance.Mass);

            Vector2 screenPos = Camera.current.WorldToScreenPoint(instance.transform.position);
            screenPos.x /= Camera.current.pixelWidth;
            screenPos.y /= Camera.current.pixelHeight;
            material.SetVector("_ScreenPos", screenPos);

            materials.Add(material);
        }

        return materials;
    }
}