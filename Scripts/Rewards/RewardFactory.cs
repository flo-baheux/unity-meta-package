using System;

namespace MetaPackage
{
  public static class RewardFactory
  {
    public static BaseReward Build(RewardSettings settings)
    {
      switch (settings.rewardKind)
      {
        case RewardKind.TrackProgressPoints:
          return new TrackProgressPointsReward(settings);
        case RewardKind.TrackRewardPoints:
          return new TrackRewardPointsReward(settings);
        case RewardKind.UpgradeChest:
          return new UpgradeChestReward(settings);
        case RewardKind.UpgradableUnlock:
          return new UpgradableUnlockReward(settings);
        case RewardKind.Currency:
          return new CurrencyReward(settings);
        case RewardKind.CurrencyUnlock:
          return new CurrencyUnlockReward(settings);

        default:
          throw new NotImplementedException($"Reward kind {settings.rewardKind} has no associated implementation. Did you forget to update RewardFactory?");
      }
    }
  }
}
