using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Currency", fileName = "CurrencySettings")]
  public class CurrencySettings: ScriptableObject
  {
    public CurrencyKind currencyKind;
    public string displayNameSingular;
    public string displayNamePlural;

    public Sprite singleIcon;
    public Sprite multipleIcon;

    public bool unlockedByDefault = true;

    public int minimumQuantity;

    public bool hasMaximumQuantity;
    [ShowIf("hasMaximumQuantity", true)]
    public int maximumQuantity;

    public Currency InstantiateCurrency() => new Currency(this);
  }
}

