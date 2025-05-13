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

#if UNITY_EDITOR
    public void CustomValidation()
    {
      costsToUpgrade.ForEach(x => x.CustomValidation());
    }

    public void SetName(string name) => this.name = name;
#endif
  }
}