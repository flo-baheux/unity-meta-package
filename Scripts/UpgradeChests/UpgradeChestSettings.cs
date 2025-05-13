using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/UpgradeChest", fileName = "UpgradeChestSettings")]
  public class UpgradeChestSettings : ScriptableObject
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
  }
}