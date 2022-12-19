using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class EditorHelper
{
    public static readonly float WideButtonWidth = EditorGUIUtility.singleLineHeight * 2f;
    public static readonly float NarrowButtonWidth = EditorGUIUtility.singleLineHeight * 1.2f;
    public static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
    public static readonly float ElementSpace = 5f;
    public static readonly Color32 DetailFoldoutBackerCol = new Color32(41, 45, 62, 255); //new Color32(37, 34, 102, 255);
    public static readonly Color32 GroupBoxCol = new Color32(64, 64, 64, 255);

    public static Rect Header(string text, float offset = 0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
        rect.x += offset;
        rect.width -= offset;
        EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), new Color(1f, 1f, 1f, 0.1f));
        rect.x += (EditorGUIUtility.singleLineHeight / 3) - offset;
        EditorGUI.LabelField(rect, text, EditorStyles.boldLabel);
        return rect;
    }

    public static bool Toggle(string variableName, bool toggled)
    {
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(variableName, GUILayout.Width(180));
            GUILayout.FlexibleSpace();
            toggled = EditorGUILayout.Toggle(toggled, GUILayout.Width(15 * (EditorGUI.indentLevel + 1)));
        }
        return toggled;
    }

    public static bool Foldout(bool foldState, string content, float offset = 0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
        rect.x += offset;
        rect.width -= offset;

        rect.x += 10;
        EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), new Color(1f, 1f, 1f, 0.1f));
        rect.x -= 10;

        foldState = EditorGUI.Foldout(rect, foldState, "", true);
        rect.x += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);
        return foldState;
    }

    public static bool Foldout(bool foldState, string content, float labelOffset, float offset = 0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
        rect.x += offset;
        rect.width -= offset;

        rect.xMax += labelOffset;
        rect.x += 10;
        EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), new Color(1f, 1f, 1f, 0.1f));
        rect.x -= 10;
        rect.xMax -= labelOffset;

        foldState = EditorGUI.Foldout(rect, foldState, "", true);
        rect.x += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);
        return foldState;
    }

    public static bool Foldout(bool foldState, GUIContent content, float offset = 0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
        rect.x += offset;
        rect.width -= offset;

        rect.width -= 10;
        rect.x += 10;
        EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), new Color(1f, 1f, 1f, 0.1f));
        rect.x -= 10;
        rect.width += 10;

        foldState = EditorGUI.Foldout(rect, foldState, "", true);
        rect.x += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);
        return foldState;
    }

    public static bool Foldout(bool foldState, GUIContent content, GUIStyle guiStyle, float offset = 0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
        rect.x += offset;
        rect.width -= offset;

        rect.width -= 10;
        rect.x += 10;
        EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), new Color(1f, 1f, 1f, 0.1f));
        rect.x -= 10;
        rect.width += 10;

        foldState = EditorGUI.Foldout(rect, foldState, "", true);
        rect.x += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(rect, content, guiStyle);
        return foldState;
    }

    public static bool Foldout(bool foldState, string content, GUIStyle guiStyle, float offset = 0f)
    {
        Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);
        rect.x += offset;
        rect.width -= offset;

        rect.width -= 10;
        rect.x += 10;
        EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), new Color(1f, 1f, 1f, 0.1f));
        rect.x -= 10;
        rect.width += 10;

        foldState = EditorGUI.Foldout(rect, foldState, "", true);
        rect.x += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(rect, content, guiStyle);
        return foldState;
    }

    public static Vector2 MinMaxSlider(GUIContent label, Vector2 minMax)
    {
        float minWander = minMax.x;
        float maxWander = minMax.y;

        Rect[] splitRect = EditorHelper.SplitRect(EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(), label), 3);

        int padding = (int)splitRect[0].width - 40;
        int spacing = 5;

        splitRect[0].width -= padding + spacing;

        splitRect[1].x -= padding;
        splitRect[1].width += padding * 2;

        splitRect[2].width -= padding + spacing;
        splitRect[2].x += padding + spacing;

        minWander = EditorGUI.FloatField(splitRect[0], float.Parse(minWander.ToString("F2")));
        maxWander = EditorGUI.FloatField(splitRect[2], float.Parse(maxWander.ToString("F2")));
        EditorGUI.MinMaxSlider(splitRect[1], ref minWander, ref maxWander, 0f, 10f);

        minWander = Mathf.Max(minWander, 0f);
        maxWander = Mathf.Min(maxWander, 10f);
        maxWander = Mathf.Max(maxWander, 0f);
        minWander = Mathf.Min(minWander, maxWander);
        maxWander = Mathf.Max(maxWander, minWander);

        minMax = new Vector2(minWander, maxWander);
        return minMax;
    }

    public static GUIStyle GetColoredStyle(Color color)
    {
        GUIStyle style = new GUIStyle();
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        style.normal.background = texture;
        return style;
    }

    public static GUIStyle GetColoredStyle(GUIStyle styleTag, Color color)
    {
        GUIStyle style = new GUIStyle(styleTag);
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        style.normal.background = texture;
        return style;
    }

    public static GUIStyle SetColoredStyle(GUIStyle style, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        style.normal.background = texture;
        return style;
    }

    public static void SetColoredStyle(ref GUIStyle style, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        style.normal.background = texture;
    }

    public static bool GUIButtonTextured(Rect rect, Texture2D texture)
    {
        bool btn = GUI.Button(rect, "");
        GUI.DrawTexture(rect, texture);
        return btn;
    }

    public static bool GUIButtonTextured(Rect rect, Texture2D texture, float texScale)
    {
        bool btn = GUI.Button(rect, "");
        rect.size = new Vector2(texScale, texScale);
        GUI.DrawTexture(rect, texture);
        return btn;
    }

    public static bool GUIButtonTextured(Rect rect, Texture2D texture, float texWidth, float texHeight)
    {
        bool btn = GUI.Button(rect, "");
        rect.size = new Vector2(texWidth, texHeight);
        GUI.DrawTexture(rect, texture);
        return btn;
    }

    public static bool GUILayoutButtonIndented(string text, float indent)
    {
        using (var scope = new GUILayout.HorizontalScope())
        {
            GUILayout.Space(indent);
            if (GUILayout.Button(text)){
                return true;
            }
        }
        return false;
    }

    /// <summary> Evently splits a rect transform into a specified number of evenly spaced segments. </summary>
    public static Rect[] SplitRect(Rect rect, int segments)
    {
        Rect[] rects = new Rect[segments];
        for (int i = 0; i < segments; i++){
            rects[i] = new Rect(rect.position.x + (i * rect.width / segments), rect.position.y, rect.width / segments, rect.height);
        }
        return rects;
    }
}
#endif