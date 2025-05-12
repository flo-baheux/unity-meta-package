using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Rarity/RarityDatabase", fileName = "RarityDatabase")]
  public class RarityDatabase : ScriptableObject
  {
    [HelpBox("List of rarities to be associated to chest rewardables.\nCannot have twice the same display name.", HelpBoxMessageType.Info)]
    [SerializeField] public List<RaritySettings> raritySettingsList;

    private void OnValidate()
    {
      foreach (var raritySettings in raritySettingsList)
        raritySettings.Refresh();

      if (HasDuplicateRarity())
        Debug.LogWarning($"Duplicate rarity found in meta rarity settings!");

      FixDuplicateIDs();
    }

    private bool HasDuplicateRarity()
    {
      HashSet<string> rarityNameInList = new();
      foreach (var raritySettings in raritySettingsList)
        if (!rarityNameInList.Add(raritySettings.displayName))
          return true;

      return false;
    }

    private void FixDuplicateIDs()
    {
      HashSet<string> rarityIDInList = new();
      foreach (var raritySettings in raritySettingsList)
        if (!rarityIDInList.Add(raritySettings.ID))
          raritySettings.ResetID();
    }
  }
}