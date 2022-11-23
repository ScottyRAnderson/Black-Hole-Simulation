using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BlackHole/Settings", fileName = "BlackHoleSettings")]
public class BlackHoleSettings : ScriptableObject
{
    [SerializeField]
    private bool invertColors;

    public bool InvertColors { get { return invertColors; } }
}