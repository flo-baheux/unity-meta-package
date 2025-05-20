using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  public abstract class InternalUpgradableSettings : ScriptableObject
  {
    public abstract UpgradableKind UpgradableKind { get; }
    public abstract Enum EntityKindAsEnum { get; }
    public abstract UpgradableRewardData RewardData { get; }
    public string displayName;
    public Sprite icon;
    public RarityKind rarityKind;

    public bool unlockedByDefault = false;
    public bool eligibleForRewards = true;


    public abstract IBaseUpgradable InstantiateUpgradable();
  }

  public abstract class UpgradableSettings<T_EntityKind, T_LevelSettings> : InternalUpgradableSettings
    where T_EntityKind : Enum
    where T_LevelSettings : UpgradableLevelSettings
  {
    public T_EntityKind entityKind;
    public override Enum EntityKindAsEnum { get => entityKind; }
    public override UpgradableRewardData RewardData
    {
      get => new()
      {
        upgradableKind = UpgradableKind,
        entityKind = EntityKindAsEnum,
        rarityKind = rarityKind
      };
    }
    public UpgradableCategorySettings<T_EntityKind, T_LevelSettings> CategorySettings;

    private List<T_LevelSettings> _levelsSettings;
    public List<T_LevelSettings> LevelsSettings
    {
      get
      {
        if (_levelsSettings == null || _levelsSettings.Count == 0)
        {
          _levelsSettings = CategorySettings
            .rarityLevelsSettings
            .Find(x => x.rarityKind == rarityKind)
            .levelsSettings;
        }
        return _levelsSettings;
      }
    }
  }
}