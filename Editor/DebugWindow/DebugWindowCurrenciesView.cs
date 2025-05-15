using System;
using System.Collections.Generic;
using System.Linq;
using MetaPackage;
using UnityEngine;
using UnityEngine.UIElements;

using static MetaPackageDebug.Utils;

namespace MetaPackageDebug
{
  public class DebugWindowCurrenciesView
  {

    public VisualElement BuildCurrenciesView()
    {
      try
      {
        VisualElement currenciesView = new Box();

        currenciesView.Add(new HelpBox("If you cannot find a currency, it's not configured properly in MetaManager", UnityEngine.UIElements.HelpBoxMessageType.Info));
        var firstCurrencyInEnum = Enum.GetValues(typeof(CurrencyKind)).Cast<CurrencyKind>().ElementAt(0);
        var currenciesDropdown = new EnumField("Selected currency", firstCurrencyInEnum);
        currenciesView.Add(currenciesDropdown);

        Dictionary<CurrencyKind, VisualElement> viewByKind = new();
        foreach (CurrencyKind currencyKind in Enum.GetValues(typeof(CurrencyKind)))
        {
          var currencyView = BuildCurrencyView(currencyKind);
          SetVisible(currencyView, currencyKind.Equals(firstCurrencyInEnum));
          viewByKind[currencyKind] = currencyView;
          currenciesView.Add(currencyView);
        }

        currenciesDropdown.RegisterValueChangedCallback(e =>
        {
          foreach ((Enum key, var value) in viewByKind)
            SetVisible(value, key.Equals(e.newValue));
        });

        return currenciesView;

      }
      catch (Exception e)
      {
        Debug.LogException(e);
        return BuildErrorLabel("Failed to build currencies view");
      }
    }

    private VisualElement BuildCurrencyView(CurrencyKind currencyKind)
    {
      VisualElement currencyView = new Box();

      var data = new CurrencyDataAccessor(currencyKind);

      string currencyName = string.IsNullOrWhiteSpace(data.currency.GetDisplayNameSingular()) ? "[no display name]" : $"1 {data.currency.GetDisplayNameSingular()} / n {data.currency.GetDisplayNamePlural()}";
      var currencyNameLabel = BuildLabel($"Currency name: {currencyName}", 16);
      currencyView.Add(currencyNameLabel);

      var minQuantityLabel = BuildLabel($"Minimum quantity: {data.settings.minimumQuantity}", marginBottom: 0);
      currencyView.Add(minQuantityLabel);

      var maxQuantityLabel = BuildLabel($"Maximum quantity: {(data.settings.hasMaximumQuantity ? data.settings.maximumQuantity : "Infinite")}", marginTop: 0, marginBottom: 0);
      currencyView.Add(maxQuantityLabel);

      var unlockedByDefaultLabel = BuildLabel($"Unlocked by default? {(data.settings.unlockedByDefault ? "yes" : "no")}", marginTop: 0, marginBottom: 0);
      currencyView.Add(unlockedByDefaultLabel);

      var isUnlockedLabel = BuildLabel("", marginTop: 0, marginBottom: 0);
      BindLabel(isUnlockedLabel, () => $"Currently unlocked? {(data.currency.IsUnlocked ? "yes" : "no")}");
      currencyView.Add(isUnlockedLabel);

      var unlockButton = new Button(() => data.currency.Unlock());
      BindActionToElement(unlockButton, () =>
      {
        bool isUnlocked = data.currency.IsUnlocked;
        unlockButton.text = isUnlocked ? "Currency is unlocked" : "Unlock currency";
        SetVisible(unlockButton, !isUnlocked);
      });
      currencyView.Add(unlockButton);

      var currentQuantityLabel = BuildLabel("", 16);
      BindLabel(currentQuantityLabel, () => $"Current quantity: {data.currency.Quantity}");
      currencyView.Add(currentQuantityLabel);

      var adjustmentView = BuildCurrencyAdjustmentView(data);
      currencyView.Add(adjustmentView);

      return currencyView;
    }

    private VisualElement BuildCurrencyAdjustmentView(CurrencyDataAccessor data)
    {
      var adjustmentView = new VisualElement();

      var quantityContainer = new VisualElement();
      quantityContainer.style.flexDirection = FlexDirection.Row;
      quantityContainer.style.marginTop = 5;

      var quantityField = new IntegerField("Quantity");
      quantityField.style.flexGrow = 1;
      quantityField.style.minWidth = 100;
      quantityField.labelElement.style.minWidth = 100;
      quantityField.labelElement.style.flexBasis = 100;
      quantityField.value = 0;
      quantityContainer.Add(quantityField);

      var adjustButton = new Button(() => data.currency.AdjustQuantity(quantityField.value));
      adjustButton.text = "Adjust";
      quantityContainer.Add(adjustButton);

      adjustmentView.Add(quantityContainer);

      return adjustmentView;
    }

    private class CurrencyDataAccessor
    {
      public CurrencyKind currencyKind;
      public Currency currency;
      public CurrencySettings settings;

      public CurrencyDataAccessor(CurrencyKind currencyKind)
      {
        this.currencyKind = currencyKind;
        currency = MetaManager.Instance.GetCurrency(currencyKind);
        settings = currency.Settings;
      }
    }
  }
}