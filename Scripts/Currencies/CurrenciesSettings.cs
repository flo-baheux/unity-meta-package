using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/CurrenciesSettings", fileName = "CurrenciesSettings")]
  public class CurrenciesSettings: ScriptableObject
  {
    public List<CurrencySettings> currencySettingsList;
  }
}

