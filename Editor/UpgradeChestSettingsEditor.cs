using UnityEditor;

namespace MetaPackage
{
  [CustomEditor(typeof(UpgradeChestSettings), true)]
  public class UpgradeChestSettingsEditor : UnityEditor.Editor
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

