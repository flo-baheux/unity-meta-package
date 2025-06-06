using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  public interface IBaseUpgradable
  {
    public UpgradableKind UpgradableKind { get; }
    public Enum EntityKindAsEnum { get; }
    public BaseUpgradableSettings BaseSettings { get; }

    public Action OnUnlock { get; set; }
    public Action OnLevelChanged { get; set; }
    public Action OnExperienceIncreased { get; set; }
    public Action OnUpgradeAvailable { get; set; }

    public string DisplayName { get; }
    public RarityKind RarityKind { get; }

    public int Experience { get; }
    public int Level { get; }
    public int MaxLevel { get; }

    public void IncreaseExperience(int increaseBy);
    public void ForceSetLevel(int level);

    public bool CanUpgrade();
    public bool TryUpgrade();
    public List<UpgradableUpgradeCost> CurrentCostsToUpgrade { get; }

    public bool IsUnlocked { get; }
    public bool IsEligibleForRewards { get; }
    public UpgradableRewardData RewardData { get; }
    public void Unlock();

    public int ExperienceRelativeToLevel { get; }
    public int ExperienceToNextLevel { get; }
    public bool IsCurrentLevelFirst { get; }
    public bool IsCurrentLevelLast { get; }

    public UpgradableSaveData GetSaveData();
    public void LoadSaveData(UpgradableSaveData saveData);
    public void ResetSaveData();

    public static readonly Dictionary<UpgradableKind, Type> entityKindByUpgradableKind = new() { };
  };

  public interface IUpgradable<out T_EntityKind, out T_UpgradableSettings, out T_LevelSettings> : IBaseUpgradable
    where T_EntityKind : Enum
    where T_UpgradableSettings : UpgradableSettings<T_EntityKind, T_LevelSettings>
    where T_LevelSettings : UpgradableLevelSettings
  {
    public T_UpgradableSettings Settings { get; }

    public T_LevelSettings CurrentLevelSettings { get; }
    public T_LevelSettings NextLevelSettings { get; }
    public T_LevelSettings PreviousLevelSettings { get; }
  }

  public abstract class Upgradable<T_EntityKind, T_UpgradableSettings, T_LevelSettings>
    : IUpgradable<T_EntityKind, T_UpgradableSettings, T_LevelSettings>
    where T_EntityKind : Enum
    where T_UpgradableSettings : UpgradableSettings<T_EntityKind, T_LevelSettings>
    where T_LevelSettings : UpgradableLevelSettings
  {
    public T_UpgradableSettings Settings { get; private set; }
    public BaseUpgradableSettings BaseSettings { get => Settings; }

    public UpgradableKind UpgradableKind { get => Settings.UpgradableKind; }
    public Enum EntityKindAsEnum { get => Settings.EntityKindAsEnum; }
    public T_EntityKind EntityKind { get => Settings.entityKind; }

    public UpgradableRewardData RewardData { get => Settings.RewardData; }

    public Upgradable(T_UpgradableSettings settings)
    {
      Settings = settings;
      IsUnlocked = Settings.unlockedByDefault;

      if (!IBaseUpgradable.entityKindByUpgradableKind.ContainsKey(Settings.UpgradableKind))
        IBaseUpgradable.entityKindByUpgradableKind[Settings.UpgradableKind] = typeof(T_EntityKind);
    }

    public Action OnUnlock { get; set; }
    public Action OnLevelChanged { get; set; }
    public Action OnExperienceIncreased { get; set; }
    public Action OnUpgradeAvailable { get; set; }

    public string DisplayName { get => Settings.displayName; }
    public RarityKind RarityKind { get => Settings.rarityKind; }

    private int _experience = 0;
    public int Experience
    {
      get => _experience;
      private set => _experience = Math.Clamp(value, 0, Settings.LevelsSettings.Sum(x => x.experienceToNextLevel));
    }

    private int _level = 1;
    public int Level
    {
      get => _level;
      private set => _level = Math.Clamp(value, 0, MaxLevel);
    }

    public int MaxLevel => Settings.LevelsSettings.Count() - 1;
    public int ExperienceRelativeToLevel => Experience - Settings.LevelsSettings.Take(Level).Sum(x => x.experienceToNextLevel);
    public int ExperienceToNextLevel => CurrentLevelSettings.experienceToNextLevel;

    public T_LevelSettings PreviousLevelSettings => IsCurrentLevelFirst ? null : Settings.LevelsSettings.ElementAt(Level - 1);
    public T_LevelSettings CurrentLevelSettings => Settings.LevelsSettings.ElementAt(Level);
    public T_LevelSettings NextLevelSettings => IsCurrentLevelLast ? null : Settings.LevelsSettings.ElementAt(Level + 1);

    public bool IsCurrentLevelFirst => Level == 1;
    public bool IsCurrentLevelLast => Level == MaxLevel;
    public bool IsEligibleForRewards { get => Settings.eligibleForRewards && IsUnlocked; }

    public bool IsUnlocked { get; private set; }

    // Experience & Upgrade

    public void IncreaseExperience(int increaseBy)
    {
      if (increaseBy <= 0)
        throw new InvalidOperationException($"Can only increase entity experience by a strictly positive value. Tried increasing experience by [{increaseBy}].");

      if (!IsUnlocked)
        return;

      bool couldUpgradeBefore = CanUpgrade();
      int oldValue = Experience;
      Experience += increaseBy;
      OnExperienceIncreased?.Invoke();

      if (Experience != oldValue && !couldUpgradeBefore && CanUpgrade())
        OnUpgradeAvailable?.Invoke();
    }

    public List<UpgradableUpgradeCost> CurrentCostsToUpgrade { get => CurrentLevelSettings.costsToUpgrade; }

    public bool TryUpgrade()
    {
      if (!CanUpgrade())
        return false;
      if (!TryPayForUpgrade())
        return false;

      int oldLevel = Level;
      Level += 1;
      if (oldLevel == Level)
        return false;

      OnLevelChanged?.Invoke();
      return true;
    }

    public void ForceSetLevel(int level)
    {
      int oldLevel = Level;
      Level = level;
      if (oldLevel != level)
        OnLevelChanged?.Invoke();
    }

    public void Unlock()
    {
      if (IsUnlocked)
        return;

      IsUnlocked = true;
      OnUnlock?.Invoke();
    }

    public virtual bool CanUpgrade()
    {
      if (!IsUnlocked)
        return false;

      if (IsCurrentLevelLast || ExperienceRelativeToLevel < CurrentLevelSettings.experienceToNextLevel)
        return false;

      if (CurrentCostsToUpgrade.Any(cost => cost.quantity > MetaManager.Instance.GetCurrency(cost.currencyKind).Quantity))
        return false;

      return true;
    }

    protected virtual bool TryPayForUpgrade()
    {
      if (!CanUpgrade())
        return false;

      foreach (var cost in CurrentCostsToUpgrade)
        MetaManager.Instance.GetCurrency(cost.currencyKind).AdjustQuantity(-cost.quantity);

      return true;
    }

    // Load & Save

    public UpgradableSaveData GetSaveData()
    {
      return new()
      {
        upgradableKindAsInt = Convert.ToInt32(Settings.UpgradableKind),
        entityKindAsInt = Convert.ToInt32(Settings.entityKind),
        level = Level,
        experience = Experience,
        isUnlocked = IsUnlocked
      };
    }

    public void LoadSaveData(UpgradableSaveData saveData)
    {
      if (saveData.upgradableKindAsInt != Convert.ToInt32(Settings.UpgradableKind) || saveData.entityKindAsInt != Convert.ToInt32(Settings.entityKind))
        throw new InvalidOperationException("Incompatible save data");

      _level = saveData.level;
      _experience = saveData.experience;
      IsUnlocked = Settings.unlockedByDefault || saveData.isUnlocked;
    }

    public void ResetSaveData()
    {
      _level = 1;
      _experience = 0;
      IsUnlocked = Settings.unlockedByDefault;
    }
  }
}