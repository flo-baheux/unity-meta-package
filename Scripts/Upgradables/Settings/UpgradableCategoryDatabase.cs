using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/Upgradables/UpgradableCategoryDatabase", fileName = "UpgradableCategoryDatabase")]
  public class UpgradableCategoryDatabase : ValidatedScriptableObject
  {
    public List<BaseUpgradableCategorySettings> upgradableCategorySettingsList;

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasDuplicateUpgradableCategoryKind(out string duplicateErrorMessage))
        errors.Add(duplicateErrorMessage);
    }

    public override void Refresh()
    { }

    private bool HasDuplicateUpgradableCategoryKind(out string duplicateErrorMessage)
    {
      bool duplicateFound = false;
      duplicateErrorMessage = "Cannot contain twice the same upgradableCategory.";
      Dictionary<UpgradableKind, List<BaseUpgradableCategorySettings>> upgradableCategorySettingsByUpgradableCategory = new();
      foreach (var upgradableCategorySettings in upgradableCategorySettingsList)
      {
        var upgradableKind = upgradableCategorySettings.upgradableKind;
        if (!upgradableCategorySettingsByUpgradableCategory.ContainsKey(upgradableKind))
          upgradableCategorySettingsByUpgradableCategory[upgradableKind] = new();
        else
          duplicateFound = true;

        upgradableCategorySettingsByUpgradableCategory[upgradableKind].Add(upgradableCategorySettings);
      }

      if (duplicateFound)
        foreach ((var upgradableCategoryKind, var upgradableCategorySettingsList) in upgradableCategorySettingsByUpgradableCategory)
          if (upgradableCategorySettingsList.Count > 1)
            duplicateErrorMessage += $"\n{upgradableCategoryKind} - in {string.Join(',', upgradableCategorySettingsList.Select(x => x.name))}";

      return duplicateFound;
    }
#endif
  }
}