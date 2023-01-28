// Dither rendering logic developed by Joseph Kalathil, modified for personal use
// Attribution (https://gist.github.com/josephbk117/8344b204588f328e50556a45db042e9c)
// See attached Readme & License for details

using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView][CreateAssetMenu (menuName = "PostProcessing/DitherEffect")]
public class DitherEffect : ImageEffect
{
    [SerializeField][Range(0.0f, 1.0f)]
    private float ditherStrength = 0.1f;
    [SerializeField][Range(1, 32)]
    private int colourDepth = 4;

    private Material material;
    private Shader _shader;

    void OnEnable()
    {
        _shader = null;
        var shader = _shader ? _shader : Shader.Find("Hidden/Dither");
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
    }

    void OnDisable(){
        DestroyImmediate(material);
    }

    public override void Render(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_DitherStrength", ditherStrength);
        material.SetInt("_ColourDepth", colourDepth);
        Graphics.Blit(source, destination, material);
    }
}