using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/Rarity/RaritiesSettings", fileName = "RaritiesSettings")]
  public class RaritiesSettings : ScriptableObject
  {
    [HelpBox("List of rarities to be associated to chest rewardables.\nLimited to max' one of each kind, you can remove what's not used.", HelpBoxMessageType.Info)]
    public List<RaritySettings> raritySettingsList;

    private void OnValidate()
    {
      if (HasDuplicateRarityType())
        Debug.LogError($"Duplicate rarity types in meta rarity settings!");
    }

    private bool HasDuplicateRarityType()
    {
      HashSet<RarityKind> rarityTypesInList = new();
      foreach (var raritySettings in raritySettingsList)
        if (!rarityTypesInList.Add(raritySettings.kind))
          return true;

      return false;
    }
  }
}