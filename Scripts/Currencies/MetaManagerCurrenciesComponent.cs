using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerCurrenciesComponent : MetaManagerComponent
  {
    [SerializeField] private CurrencyDatabase currenciesSettings;
    private ReadOnlyDictionary<CurrencyKind, Currency> currenciesByKind;

    public Action<CurrencyKind, (int oldValue, int newValue)> OnCurrencyQuantityChanged;
    public Action<CurrencyKind> OnCurrencyUnlocked;

    protected override void Setup()
    {
      Dictionary<CurrencyKind, Currency> dict = new();

      if (currenciesSettings.currencySettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No currency settings are set ({name}).");
        currenciesByKind = new(dict);
        return;
      }

      foreach (var currencySettings in currenciesSettings.currencySettingsList)
      {
        var instanciatedCurrencySettings = ScriptableObject.Instantiate(currencySettings);
        var currency = instanciatedCurrencySettings.InstantiateCurrency();
        var currencyKind = instanciatedCurrencySettings.currencyKind;

        dict[currencyKind] = currency;

        currency.OnQuantityChanged += (param) => OnCurrencyQuantityChanged?.Invoke(currencyKind, param);
        currency.OnUnlocked += () => OnCurrencyUnlocked?.Invoke(currencyKind);
      }
      currenciesByKind = new(dict);
    }

    public Currency GetCurrency(CurrencyKind kind)
      => currenciesByKind[kind];

    public CurrenciesSaveData GetSaveData() => new()
    {
      currenciesSaveData = currenciesByKind.Values.Select(track => track.GetSaveData()).ToList(),
    };

    public void LoadSaveData(CurrenciesSaveData saveData)
    {
      foreach (CurrencySaveData currencySaveData in saveData.currenciesSaveData)
      {
        if (currenciesByKind.TryGetValue(currencySaveData.currencyKind, out Currency currency))
          currency.LoadSaveData(currencySaveData);
      }
    }

    public void ResetSaveData()
    {
      foreach (Currency currency in currenciesByKind.Values)
        currency.ResetSaveData();
    }
  }
}