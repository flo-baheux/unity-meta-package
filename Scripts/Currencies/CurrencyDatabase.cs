using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Currency/CurrencyDatabase", fileName = "CurrencyDatabase")]
  public class CurrencyDatabase : ScriptableObject
  {
    [SerializeField] public List<CurrencySettings> currencySettingsList;

    private void OnValidate()
    {
      foreach (var currencySettings in currencySettingsList)
        currencySettings.Refresh();

      if (HasDuplicateName())
        Debug.LogWarning($"Duplicate currency (same name) found in meta currency settings!");

      FixDuplicateIDs();
    }

    private bool HasDuplicateName()
    {
      HashSet<string> singularCurrencyNameInList = new();
      HashSet<string> pluralCurrencyNameInList = new();
      foreach (var currencySettings in currencySettingsList)
      {
        if (!singularCurrencyNameInList.Add(currencySettings.DisplayNameSingular))
          return true;

        if (!pluralCurrencyNameInList.Add(currencySettings.DisplayNamePlural))
          return true;
      }

      return false;
    }

    private void FixDuplicateIDs()
    {
      HashSet<string> currencyIDInList = new();
      foreach (var currencySettings in currencySettingsList)
        if (!currencyIDInList.Add(currencySettings.ID))
          currencySettings.ResetID();
    }
  }
}

