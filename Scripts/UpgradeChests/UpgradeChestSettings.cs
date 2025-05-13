using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/UpgradeChest", fileName = "UpgradeChestSettings")]
  public class UpgradeChestSettings : ScriptableObject
  {
    public UpgradeChestKind upgradeChestKind;
    public string displayName;
    public Sprite icon;
    public Sprite iconTop;
    public Sprite iconBottom;
    public UpgradableKind upgradableKind;
    public int maxRewards;
    public int maxOfEachReward;
    public List<RarityUpgradeChestData> rarityUpgradeChestDatas;
  }
}