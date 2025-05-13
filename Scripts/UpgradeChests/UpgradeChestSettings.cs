using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/UpgradeChest", fileName = "UpgradeChestSettings")]
  public class UpgradeChestSettings : ValidatedScriptableObject
  {
    public bool isCustom = false;

    public UpgradeChestKind upgradeChestKind;
    public string displayName;
    public Sprite icon;
    public Sprite iconTop;
    public Sprite iconBottom;
    public UpgradableKind upgradableKind;

    [ShowIf("isCustom", false)]
    public int maxRewards;
    [ShowIf("isCustom", false)]
    public int maxOfEachReward;

    // FIXME: Custom UpgradeChestSettings editor to hide the wrong one
    public List<RarityUpgradeChestData> rarityUpgradeChestDatas;
    public List<CustomUpgradeChestData> customUpgradeChestDatas;

    public override void CustomValidation()
    {
      if (HasNonCompatibleCustomUpgradableKind(out string nonCompatibleUpgradableKindErrorMessage))
        errors.Add(nonCompatibleUpgradableKindErrorMessage);
    }

    private bool HasNonCompatibleCustomUpgradableKind(out string errorMessage)
    {
      errorMessage = $"Custom upgrades are not compatible (not of upgradable kind {upgradableKind}).";
      bool foundNotCompatible = false;
      if (!isCustom)
        return false;

      foreach (var customUpgradeChestData in customUpgradeChestDatas)
        if (customUpgradeChestData.upgradable.UpgradableKind != upgradableKind)
        {
          foundNotCompatible = true;
          errorMessage += $"\n{customUpgradeChestData.upgradable.name} ({customUpgradeChestData.upgradable.UpgradableKind})";
        }

      return foundNotCompatible;
    }
  }
}