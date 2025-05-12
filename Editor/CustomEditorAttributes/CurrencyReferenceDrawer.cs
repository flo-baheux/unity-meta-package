
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace MetaPackage
{
  [CustomPropertyDrawer(typeof(CurrencyReference))]
  public class CurrencyReferenceDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var currencyID = property.FindPropertyRelative("_ID");
      var currencyName = property.FindPropertyRelative("_name");
      List<CurrencySettings> allCurrencySettings = (List<CurrencySettings>)MetaManager.Instance.GetAllCurrencySettings();

      EditorGUI.BeginProperty(position, label, property);

      string[] displayNames = allCurrencySettings.Select(r => r.DisplayNamePlural).ToArray();
      int currentIndex = allCurrencySettings.FindIndex(r => r.ID == currencyID.stringValue);

      if (currentIndex < 0)
      {
        currentIndex = 0;
        Debug.LogError($"currency [{currencyName.stringValue}] (ID = {currencyID.stringValue}) not found in Currency Database. Resetting to {allCurrencySettings[currentIndex].DisplayNamePlural}.");
      }

      int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayNames);
      var newReference = allCurrencySettings[newIndex].GenerateReference();
      currencyID.stringValue = newReference.ID;
      currencyName.stringValue = newReference.Name;

      EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUIUtility.singleLineHeight;
    }
  }
}