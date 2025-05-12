using UnityEngine;

namespace MetaPackage
{
  public class CurrencyReward : BaseReward
  {
    public CurrencyReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      MetaManager.Instance.GetCurrency(settings.currencyReference).AdjustQuantity(settings.quantity);
    }

    public override Sprite GetSprite()
      => settings.quantity == 1
        ? MetaManager.Instance.GetCurrency(settings.currencyReference).GetSingleIcon()
        : MetaManager.Instance.GetCurrency(settings.currencyReference).GetMultipleIcon();

    public override string GetText()
      => settings.quantity == 1
        ? MetaManager.Instance.GetCurrency(settings.currencyReference).GetDisplayNameSingular()
        : MetaManager.Instance.GetCurrency(settings.currencyReference).GetDisplayNamePlural();
  }
}
