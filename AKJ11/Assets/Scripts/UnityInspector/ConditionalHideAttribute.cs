using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[AttributeUsage(
    AttributeTargets.Field |
    AttributeTargets.Property |
    AttributeTargets.Class |
    AttributeTargets.Struct
)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public List<ConditionalHideAttributeOptions> OpProperties {get; private set;}
    public ConditionalHideAttribute(string sourceField, object compareObject)
    {
        OpProperties = new List<ConditionalHideAttributeOptions>() {
            new ConditionalHideAttributeOptions(sourceField, compareObject)
        };
    }

    public ConditionalHideAttribute(string sourceField, bool compareValue = true)
    {
        OpProperties = new List<ConditionalHideAttributeOptions>() {
            new ConditionalHideAttributeOptions(sourceField, compareValue)
        };
    }

    public ConditionalHideAttribute(params object[] options)
    {
        OpProperties = new List<ConditionalHideAttributeOptions>();
        for (int index = 1; index < options.Length; index += 2)
        {
            string source = options[index - 1].ToString();
            object compareObject = options[index];
            OpProperties.Add(new ConditionalHideAttributeOptions(source, compareObject));
        }
    }
}

[System.Serializable]
public class ConditionalHideAttributeOptions
{
    public string SourceField;
    public object CompareValue = true;

    public ConditionalHideAttributeOptions(string sourceField, object compareObject)
    {
        this.SourceField = sourceField;
        this.CompareValue = compareObject == null ? true : compareObject;
    }

    public ConditionalHideAttributeOptions(string sourceField, bool compareValue = true)
    {
        this.SourceField = sourceField;
        this.CompareValue = compareValue;
    }
}