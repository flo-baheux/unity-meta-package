using System;
using System.Collections.Generic;

namespace MetaPackage
{
  [Serializable]
  public abstract class UpgradableLevelSettings
  {
    public int experienceToNextLevel;
    public List<UpgradableUpgradeCost> costsToUpgrade;
  }
}