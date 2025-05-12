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

    public void OnValidate()
    {
      foreach (var rarityLevelSettings in rarityLevelsSettings)
        rarityLevelSettings.CustomOnValidate();
    }
  }

  [Serializable]
  public class UpgradableRarityLevelsSettings<T_LevelSettings>
    where T_LevelSettings : UpgradableLevelSettings
  {
    [HideInInspector, SerializeField] private string name;
    public RarityKind rarityKind;
    public List<T_LevelSettings> levelsSettings;

    public void CustomOnValidate()
    {
      name = MetaManager.Instance.GetRaritySettings(rarityKind).displayName;
      for (int i = 0; i < levelsSettings.Count; i++)
        levelsSettings[i].CustomOnValidate($"Level {i + 1}");
    }
  }
}