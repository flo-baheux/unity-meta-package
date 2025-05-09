using UnityEngine;

namespace MetaPackage
{
  public abstract class BaseReward
  {
    public readonly RewardSettings settings;

    public BaseReward(RewardSettings settings)
    {
      this.settings = settings;
    }

    public abstract void Claim();
    public abstract Sprite GetSprite();
    public abstract string GetText();

    public override string ToString()
      => $"{settings.rewardKind} x{settings.quantity}";
  }
}
