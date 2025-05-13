using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerRaritiesComponent : MetaManagerComponent
  {
    [SerializeField] private RarityDatabase raritiesSettings;
    public ReadOnlyDictionary<RarityKind, RaritySettings> raritySettingsByKind;

    protected override void Setup()
    {
      Dictionary<RarityKind, RaritySettings> dict = new();

      if (!raritiesSettings || raritiesSettings.raritySettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No rarity settings set ({name}).");
        raritySettingsByKind = new(dict);
        return;
      }

      foreach (var raritySettings in raritiesSettings.raritySettingsList)
      {
        var instantiatedRaritySettings = ScriptableObject.Instantiate(raritySettings);
        dict[raritySettings.rarityKind] = instantiatedRaritySettings;
      }
      raritySettingsByKind = new(dict);
    }
  }
}