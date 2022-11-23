using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PostProcessing/BlackHoleEffect", fileName = "BlackHoleEffect")]
public class BlackHoleEffect : ImageEffect
{
    [SerializeField]
    private BlackHoleSettings settings;

    public override void Render(RenderTexture source, RenderTexture destination)
    {
        if(settings == null){
            settings = (BlackHoleSettings)CreateInstance("BlackHoleSettings");
        }

        Material mat = BuildMaterial();
        EffectRenderer.RenderMaterial(source, destination, mat);
    }

    protected override Material BuildMaterial()
    {
        base.BuildMaterial();

        // Update material to match settings
        if(settings.InvertColors){
            material.EnableKeyword("INVERTCOL");
        }
        else{
            material.DisableKeyword("INVERTCOL");
        }

        return material;
    }
}