using UnityEngine;

namespace MetaPackage
{
  public class UpgradableUnlockReward : BaseReward
  {
    public UpgradableUnlockReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      MetaManager.Instance.GetUpgradable(settings.upgradable.UpgradableKind, settings.upgradable.EntityKindAsEnum).Unlock();
    }

    public override Sprite GetSprite() => settings.upgradable.icon;

    public override string GetText() => settings.upgradable.displayName;
  }
}
