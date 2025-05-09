using System;

namespace MetaPackage
{
  [Serializable]
  public class UpgradableUpgradeCost
  {
    public CurrencyKind currencyKind;
    public int quantity;
  }
}