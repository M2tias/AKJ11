using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;

        bool enabled = AllAreEnabled(condHAtt, property);
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = AllAreEnabled(condHAtt, property);

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }

    }

    private bool AllAreEnabled(ConditionalHideAttribute conditionalHideAttribute, SerializedProperty property) {
        bool allEnabled = true;
        try {
        foreach(ConditionalHideAttributeOptions condOptions in conditionalHideAttribute.OpProperties) {
            if (!GetConditionalHideAttributeResult(condOptions, property)) {
                allEnabled = false;
                break;
            }
        } } catch (Exception e) {
            MonoBehaviour.print("Exception: " + e);
        }
        return allEnabled;
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttributeOptions condHAtt, SerializedProperty propertyA)
    {
        bool enabled = true;


        SerializedProperty sourcePropertyValue = FindSerializableProperty(condHAtt, propertyA);

        if (sourcePropertyValue != null)
        {
            /*if (sourcePropertyValue.isArray) {
                bool allAreTrue = true;
                for (int index = 0; index < sourcePropertyValue.arraySize; index++) {
                    SerializedProperty fieldValue = sourcePropertyValue.GetArrayElementAtIndex(index);
                    if (!IsSingleFieldEnabled(condHAtt, fieldValue)) {
                        allAreTrue = false;
                    }
                }
                enabled = allAreTrue;
            } else {*/
            enabled = IsSingleFieldEnabled(condHAtt, sourcePropertyValue);
            //}

        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.SourceField);
        }

        //if (condHAtt.Inverse) enabled = !enabled;

        return enabled;
    }

    private bool IsSingleFieldEnabled(ConditionalHideAttributeOptions condHAtt, SerializedProperty propertyValue) {
        var fieldValue = GetPropertyValue(propertyValue);
        var comparingValue = condHAtt.CompareValue.ToString();
        var fieldValueString = fieldValue.ToString();

        return comparingValue == fieldValueString;
    }

    private SerializedProperty FindSerializableProperty(ConditionalHideAttributeOptions condHAtt, SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        int idx = propertyPath.LastIndexOf('.');
        if (idx == -1)
        {
            return property.serializedObject.FindProperty(condHAtt.SourceField);
        }
        else
        {
            propertyPath = propertyPath.Substring(0, idx);
            return property.serializedObject.FindProperty(propertyPath).FindPropertyRelative(condHAtt.SourceField);
        }
    }

    public static object GetPropertyValue(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                return prop.intValue;
            case SerializedPropertyType.Boolean:
                return prop.boolValue;
            case SerializedPropertyType.Float:
                return prop.floatValue;
            case SerializedPropertyType.String:
                return prop.stringValue;
            case SerializedPropertyType.Color:
                return prop.colorValue;
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return (LayerMask)prop.intValue;
            case SerializedPropertyType.Enum:
                if (prop.enumNames.Length == 0) { return "undefined"; }
                return prop.enumNames[prop.enumValueIndex];
            case SerializedPropertyType.Vector2:
                return prop.vector2Value;
            case SerializedPropertyType.Vector3:
                return prop.vector3Value;
            case SerializedPropertyType.Vector4:
                return prop.vector4Value;
            case SerializedPropertyType.Rect:
                return prop.rectValue;
            case SerializedPropertyType.ArraySize:
                return prop.arraySize;
            case SerializedPropertyType.Character:
                return (char)prop.intValue;
            case SerializedPropertyType.AnimationCurve:
                return prop.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return prop.boundsValue;
            case SerializedPropertyType.Gradient:
                return null;
                //throw new System.InvalidOperationException("Can not handle Gradient types.");
        }

        return null;
    }
}