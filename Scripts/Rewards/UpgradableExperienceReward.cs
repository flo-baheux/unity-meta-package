using UnityEngine;

namespace MetaPackage
{
  public class UpgradableExperienceReward : BaseReward
  {
    public UpgradableExperienceReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      var upgradable = MetaManager.Instance.GetUpgradable(settings.upgradable.UpgradableKind, settings.upgradable.EntityKindAsEnum);
      upgradable.IncreaseExperience(settings.quantity);
    }

    public override Sprite GetSprite() => settings.upgradable.icon;

    public override string GetText() => $"{settings.quantity}xp for {settings.upgradable.displayName}";
  }
}
