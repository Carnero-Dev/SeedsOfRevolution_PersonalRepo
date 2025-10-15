using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(DynamicRangeAttribute))]
public class DynamicRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DynamicRangeAttribute rangeAttribute = (DynamicRangeAttribute)attribute;
        SerializedObject serializedObject = property.serializedObject;
        SerializedProperty maxVariable = serializedObject.FindProperty(rangeAttribute.maxVariableName);
        int minValue = rangeAttribute.minValue;

        float maxValue = maxVariable != null ? maxVariable.intValue : 0; //! Valor por defecto en caso de error //! Valor por defecto en caso de error

        property.floatValue = EditorGUI.Slider(position, label, property.floatValue, minValue, maxValue);
    }    
}
