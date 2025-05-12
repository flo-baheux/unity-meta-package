
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace MetaPackage
{
  [CustomPropertyDrawer(typeof(RarityReference))]
  public class RarityReferenceDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var rarityID = property.FindPropertyRelative("_ID");
      var rarityName = property.FindPropertyRelative("_name");
      List<RaritySettings> allRaritySettings = (List<RaritySettings>)MetaManager.Instance.GetAllRaritySettings();

      EditorGUI.BeginProperty(position, label, property);

      string[] displayNames = allRaritySettings.Select(r => r.displayName).ToArray();
      int currentIndex = allRaritySettings.FindIndex(r => r.ID == rarityID.stringValue);

      if (currentIndex < 0)
      {
        currentIndex = 0;
        Debug.LogError($"Rarity [{rarityName.stringValue}] (ID = {rarityID.stringValue}) not found in Rarity Database. Resetting to {allRaritySettings[currentIndex].displayName}.");
      }

      int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayNames);
      var newReference = allRaritySettings[newIndex].GenerateReference();
      rarityID.stringValue = newReference.ID;
      rarityName.stringValue = newReference.Name;

      EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIUtility.singleLineHeight;
    }
  }
}