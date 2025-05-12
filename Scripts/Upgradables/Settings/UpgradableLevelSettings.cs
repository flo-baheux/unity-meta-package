using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [Serializable]
  public abstract class UpgradableLevelSettings
  {
    [HideInInspector, SerializeField] private string name;
    public int experienceToNextLevel;
    public List<UpgradableUpgradeCost> costsToUpgrade;

    public void CustomOnValidate(string name = null)
    {
      if (name != null)
        this.name = name;

      for (int i = 0; i < costsToUpgrade.Count; i++)
        costsToUpgrade[i].CustomOnValidate();
    }
  }
}