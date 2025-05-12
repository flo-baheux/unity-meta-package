using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerUpgradeChestsComponent : MetaManagerComponent
  {
    [SerializeField] private UpgradeChestsSettings upgradeChestsSettings;
    public ReadOnlyDictionary<UpgradeChestKind, UpgradeChestSettings> upgradeChestSettingsByKind;

    protected override void Setup()
    {
      Dictionary<UpgradeChestKind, UpgradeChestSettings> dict = new();

      if (!upgradeChestsSettings || upgradeChestsSettings.upgradeChestSettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No upgrade chest settings set ({name})");
        upgradeChestSettingsByKind = new(dict);
        return;
      }

      foreach (var upgradeChestSettings in upgradeChestsSettings.upgradeChestSettingsList)
      {
        var instantiatedUpgradeChestSettings = ScriptableObject.Instantiate(upgradeChestSettings);
        dict[upgradeChestSettings.kind] = instantiatedUpgradeChestSettings;
      }
      upgradeChestSettingsByKind = new(dict);
    }
  }
}