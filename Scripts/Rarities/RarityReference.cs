using System;

namespace MetaPackage
{
  [Serializable]
  public class RarityReference : Identifiable
  {
    public RarityReference(string ID, string name) : base(ID, name) { }
  }
}