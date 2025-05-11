using System;

namespace MetaPackage
{
  [Serializable]
  public class RewardSettings
  {
    public int quantity = 1;

    public RewardKind rewardKind;

    [ShowIf("rewardKind", new object[] { RewardKind.TrackProgressPoints, RewardKind.TrackRewardPoints })]
    public TrackKind track;

    [ShowIf("rewardKind", RewardKind.UpgradeChest)]
    public UpgradeChestKind upgradeChestKind;

    [ShowIf("rewardKind", RewardKind.UpgradableUnlock)]
    public InternalUpgradableSettings upgradable;

    [ShowIf("rewardKind", RewardKind.Currency)]
    public CurrencyKind currencyKind;

    public void Init()
    {
      quantity = 1;
    }
  }
}