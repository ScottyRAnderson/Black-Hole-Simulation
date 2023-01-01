using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Singularity))][CanEditMultipleObjects]
public class Singularity_Inspector : Editor
{
    private static bool displayDebugData = true;

    private Singularity singularityBase;

    private SerializedProperty schwarzschildRadius;
    private SerializedProperty debugSchwarzschild;
    private SerializedProperty drawDummyEventHorizon;
    private SerializedProperty debugCol;

    private void OnEnable()
    {
        singularityBase = target as Singularity;

        schwarzschildRadius = serializedObject.FindProperty("schwarzschildRadius");
        debugSchwarzschild = serializedObject.FindProperty("debugSchwarzschild");
        drawDummyEventHorizon = serializedObject.FindProperty("drawDummyEventHorizon");
        debugCol = serializedObject.FindProperty("debugCol");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawOverview();
        GUILayout.Space(5f);
        DrawDebugData();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOverview()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorHelper.Header("Overview");
            using (new EditorGUI.DisabledScope(true)){
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);
            }
            EditorGUILayout.PropertyField(schwarzschildRadius);
        }
    }

    private void DrawDebugData()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorGUI.indentLevel++;
            displayDebugData = EditorHelper.Foldout(displayDebugData, "Debug");
            if (displayDebugData)
            {
                EditorGUILayout.PropertyField(debugCol);
                EditorGUILayout.PropertyField(debugSchwarzschild);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(drawDummyEventHorizon);
                    if (check.changed){
                        singularityBase.SetDummyEventHorizon(drawDummyEventHorizon.boolValue);
                    }
                }
                using (new EditorGUI.DisabledGroupScope(true)){
                    EditorGUILayout.FloatField("Mass", singularityBase.Mass);
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}