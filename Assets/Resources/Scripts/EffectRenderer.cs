using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode][ImageEffectAllowedInSceneView]
public class EffectRenderer : MonoBehaviour
{
    [SerializeField]
    private ImageEffect[] effects;

    private static Material defaultMat;

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture initialSource, RenderTexture finalDestination)
    {
        if (defaultMat == null){
            defaultMat = new Material(Shader.Find("Unlit/Texture"));
        }
        if (Camera.current.depthTextureMode != DepthTextureMode.Depth){
            Camera.current.depthTextureMode = DepthTextureMode.Depth;
        }

        // If no effects are given, blit the default mat
        if (effects == null || effects.Length == 0)
        {
            Graphics.Blit(intialSource, finalDestination, defaultMat);
            return;
        }

        List<RenderTexture> temporaryTextures = new List<RenderTexture>();
        RenderTexture currentSource = intialSource;
        RenderTexture currentDestination = null;

        for (int i = 0; i < effects.Length; i++)
        {
            ImageEffect effect = effects[i];
            if(!effect.RenderEffect){
                continue;
            }

            if (i == effects.Length - 1)
            {
                // If final effect, render into final destination
                currentDestination = finalDestination;
            }
            else
            {
                // ...Otherwise, create a temporary texture
                currentDestination = TemporaryRenderTexture(finalDestination);
                temporaryTextures.Add(currentDestination); // Record temporary texture to be released later
            }

            effect.Render(currentSource, currentDestination);
            currentSource = currentDestination;
        }
        
        // If a given material is null, blit the default mat
        if(currentDestination != finalDestination){
            Graphics.Blit(currentSource, finalDestination, defaultMat);
        }

        // Release temporary textures
        for (int i = 0; i < temporaryTextures.Count; i++){
            RenderTexture.ReleaseTemporary(temporaryTextures[i]);
        }
    }

    public static void RenderMaterials(RenderTexture source, RenderTexture destination, List<Material> materials)
    {
        List<RenderTexture> temporaryTextures = new List<RenderTexture>();
        RenderTexture currentSource = source;
        RenderTexture currentDestination = null;

        for (int i = 0; i < materials.Count; i++)
        {
            Material material = materials[i];
            if (i == materials.Count - 1)
            {
                // If final effect, render into final destination
                currentDestination = destination;
            }
            else
            {
                // ...Otherwise, create a temporary texture
                currentDestination = TemporaryRenderTexture(destination);
                temporaryTextures.Add(currentDestination); // Record temporary texture to be released later
            }

            Graphics.Blit(currentSource, currentDestination, material);
            currentSource = currentDestination;
        }

        // If a given material is null, blit the default mat
        if (currentDestination != destination || materials.Count == 0){
            Graphics.Blit(currentSource, destination, defaultMat);
        }

        // Release temporary textures
        for (int i = 0; i < temporaryTextures.Count; i++){
            RenderTexture.ReleaseTemporary(temporaryTextures[i]);
        }
    }

    public static RenderTexture TemporaryRenderTexture(RenderTexture texture){
        return RenderTexture.GetTemporary(texture.descriptor);
    }
}