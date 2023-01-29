using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer
{
    [SerializeField]
    private bool enabled = true;
    [SerializeField]
    private float sampleScale = 2f;
    [SerializeField]
    private float scrollRate;

    public bool Enabled { get { return enabled; } }
    public float SampleScale { get { return sampleScale; } }
    public float ScrollRate { get { return scrollRate; } }
}