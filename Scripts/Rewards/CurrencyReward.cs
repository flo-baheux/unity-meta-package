using UnityEngine;

namespace MetaPackage
{
  public class CurrencyReward : BaseReward
  {
    public CurrencyReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      MetaManager.Instance.GetCurrency(settings.currencyKind).AdjustQuantity(settings.quantity);
    }

    public override Sprite GetSprite()
      => settings.quantity == 1
        ? MetaManager.Instance.GetCurrency(settings.currencyKind).GetSingleIcon()
        : MetaManager.Instance.GetCurrency(settings.currencyKind).GetMultipleIcon();

    public override string GetText()
      => settings.quantity == 1
        ? MetaManager.Instance.GetCurrency(settings.currencyKind).GetDisplayNameSingular()
        : MetaManager.Instance.GetCurrency(settings.currencyKind).GetDisplayNamePlural();
  }
}
