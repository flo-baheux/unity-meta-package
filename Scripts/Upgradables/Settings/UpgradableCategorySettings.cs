using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public abstract class BaseUpgradableCategorySettings : ValidatedScriptableObject
  {
    public abstract UpgradableKind upgradableKind { get; }
    public abstract List<BaseUpgradableSettings> BaseUpgradableSettings { get; }
  }

  public abstract class UpgradableCategorySettings<T_EntityKind, T_LevelSettings> : BaseUpgradableCategorySettings
    where T_EntityKind : Enum
    where T_LevelSettings : UpgradableLevelSettings
  {
    public List<UpgradableRarityLevelsSettings<T_LevelSettings>> rarityLevelsSettings;
    public List<UpgradableSettings<T_EntityKind, T_LevelSettings>> upgradableSettings;
    public override List<BaseUpgradableSettings> BaseUpgradableSettings { get => upgradableSettings.Cast<BaseUpgradableSettings>().ToList(); }

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasDuplicateRarityKind(out string duplicateRarityErrorMessage))
        errors.Add(duplicateRarityErrorMessage);

      if (HasMissingRarityKind(out string missingRarityErrorMessage))
        errors.Add(missingRarityErrorMessage);

      if (HasMissingLevelSettings(out string missingLevelSettingsErrorMessage))
        errors.Add(missingLevelSettingsErrorMessage);
    }

    public override void Refresh()
    {
      foreach (var rarityLevelSettings in rarityLevelsSettings)
        rarityLevelSettings.Refresh();
    }

    private bool HasDuplicateRarityKind(out string errorMessage)
    {
      errorMessage = $"Duplicate rarity configs in {name}.";
      HashSet<RarityKind> rarityKindsInList = new();
      foreach (var raritySettings in rarityLevelsSettings)
        if (!rarityKindsInList.Add(raritySettings.rarityKind))
          return true;

      return false;
    }

    private bool HasMissingRarityKind(out string errorMessage)
    {
      errorMessage = $"Requires one Rarity Levels Settings entry per rarity.";
      HashSet<RarityKind> completeRaritySet = Enum.GetValues(typeof(RarityKind)).Cast<RarityKind>().ToHashSet();
      HashSet<RarityKind> rarityKindsInList = new();
      rarityLevelsSettings.ForEach(x => rarityKindsInList.Add(x.rarityKind));

      bool hasSomeMissing = rarityKindsInList.SetEquals(completeRaritySet);
      if (!hasSomeMissing)
        errorMessage += $"\nMissing rarities: {string.Join(',', completeRaritySet.Except(rarityKindsInList))}";

      return !hasSomeMissing;
    }

    private bool HasMissingLevelSettings(out string errorMessage)
    {
      errorMessage = $"Requires least one \"Level 1\" for each rarity.";
      HashSet<RarityKind> ListNoLevelRarity = new();

      foreach (var raritySettings in rarityLevelsSettings)
        if (raritySettings.levelsSettings.Count < 2) // Level 0 + Level 1 = 2 entries
          ListNoLevelRarity.Add(raritySettings.rarityKind);

      if (ListNoLevelRarity.Count > 0)
        errorMessage += $"\nRarity with no \"Level 1\" configured: {string.Join(',', ListNoLevelRarity)}";

      return ListNoLevelRarity.Count > 0;
    }
#endif
  }

  [Serializable]
  public class UpgradableRarityLevelsSettings<T_LevelSettings>
    where T_LevelSettings : UpgradableLevelSettings
  {
    [HideInInspector, SerializeField] private string name;
    public RarityKind rarityKind;
    public List<T_LevelSettings> levelsSettings;

#if UNITY_EDITOR
    public void Refresh()
    {
      name = rarityKind.ToString();
      for (int i = 0; i < levelsSettings.Count; i++)
      {
        levelsSettings[i].SetName($"Level {i}{(i == 0 ? $" - Please leave this one empty" : "")}");
        levelsSettings[i].Refresh();
      }
    }
#endif
  }
}