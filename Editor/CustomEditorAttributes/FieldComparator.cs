
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MetaPackage
{
  public static class FieldComparator
  {
    public static bool IsEquivalent(SerializedProperty field, object toCompareWith)
    {
      if (field == null || toCompareWith == null)
        return false;

      switch (field.propertyType)
      {
        case SerializedPropertyType.Boolean:
          bool compareWithBool = (bool)toCompareWith;
          return field.boolValue == compareWithBool;
        case SerializedPropertyType.Enum:
          object[] toCompareWithEnums = toCompareWith is object[] v ? v : new[] { toCompareWith };
          return toCompareWithEnums.Any(x => field.enumValueIndex.Equals((int)x));
        default:
          Debug.LogError($"[ShowIfDrawer] - Unsupported type ({field.propertyType}).");
          return false;
      }
    }
  }
}