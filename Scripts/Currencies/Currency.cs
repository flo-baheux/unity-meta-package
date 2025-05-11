using System;
using UnityEngine;

namespace MetaPackage
{
  [Serializable]
  public class Currency
  {
    public CurrencySettings Settings { get; private set; }

    public bool IsUnlocked { get; private set; }

    private int _quantity;
    public int Quantity
    {
      get => _quantity;
      private set => _quantity = Math.Clamp(value, Settings.minimumQuantity, Settings.hasMaximumQuantity ? Settings.maximumQuantity : int.MaxValue);
    }

    public Currency(CurrencySettings settings)
    {
      this.Settings = settings;

      IsUnlocked = settings.unlockedByDefault;
      Quantity = settings.minimumQuantity;
    }

    public Action<(int, int)> OnQuantityChanged;
    public Action OnUnlocked;

    public void AdjustQuantity(int adjustBy)
    {
      if (!IsUnlocked)
        return;

      int oldValue = Quantity;
      Quantity += adjustBy;

      if (Quantity != oldValue)
        OnQuantityChanged?.Invoke((oldValue, Quantity));
    }

    public void Unlock()
    {
      if (IsUnlocked)
        return;

      IsUnlocked = true;
      OnUnlocked?.Invoke();
    }

    public string GetDisplayNameSingular() => Settings.displayNameSingular;
    public string GetDisplayNamePlural()
      => string.IsNullOrWhiteSpace(Settings.displayNamePlural)
        ? Settings.displayNameSingular
        : Settings.displayNamePlural;


    public Sprite GetSingleIcon() => Settings.singleIcon;
    public Sprite GetMultipleIcon()
      => Settings.multipleIcon == null
        ? Settings.singleIcon
        : Settings.multipleIcon;

    public void LoadSaveData(CurrencySaveData saveData)
    {
      IsUnlocked = saveData.isUnlocked;
      Quantity = saveData.quantity;
    }

    public CurrencySaveData GetSaveData() => new()
    {
      currencyKind = Settings.currencyKind,
      isUnlocked = IsUnlocked,
      quantity = Quantity,
    };

    public void ResetSaveData()
    {
      IsUnlocked = Settings.unlockedByDefault;
      Quantity = Settings.minimumQuantity;
    }
  }
}

