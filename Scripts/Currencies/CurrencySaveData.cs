using System;
using System.Collections.Generic;

namespace MetaPackage
{
  [Serializable]
  public class CurrenciesSaveData
  {
    public List<CurrencySaveData> currenciesSaveData;
  }

  [Serializable]
  public class CurrencySaveData
  {
    public CurrencyReference currency;
    public bool isUnlocked;
    public int quantity;
  }
}