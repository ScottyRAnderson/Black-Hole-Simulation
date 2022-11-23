using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PostProcessing/ImageEffect", fileName = "ImageEffect")]
public class ImageEffect : ScriptableObject
{
    [SerializeField]
    protected bool renderEffect = true;
    [SerializeField]
    protected Shader effectShader;

    protected Material material;

    public virtual bool RenderEffect { get { return renderEffect; } }
    public virtual Shader EffectShader { get { return effectShader; } }

    public virtual void Render(RenderTexture source, RenderTexture destination) { }
    protected virtual Material BuildMaterial()
    {
        if(material == null || material.shader != effectShader){
            material = new Material(effectShader);
        }
        return material;
    }
}