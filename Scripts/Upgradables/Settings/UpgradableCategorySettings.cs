using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public abstract class BaseUpgradableCategorySettings : ValidatedScriptableObject
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

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      foreach (var rarityLevelSettings in rarityLevelsSettings)
        rarityLevelSettings.CustomValidation();

      if (HasDuplicateRarityKind(out string duplicateRarityErrorMessage))
        errors.Add(duplicateRarityErrorMessage);

      if (HasMissingRarityKind(out string missingRarityErrorMessage))
        errors.Add(missingRarityErrorMessage);

      if (HasMissingLevelSettings(out string missingLevelSettingsErrorMessage))
        errors.Add(missingLevelSettingsErrorMessage);

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
      errorMessage = $"Requires one levels settings entry per rarity.";
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
      errorMessage = $"Requires at least one level for each rarity.";
      HashSet<RarityKind> ListNoLevelRarity = new();

      foreach (var raritySettings in rarityLevelsSettings)
        if (raritySettings.levelsSettings.Count == 0)
          ListNoLevelRarity.Add(raritySettings.rarityKind);

      if (ListNoLevelRarity.Count > 0)
        errorMessage += $"\nRarity with no levels configured: {string.Join(',', ListNoLevelRarity)}";

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
    public void CustomValidation()
    {
      name = rarityKind.ToString();
      for (int i = 0; i < levelsSettings.Count; i++)
      {
        levelsSettings[i].SetName($"Level {i + 1}");
        levelsSettings[i].CustomValidation();
      }
    }
  }
#endif
}