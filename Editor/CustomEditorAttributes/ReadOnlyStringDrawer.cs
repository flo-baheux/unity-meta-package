
using UnityEngine;
using UnityEditor;

namespace MetaPackage
{
  [CustomPropertyDrawer(typeof(ReadOnlyStringAttribute))]
  public class ReadOnlyStringDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      EditorGUI.LabelField(position, label.text, property.stringValue);
      EditorGUI.EndProperty();
    }
  }
}