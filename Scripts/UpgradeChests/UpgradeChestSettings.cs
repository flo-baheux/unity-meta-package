﻿using System.Collections.Generic;
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

    public int maxRewards;
    public int maxOfEachReward;
    public List<RarityUpgradeChestData> rarityUpgradeChestDatas;

    public List<CustomUpgradeChestData> customUpgradeChestDatas;

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasNonCompatibleCustomUpgradableKind(out string nonCompatibleUpgradableKindErrorMessage))
        errors.Add(nonCompatibleUpgradableKindErrorMessage);
    }

    public override void Refresh()
    { }
#endif

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