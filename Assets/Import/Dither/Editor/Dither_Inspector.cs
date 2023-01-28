using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DitherEffect))][CanEditMultipleObjects]
public class Dither_Inspector : Editor
{
    private SerializedProperty renderEffect;
    private SerializedProperty ditherStrength;
    private SerializedProperty colourDepth;

    private void OnEnable()
    {
        renderEffect = serializedObject.FindProperty("renderEffect");
        ditherStrength = serializedObject.FindProperty("ditherStrength");
        colourDepth = serializedObject.FindProperty("colourDepth");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(renderEffect);
        EditorGUILayout.PropertyField(ditherStrength);
        EditorGUILayout.PropertyField(colourDepth);
        serializedObject.ApplyModifiedProperties();
    }
}