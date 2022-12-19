using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : MonoBehaviour
{
    public const float speedOfLight = 299792458f;
    public const float gravitationalConst = 0.000000000066743f;

    [SerializeField]
    private float schwarzschildRadius = 20f;
    [SerializeField]
    private bool debugSchwarzschild;
    [SerializeField]
    private bool drawDummyEventHorizon;
    [SerializeField]
    private Color debugCol = Color.white;

    public float SchwarzschildRadius { get { return schwarzschildRadius; } }
    public float Mass { get { return (schwarzschildRadius * Mathf.Pow(speedOfLight, 2f)) / (gravitationalConst * 2f); } }

    //public float SchwarzschildRadius { get { return (2 * gravitationalConst * mass) / Mathf.Pow(speedOfLight, 2f); } } // Equation Source: https://en.wikipedia.org/wiki/Schwarzschild_radius

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = debugCol;
        if (debugSchwarzschild){
            Gizmos.DrawWireSphere(transform.position, SchwarzschildRadius);
        }
    }

    public void SetDummyEventHorizon(bool active)
    {
        Transform dummyEH = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "dummyEH" + gameObject.GetHashCode())
            {
                dummyEH = child;
                break;
            }
        }

        if (active && dummyEH == null)
        {
            dummyEH = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            dummyEH.SetParent(transform);
            dummyEH.name = "dummyEH" + gameObject.GetHashCode();
            dummyEH.localScale = Vector3.one * SchwarzschildRadius * 2f;
            dummyEH.gameObject.hideFlags = HideFlags.HideInHierarchy;
            
            MeshRenderer mr = dummyEH.GetComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(Shader.Find("Standard"));
            mr.sharedMaterial.SetColor("_Color", Color.black);
        }
        else if(dummyEH != null){
            DestroyImmediate(dummyEH.gameObject);
        }
    }
}