using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerUpgradablesComponent : MetaManagerComponent
  {
    [SerializeField] private UpgradableCategoryDatabase upgradableCategoriesSettings;
    private ReadOnlyDictionary<(int, int), IBaseUpgradable> upgradableByKind;
    private ReadOnlyDictionary<UpgradableKind, BaseUpgradableCategorySettings> categorySettingsByKind;

    public Action<UpgradableKind, Enum> OnUnlock { get; set; }
    public Action<UpgradableKind, Enum> OnExperienceIncreased { get; set; }
    public Action<UpgradableKind, Enum> OnLevelChanged { get; set; }
    public Action<UpgradableKind, Enum> OnUpgradeAvailable { get; set; }

    protected override void Setup()
    {
      Dictionary<(int, int), IBaseUpgradable> upgradableDict = new();
      Dictionary<UpgradableKind, BaseUpgradableCategorySettings> upgradableCategoryDict = new();

      if (upgradableCategoriesSettings == null || upgradableCategoriesSettings.upgradableCategorySettingsList.Count == 0)
      {
        Debug.LogWarning("[Meta Manager] - No upgradable entities set.");
        categorySettingsByKind = new(upgradableCategoryDict);
        upgradableByKind = new(upgradableDict);
        return;
      }

      foreach (var categorySettings in upgradableCategoriesSettings.upgradableCategorySettingsList)
      {
        BaseUpgradableCategorySettings instantiatedCategorySettings = ScriptableObject.Instantiate(categorySettings);
        upgradableCategoryDict[categorySettings.upgradableKind] = instantiatedCategorySettings;
        foreach (var upgradableSettings in categorySettings.BaseUpgradableSettings)
        {
          var upgradable = ScriptableObject.Instantiate(upgradableSettings).InstantiateUpgradable();
          upgradableDict[((int)upgradableSettings.UpgradableKind, Convert.ToInt32(upgradableSettings.EntityKindAsEnum))] = upgradable;
          upgradable.OnUnlock += () => OnUnlock?.Invoke(upgradableSettings.UpgradableKind, upgradableSettings.EntityKindAsEnum);
          upgradable.OnExperienceIncreased += () => OnExperienceIncreased?.Invoke(upgradableSettings.UpgradableKind, upgradableSettings.EntityKindAsEnum);
          upgradable.OnLevelChanged += () => OnLevelChanged?.Invoke(upgradableSettings.UpgradableKind, upgradableSettings.EntityKindAsEnum);
          upgradable.OnUpgradeAvailable += () => OnUpgradeAvailable?.Invoke(upgradableSettings.UpgradableKind, upgradableSettings.EntityKindAsEnum);
        }
      }

      categorySettingsByKind = new(upgradableCategoryDict);
      upgradableByKind = new(upgradableDict);
    }

    public T GetUpgradable<T>(UpgradableKind upgradableKind, Enum entityKind)
      where T : IBaseUpgradable
    {
      if (!upgradableByKind.TryGetValue((Convert.ToInt32(upgradableKind), Convert.ToInt32(entityKind)), out IBaseUpgradable upgradable))
      {
        Debug.LogWarning($"Upgradable entity of kind {entityKind} not found. Please make sure all upgradable entity settings are properly set.");
        return default;
      }

      if (upgradable is T typedUpgradable)
        return typedUpgradable;

      throw new InvalidCastException($"Upgradable entity of kind {entityKind} is not of type {typeof(T)}.");
    }

    public T GetCategorySettings<T>(UpgradableKind upgradableKind)
      where T : BaseUpgradableCategorySettings
    {
      if (!categorySettingsByKind.TryGetValue(upgradableKind, out BaseUpgradableCategorySettings categorySettings))
      {
        Debug.LogWarning($"Upgradable category settings of kind {upgradableKind} not found. Please make sure all upgradable category settings are properly set.");
        return default;
      }

      if (categorySettings is T typedCategorySettings)
        return typedCategorySettings;

      throw new InvalidCastException($"Upgradable category settings of kind {upgradableKind} is not of type {typeof(T)}.");
    }

    public List<UpgradableRewardData> GetAllEligibeForReward(UpgradableKind upgradableKind)
    {
      List<UpgradableRewardData> eligilesRewardData = new();
      foreach (((var upgradableKindAsInt, _), var upgradable) in upgradableByKind)
        if (upgradableKind == (UpgradableKind)upgradableKindAsInt && upgradable.IsEligibleForRewards)
          eligilesRewardData.Add(upgradable.RewardData);

      return eligilesRewardData;
    }

    public UpgradablesSaveData GetSaveData() => new()
    {
      upgradablesSaveData = upgradableByKind.Values.Select(x => x.GetSaveData()).ToList()
    };

    public void LoadSaveData(UpgradablesSaveData saveData)
    {
      foreach (UpgradableSaveData upgradableSaveData in saveData.upgradablesSaveData)
      {
        if (upgradableByKind.TryGetValue((upgradableSaveData.upgradableKindAsInt, upgradableSaveData.entityKindAsInt), out IBaseUpgradable upgradable))
          upgradable.LoadSaveData(upgradableSaveData);
      }
    }

    public void ResetSaveData()
    {
      foreach (var upgradable in upgradableByKind.Values)
        upgradable.ResetSaveData();
    }
  }
}