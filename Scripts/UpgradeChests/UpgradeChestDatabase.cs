using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/UpgradeChestDatabase", fileName = "UpgradeChestDatabase")]
  public class UpgradeChestDatabase : ValidatedScriptableObject
  {
    [HelpBox("Data for each existing UpgradeChestKind. Used for upgrade chest rewards.", HelpBoxMessageType.Info)]
    public List<UpgradeChestSettings> upgradeChestSettingsList;

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasDuplicateUpgradeChestKind(out string duplicateErrorMessage))
        errors.Add(duplicateErrorMessage);
    }

    private bool HasDuplicateUpgradeChestKind(out string duplicateErrorMessage)
    {
      bool duplicateFound = false;
      duplicateErrorMessage = "Cannot contain twice the same chest kind.";
      Dictionary<UpgradeChestKind, List<UpgradeChestSettings>> upgradeChestSettingsByKind = new();
      foreach (var upgradeChestSettings in upgradeChestSettingsList)
      {
        var upgradeChestKind = upgradeChestSettings.upgradeChestKind;
        if (!upgradeChestSettingsByKind.ContainsKey(upgradeChestKind))
          upgradeChestSettingsByKind[upgradeChestKind] = new();
        else
          duplicateFound = true;

        upgradeChestSettingsByKind[upgradeChestKind].Add(upgradeChestSettings);
      }

      if (duplicateFound)
        foreach ((var upgradeChestKind, var upgradeChestSettingsList) in upgradeChestSettingsByKind)
          if (upgradeChestSettingsList.Count > 1)
            duplicateErrorMessage += $"\n{upgradeChestKind} - in {string.Join(',', upgradeChestSettingsList.Select(x => x.name))}";

      return duplicateFound;
    }
#endif
  }
}