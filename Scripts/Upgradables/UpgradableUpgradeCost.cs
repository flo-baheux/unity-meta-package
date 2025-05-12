using System;

namespace MetaPackage
{
  [Serializable]
  public class UpgradableUpgradeCost
  {
    public CurrencyReference currencyReference;
    public int quantity;

    public bool ValidateReferences()
    {
      var currency = MetaManager.Instance.GetCurrency(currencyReference);
      return currency != null;
    }
  }
}