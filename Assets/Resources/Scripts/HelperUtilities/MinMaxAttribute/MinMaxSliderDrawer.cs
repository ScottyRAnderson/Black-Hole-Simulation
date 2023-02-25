using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxAttribute Attribute = (MinMaxAttribute)attribute;
        label.tooltip = Attribute.Min.ToString("F2") + " to " + Attribute.Max.ToString("F2");

        EditorGUI.BeginChangeCheck();
        bool IsVector2Int = property.propertyType == SerializedPropertyType.Vector2Int;

        Vector2 Vector2Value = !IsVector2Int ? property.vector2Value : Vector2.zero;
        Vector2Int Vector2IntValue = IsVector2Int ? property.vector2IntValue : Vector2Int.zero;
        float Min = IsVector2Int ? Vector2IntValue.x : Vector2Value.x;
        float Max = IsVector2Int ? Vector2IntValue.y : Vector2Value.y;

        if (Attribute.HasMinMax)
        {
            Rect[] SplitRect = EditorHelper.SplitRect(EditorGUI.PrefixLabel(position, label), 3);

            int Padding = (int)SplitRect[0].width - 40;
            int Spacing = 1;

            SplitRect[0].width -= Padding + Spacing;

            SplitRect[1].x -= Padding;
            SplitRect[1].width += Padding * 2;

            SplitRect[2].width -= Padding + Spacing;
            SplitRect[2].x += Padding + Spacing;

            Min = EditorGUI.FloatField(SplitRect[0], IsVector2Int ? Min : float.Parse(Min.ToString("F2")));
            Max = EditorGUI.FloatField(SplitRect[2], IsVector2Int ? Max : float.Parse(Max.ToString("F2")));
            EditorGUI.MinMaxSlider(SplitRect[1], ref Min, ref Max, Attribute.Min, Attribute.Max);

            Min = Mathf.Max(Min, Attribute.Min);
            Max = Mathf.Min(Max, Attribute.Max);

            Max = Mathf.Max(Max, Attribute.Min);

            Min = Mathf.Min(Min, Max);
            Max = Mathf.Max(Max, Min);
        }
        else
        {
            Rect[] SplitRect = EditorHelper.SplitRect(EditorGUI.PrefixLabel(position, label), 4);

            int Padding = (int)SplitRect[0].width - 40;
            int Spacing = 5;

            SplitRect[1].width += Padding;
            SplitRect[1].x -= Padding + Spacing;

            SplitRect[3].width += Padding + Spacing;
            SplitRect[3].x -= Padding + Spacing;

            EditorGUI.LabelField(SplitRect[0], "Min");
            EditorGUI.LabelField(SplitRect[2], "Max");
            Min = EditorGUI.FloatField(SplitRect[1], IsVector2Int ? Min : float.Parse(Min.ToString("F2")));
            Max = EditorGUI.FloatField(SplitRect[3], IsVector2Int ? Max : float.Parse(Max.ToString("F2")));

            Min = Mathf.Min(Min, Max);
            Max = Mathf.Max(Max, Min);
        }

        if (EditorGUI.EndChangeCheck())
        {
            if (IsVector2Int){
                property.vector2IntValue = new Vector2Int(Mathf.FloorToInt(Min > Max ? Max : Min), Mathf.FloorToInt(Max));
            }
            else{
                property.vector2Value = new Vector2(Min, Max);// new Vector2(Min > Max ? Max : Min, Max);
            }
        }
    }
}
#endif