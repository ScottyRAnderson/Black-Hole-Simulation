using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer
{
    [SerializeField]
    private bool enabled = true;

    [Space]

    [SerializeField][Tooltip("The distance at which the noise is viewed from.")]
    private float noiseScale = 8f;
    [SerializeField][Range(1, 8)][Tooltip("The number of levels of detail.")]
    private int octaves = 2;
    [SerializeField][Range(0, 1)][Tooltip("How much each octave contributes to the overall shape. [Adjusts amplitude]")]
    private float persistance = 0.4f;
    [SerializeField][Tooltip("How much detail is added or removed at each octave. [Adjusts frequency]")]
    private float lacunarity = 1f;
    [SerializeField][Tooltip("Multiplies the overall height by the specified amount.")]
    private float heightScalar = 1f;
    [SerializeField]
    private float scrollRate;

    public bool Enabled { get { return enabled; } }
    public float NoiseScale { get { return noiseScale; } }
    public int Octaves { get { return octaves; } }
    public float Persistance { get { return persistance; } }
    public float Lacunarity { get { return lacunarity; } }
    public float HeightScalar { get { return heightScalar; } }
    public float ScrollRate { get { return scrollRate; } }

    public void ValidateConfig()
    {
        noiseScale = Mathf.Max(NoiseScale, 0.0001f);
        lacunarity = Mathf.Max(Lacunarity, 1f);
        heightScalar = Mathf.Max(HeightScalar, 0f);
        scrollRate = Mathf.Max(scrollRate, 0f);
    }
}