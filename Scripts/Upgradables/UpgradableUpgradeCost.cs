using System;
using UnityEngine;

namespace MetaPackage
{
  [Serializable]
  public class UpgradableUpgradeCost
  {
    [HideInInspector, SerializeField] private string name;

    public CurrencyKind currencyKind;
    public int quantity;

    public void CustomValidation()
    {
      var currency = MetaManager.Instance.GetCurrency(currencyKind);
      name = $"{currency.GetDisplayNamePlural()} - {quantity}";
    }
  }
}