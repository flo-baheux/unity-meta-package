using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/UpgradeChestsSettings", fileName = "UpgradeChestsSettings")]
  public class UpgradeChestsSettings : ScriptableObject
  {
    [HelpBox("Data for each existing UpgradeChestKind. Used for upgrade chest rewards.", HelpBoxMessageType.Info)]
    public List<UpgradeChestSettings> upgradeChestSettingsList;
  }
}