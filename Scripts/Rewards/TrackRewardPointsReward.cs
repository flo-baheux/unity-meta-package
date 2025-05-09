using UnityEngine;

namespace MetaPackage
{
  public class TrackRewardPointsReward : BaseReward
  {
    public TrackRewardPointsReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      MetaManager.Instance.GetTrack(settings.track).AdjustRewardPointsBy(settings.quantity);
    }

    public override Sprite GetSprite()
    => MetaManager.Instance.GetTrack(settings.track).RewardPointsIcon;

    public override string GetText()
    {
      var track = MetaManager.Instance.GetTrack(settings.track);
      string name = settings.quantity == 1 ? track.RewardPointsDisplayNameSingular : track.RewardPointsDisplayNamePlural;
      return $"{settings.quantity} {name}";
    }
  }
}
