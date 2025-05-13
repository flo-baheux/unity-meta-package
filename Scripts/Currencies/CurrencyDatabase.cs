using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/CurrencyDatabase", fileName = "CurrencyDatabase")]
  public class CurrencyDatabase : ValidatedScriptableObject
  {
    public List<CurrencySettings> currencySettingsList;

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasDuplicateCurrencyKind(out string duplicateErrorMessage))
        errors.Add(duplicateErrorMessage);
    }

    private bool HasDuplicateCurrencyKind(out string duplicateErrorMessage)
    {
      bool duplicateFound = false;
      duplicateErrorMessage = "Cannot contain twice the same currency.";
      Dictionary<CurrencyKind, List<CurrencySettings>> currencySettingsByCurrency = new();
      foreach (var currencySettings in currencySettingsList)
      {
        var currencyKind = currencySettings.currencyKind;
        if (!currencySettingsByCurrency.ContainsKey(currencyKind))
          currencySettingsByCurrency[currencyKind] = new();
        else
          duplicateFound = true;

        currencySettingsByCurrency[currencyKind].Add(currencySettings);
      }

      if (duplicateFound)
        foreach ((var currencyKind, var currencySettingsList) in currencySettingsByCurrency)
          if (currencySettingsList.Count > 1)
            duplicateErrorMessage += $"\n{currencyKind} - in {string.Join(',', currencySettingsList.Select(x => x.name))}";

      return duplicateFound;
    }
#endif
  }
}

