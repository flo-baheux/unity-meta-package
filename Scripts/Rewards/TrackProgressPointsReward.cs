using UnityEngine;

namespace MetaPackage
{
  public class TrackProgressPointsReward : BaseReward
  {
    public TrackProgressPointsReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    => MetaManager.Instance.GetTrack(settings.track).AdjustProgressPointsBy(settings.quantity);

    public override Sprite GetSprite()
    => MetaManager.Instance.GetTrack(settings.track).ProgressPointsIcon;

    public override string GetText()
    {
      var track = MetaManager.Instance.GetTrack(settings.track);
      string name = settings.quantity == 1 ? track.ProgressPointsDisplayNameSingular : track.ProgressPointsDisplayNamePlural;
      return $"{settings.quantity} {name}";
    }
  }
}
