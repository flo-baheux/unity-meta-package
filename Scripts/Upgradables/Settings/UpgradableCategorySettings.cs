using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public abstract class BaseUpgradableCategorySettings : ScriptableObject
  {
    public abstract UpgradableKind upgradableKind { get; }
    public abstract List<InternalUpgradableSettings> InternalUpgradableSettings { get; }
  }

  public abstract class UpgradableCategorySettings<T_EntityKind, T_LevelSettings> : BaseUpgradableCategorySettings
    where T_EntityKind : Enum
    where T_LevelSettings : UpgradableLevelSettings
  {
    public List<UpgradableRarityLevelsSettings<T_LevelSettings>> rarityLevelsSettings;
    public List<UpgradableSettings<T_EntityKind, T_LevelSettings>> upgradableSettings;
    public override List<InternalUpgradableSettings> InternalUpgradableSettings { get => upgradableSettings.Cast<InternalUpgradableSettings>().ToList(); }
  }

  [Serializable]
  public class UpgradableRarityLevelsSettings<T_LevelSettings>
  {
    public RarityKind rarityKind;
    [HelpBox("Upgradables start at level 1.\nPlease let a first empty level settings at the start of this list!")]
    public List<T_LevelSettings> levelsSettings;
  }
}