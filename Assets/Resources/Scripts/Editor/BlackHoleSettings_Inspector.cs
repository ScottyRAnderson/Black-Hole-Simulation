using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlackHoleSettings))][CanEditMultipleObjects]
public class BlackHoleSettings_Inspector : Editor
{
    private static bool displayRenderingData = true;
    private static bool displayAccretionData = true;
    private static bool displayVolumetricData = true;

    private SerializedProperty shadowColor;

    private SerializedProperty stepCount;
    private SerializedProperty stepSize;
    private SerializedProperty gravitationalConst;
    private SerializedProperty attenuation;

    private SerializedProperty maxEffectRadius;
    private SerializedProperty effectFadeOutDist;
    private SerializedProperty effectFalloff;
    private SerializedProperty debugFalloff;

    private SerializedProperty renderAccretion;
    private SerializedProperty accretionQuality;
    private SerializedProperty accretionMainColor;
    private SerializedProperty accretionInnerColor;
    private SerializedProperty accretionColorShift;
    private SerializedProperty accretionFalloff;
    private SerializedProperty accretionIntensity;
    private SerializedProperty accretionRadius;
    private SerializedProperty accretionWidth;
    private SerializedProperty accretionSlope;
    private SerializedProperty accretionNoiseTex;
    private SerializedProperty noiseLayers;

    private SerializedProperty gasCloudThreshold;
    private SerializedProperty transmittancePower;
    private SerializedProperty densityPower;

    private void OnEnable()
    {
        shadowColor = serializedObject.FindProperty("shadowColor");

        stepCount = serializedObject.FindProperty("stepCount");
        stepSize = serializedObject.FindProperty("stepSize");
        gravitationalConst = serializedObject.FindProperty("gravitationalConst");
        attenuation = serializedObject.FindProperty("attenuation");

        maxEffectRadius = serializedObject.FindProperty("maxEffectRadius");
        effectFadeOutDist = serializedObject.FindProperty("effectFadeOutDist");
        effectFalloff = serializedObject.FindProperty("effectFalloff");
        debugFalloff = serializedObject.FindProperty("debugFalloff");

        renderAccretion = serializedObject.FindProperty("renderAccretion");
        accretionQuality = serializedObject.FindProperty("accretionQuality");
        accretionMainColor = serializedObject.FindProperty("accretionMainColor");
        accretionInnerColor = serializedObject.FindProperty("accretionInnerColor");
        accretionColorShift = serializedObject.FindProperty("accretionColorShift");
        accretionFalloff = serializedObject.FindProperty("accretionFalloff");
        accretionIntensity = serializedObject.FindProperty("accretionIntensity");
        accretionRadius = serializedObject.FindProperty("accretionRadius");
        accretionWidth = serializedObject.FindProperty("accretionWidth");
        accretionSlope = serializedObject.FindProperty("accretionSlope");
        accretionNoiseTex = serializedObject.FindProperty("accretionNoiseTex");
        noiseLayers = serializedObject.FindProperty("noiseLayers");

        gasCloudThreshold = serializedObject.FindProperty("gasCloudThreshold");
        transmittancePower = serializedObject.FindProperty("transmittancePower");
        densityPower = serializedObject.FindProperty("densityPower");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawRenderingData();
        GUILayout.Space(5f);
        DrawAccretionData();
        GUILayout.Space(5f);
        DrawVolumetricData();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRenderingData()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorGUI.indentLevel++;
            displayRenderingData = EditorHelper.Foldout(displayRenderingData, "Rendering");
            if (displayRenderingData)
            {
                EditorGUILayout.PropertyField(shadowColor);

                GUILayout.Space(5f);

                EditorGUILayout.PropertyField(stepCount);
                EditorGUILayout.PropertyField(stepSize);
                EditorGUILayout.PropertyField(gravitationalConst);
                EditorGUILayout.PropertyField(attenuation);

                GUILayout.Space(5f);

                EditorGUILayout.PropertyField(maxEffectRadius);
                EditorGUILayout.PropertyField(effectFadeOutDist);
                EditorGUILayout.PropertyField(effectFalloff);
                EditorGUILayout.PropertyField(debugFalloff);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void DrawAccretionData()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorGUI.indentLevel++;
            displayAccretionData = EditorHelper.Foldout(displayAccretionData, "Accretion Disc");
            if (displayAccretionData)
            {
                EditorGUILayout.PropertyField(renderAccretion);
                EditorGUILayout.PropertyField(accretionQuality);
                EditorGUILayout.PropertyField(accretionMainColor);
                EditorGUILayout.PropertyField(accretionInnerColor);

                GUILayout.Space(5f);

                EditorGUILayout.PropertyField(accretionColorShift);
                EditorGUILayout.PropertyField(accretionFalloff);
                EditorGUILayout.PropertyField(accretionIntensity);
                EditorGUILayout.PropertyField(accretionRadius);
                EditorGUILayout.PropertyField(accretionWidth);
                EditorGUILayout.PropertyField(accretionSlope);

                GUILayout.Space(5f);

                EditorGUILayout.PropertyField(accretionNoiseTex);
                EditorGUILayout.PropertyField(noiseLayers);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void DrawVolumetricData()
    {
        using (new GUILayout.VerticalScope(EditorHelper.GetColoredStyle(EditorHelper.GroupBoxCol)))
        {
            EditorGUI.indentLevel++;
            displayVolumetricData = EditorHelper.Foldout(displayVolumetricData, "Volumetrics");
            if (displayVolumetricData)
            {
                EditorGUILayout.PropertyField(gasCloudThreshold);
                EditorGUILayout.PropertyField(transmittancePower);
                EditorGUILayout.PropertyField(densityPower);
            }
            EditorGUI.indentLevel--;
        }
    }
}