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
    public void Refresh() => costsToUpgrade.ForEach(x => x.Refresh());
    public void SetName(string name) => this.name = name;
#endif
  }
}