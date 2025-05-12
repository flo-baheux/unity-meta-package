using UnityEngine;

namespace MetaPackage
{
  public class CurrencyUnlockReward : BaseReward
  {
    public CurrencyUnlockReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      MetaManager.Instance.GetCurrency(settings.currencyReference).Unlock();
    }

    public override Sprite GetSprite()
      => MetaManager.Instance.GetCurrency(settings.currencyReference).GetSingleIcon();

    public override string GetText()
      => MetaManager.Instance.GetCurrency(settings.currencyReference).GetDisplayNameSingular();
  }
}