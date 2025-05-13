using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/Rarity/RarityDatabase", fileName = "RarityDatabase")]
  public class RarityDatabase : ValidatedScriptableObject
  {
    [HelpBox("List of rarities to be associated to chest rewardables.\nLimited to max' one of each kind, you can remove what's not used.", HelpBoxMessageType.Info)]
    public List<RaritySettings> raritySettingsList;

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasDuplicateRarityKind(out string duplicateErrorMessage))
        errors.Add(duplicateErrorMessage);
    }

    private bool HasDuplicateRarityKind(out string duplicateErrorMessage)
    {
      bool duplicateFound = false;
      duplicateErrorMessage = "Cannot contain twice the same rarity.";
      Dictionary<RarityKind, List<RaritySettings>> raritySettingsByRarity = new();
      foreach (var raritySettings in raritySettingsList)
      {
        var rarityKind = raritySettings.rarityKind;
        if (!raritySettingsByRarity.ContainsKey(rarityKind))
          raritySettingsByRarity[rarityKind] = new();
        else
          duplicateFound = true;

        raritySettingsByRarity[rarityKind].Add(raritySettings);
      }

      if (duplicateFound)
        foreach ((var rarityKind, var raritySettingsList) in raritySettingsByRarity)
          if (raritySettingsList.Count > 1)
            duplicateErrorMessage += $"\n{rarityKind} - in {string.Join(',', raritySettingsList.Select(x => x.name))}";

      return duplicateFound;
    }
#endif
  }
}