using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  public abstract class InternalUpgradableSettings : ScriptableObject
  {
    public abstract UpgradableKind UpgradableKind { get; }
    public abstract Enum EntityKindAsEnum { get; }
    public string displayName;
    public Sprite icon;
    public RarityReference rarity;

    public bool unlockedByDefault = false;
    public bool eligibleForRewards = true;

    public abstract IBaseUpgradable InstantiateUpgradable();

    public virtual bool ValidateReferences()
    {
      var raritySettings = MetaManager.Instance.GetRaritySettings(rarity);
      return raritySettings != null;
    }
  }

  public abstract class UpgradableSettings<T_EntityKind, T_LevelSettings> : InternalUpgradableSettings
    where T_EntityKind : Enum
    where T_LevelSettings : UpgradableLevelSettings
  {
    public T_EntityKind entityKind;
    public override Enum EntityKindAsEnum { get => entityKind; }
    public UpgradableCategorySettings<T_EntityKind, T_LevelSettings> CategorySettings;

    private List<T_LevelSettings> _levelsSettings;
    public List<T_LevelSettings> LevelsSettings
    {
      get
      {
        if (_levelsSettings == null)
        {
          _levelsSettings = CategorySettings
            .rarityLevelsSettings
            .Find(x => x.rarity == rarity)
            .levelsSettings;
        }
        return _levelsSettings;
      }
    }
  }
}