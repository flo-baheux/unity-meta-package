using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MetaPackage
{
  [CustomEditor(typeof(UpgradeChestSettings), true)]
  public class UpgradeChestSettingsEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      bool isCustom = serializedObject.FindProperty("isCustom").boolValue;
      string[] isCustomKeysToHide = new string[] { "maxRewards", "maxOfEachReward", "rarityUpgradeChestDatas" };
      string[] isNotCustomKeysToHide = new string[] { "customUpgradeChestDatas" };
      DrawPropertiesExcluding(serializedObject, isCustom ? isCustomKeysToHide : isNotCustomKeysToHide);

      serializedObject.ApplyModifiedProperties();
    }
  }
}

