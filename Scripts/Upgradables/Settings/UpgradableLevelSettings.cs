using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  [Serializable]
  public abstract class UpgradableLevelSettings
  {
    public int experienceToNextLevel;
    public List<UpgradableUpgradeCost> costsToUpgrade;

    public virtual bool ValidateReferences()
    {
      return costsToUpgrade.All(x => x.ValidateReferences());
    }
  }
}