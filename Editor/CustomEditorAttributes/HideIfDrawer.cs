
using UnityEngine;
using UnityEditor;

namespace MetaPackage
{
  // Note from @Author: This was inspired by different sources on Unity's forum and opensource packages.
  // Can be widely improved, and was made only to satisfy MetaPackage's package requirements.

  [CustomPropertyDrawer(typeof(HideIfAttribute))]
  public class HideIfDrawer : PropertyDrawer
  {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      HideIfAttribute attribute = (HideIfAttribute)this.attribute;
      string conditionPath = property.propertyPath.Replace(property.name, attribute.conditionField);
      SerializedProperty conditionField = property.serializedObject.FindProperty(conditionPath);

      bool hideField = FieldComparator.IsEquivalent(conditionField, attribute.compareWith);

      if (!hideField)
        EditorGUI.PropertyField(position, property, true);
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      HideIfAttribute attribute = (HideIfAttribute)this.attribute;
      string conditionPath = property.propertyPath.Replace(property.name, attribute.conditionField);
      SerializedProperty conditionField = property.serializedObject.FindProperty(conditionPath);

      bool hideField = FieldComparator.IsEquivalent(conditionField, attribute.compareWith);

      return hideField ? 0 : EditorGUIUtility.singleLineHeight;
    }
  }
}