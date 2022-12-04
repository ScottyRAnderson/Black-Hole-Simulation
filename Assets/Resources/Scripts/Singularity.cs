using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : MonoBehaviour
{
    [SerializeField]
    private float mass = 1f;
    [SerializeField]
    private float gravitationalConstant = 1f;

    public float Mass { get { return mass; } }
    public float GravitationalConstant { get { return gravitationalConstant; } }
}