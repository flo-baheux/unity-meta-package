using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  [Serializable]
  public enum RewardBundleStateEnum
  {
    Locked,
    Available,
    Claimed
  };

  public abstract class RewardBundle
  {

    public Action<RewardBundle> OnAvailable;
    public Action<RewardBundle> OnClaimed;
    public RewardBundleStateEnum state;

    public List<BaseReward> rewards = new();

    public RewardBundle(List<RewardSettings> rewardsSettings)
    {
      state = RewardBundleStateEnum.Locked;
      foreach (var rewardSettings in rewardsSettings)
        rewards.Add(RewardFactory.Build(rewardSettings));
    }

    public bool TrySetAvailable()
    {
      if (state != RewardBundleStateEnum.Locked)
        return false;

      state = RewardBundleStateEnum.Available;
      OnAvailable?.Invoke(this);
      return true;
    }

    public bool TryClaim()
    {
      if (state != RewardBundleStateEnum.Available)
        return false;

      foreach (BaseReward reward in rewards)
        reward.Claim();

      state = RewardBundleStateEnum.Claimed;
      OnClaimed?.Invoke(this);
      return true;
    }

    public void HardSetClaimedState()
    {
      state = RewardBundleStateEnum.Claimed;
    }

    public override string ToString()
      => $"[{state}] - {string.Join(", ", rewards.Select(r => $"{r}"))}";
  }
}
