using UnityEngine;

namespace MetaPackage
{
  public class CurrencyUnlockReward : BaseReward
  {
    public CurrencyUnlockReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      MetaManager.Instance.GetCurrency(settings.currencyKind).Unlock();
    }

    public override Sprite GetSprite()
      => MetaManager.Instance.GetCurrency(settings.currencyKind).GetSingleIcon();

    public override string GetText()
      => MetaManager.Instance.GetCurrency(settings.currencyKind).GetDisplayNameSingular();
  }
}