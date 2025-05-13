using System;

namespace MetaPackage
{
  [Serializable]
  public class CustomUpgradeChestData
  {
    public InternalUpgradableSettings upgradable;
    public int quantity;
  }
}