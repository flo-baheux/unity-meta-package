
using UnityEngine;
using UnityEditor;

namespace MetaPackage
{
  [CustomPropertyDrawer(typeof(CustomLabelAttribute))]
  public class CustomLabelDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var customAttribute = (CustomLabelAttribute)attribute;
      EditorGUI.PropertyField(position, property, new GUIContent(customAttribute.customLabel), true);
    }
  }
}