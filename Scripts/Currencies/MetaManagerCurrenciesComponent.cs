using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerCurrenciesComponent : MetaManagerComponent
  {
    [SerializeField] private CurrencyDatabase currencyDatabase;
    private ReadOnlyDictionary<CurrencyReference, Currency> currencyByReference;

    public Action<CurrencyReference, (int oldValue, int newValue)> OnCurrencyQuantityChanged;
    public Action<CurrencyReference> OnCurrencyUnlocked;

    protected override void Setup()
    {
      Dictionary<CurrencyReference, Currency> dict = new();

      if (currencyDatabase == null || currencyDatabase.currencySettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No currency settings are set ({name}).");
        currencyByReference = new(dict);
        return;
      }

      foreach (var currencySettings in currencyDatabase.currencySettingsList)
      {
        var currency = currencySettings.InstantiateCurrency();
        var currencyReference = currencySettings.currencyReference;

        dict[currencyReference] = currency;

        currency.OnQuantityChanged += (param) => OnCurrencyQuantityChanged?.Invoke(currencyReference, param);
        currency.OnUnlocked += () => OnCurrencyUnlocked?.Invoke(currencyReference);
      }
      currencyByReference = new(dict);
    }

    public Currency GetCurrency(CurrencyReference currencyReference) => currencyByReference.GetValueOrDefault(currencyReference, null);
    public IReadOnlyList<CurrencySettings> GetAllCurrencySettings() => currencyDatabase.currencySettingsList;

    public CurrenciesSaveData GetSaveData() => new()
    {
      currenciesSaveData = currencyByReference.Values.Select(track => track.GetSaveData()).ToList(),
    };

    public void LoadSaveData(CurrenciesSaveData saveData)
    {
      foreach (CurrencySaveData currencySaveData in saveData.currenciesSaveData)
      {
        if (currencyByReference.TryGetValue(currencySaveData.currency, out Currency currency))
          currency.LoadSaveData(currencySaveData);
      }
    }

    public void ResetSaveData()
    {
      foreach (Currency currency in currencyByReference.Values)
        currency.ResetSaveData();
    }
  }
}