using System;
using UnityEngine;

namespace MetaPackage
{
  [Serializable]
  public class CurrencySettings : Referenceable<CurrencyReference>
  {
    public CurrencyReference currencyReference;
    private string _displayNameSingular;
    private string _displayNamePlural;

    public string DisplayNameSingular => string.IsNullOrEmpty(_displayNameSingular) ? "" : _displayNameSingular;
    public string DisplayNamePlural => string.IsNullOrEmpty(_displayNamePlural) ? DisplayNameSingular : _displayNamePlural;

    public Sprite singleIcon;
    public Sprite multipleIcon;

    public bool unlockedByDefault = true;

    public int minimumQuantity;

    public bool hasMaximumQuantity;
    [ShowIf("hasMaximumQuantity", true)]
    public int maximumQuantity;

    public Currency InstantiateCurrency() => new(this);
    public override CurrencyReference GenerateReference() => new(ID, name);

    public void Refresh()
    {
      if (string.IsNullOrEmpty(_ID))
        ResetID();
      name = DisplayNamePlural;
    }
  }
}

