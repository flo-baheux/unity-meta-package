using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerRaritiesComponent : MetaManagerComponent
  {
    [SerializeField] public RarityDatabase rarityDatabase;
    public ReadOnlyDictionary<RarityReference, RaritySettings> raritySettingsByReference;

    protected override void Setup()
    {
      Dictionary<RarityReference, RaritySettings> dict = new();

      if (!rarityDatabase || rarityDatabase.raritySettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No rarity settings set ({name}).");
        raritySettingsByReference = new(dict);
        return;
      }

      foreach (var raritySettings in rarityDatabase.raritySettingsList)
      {
        RarityReference reference = raritySettings.GenerateReference();
        dict[reference] = raritySettings;
      }
      raritySettingsByReference = new(dict);
    }

    public RaritySettings GetRaritySettings(RarityReference rarityReference) => raritySettingsByReference.GetValueOrDefault(rarityReference, null);
    public IReadOnlyList<RaritySettings> GetAllRaritySettings() => rarityDatabase.raritySettingsList;
  }
}