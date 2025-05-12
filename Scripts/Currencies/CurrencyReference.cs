using System;

namespace MetaPackage
{
  [Serializable]
  public class CurrencyReference : Identifiable
  {
    public CurrencyReference(string ID, string name) : base(ID, name) { }
  }
}