using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MetaPackage
{
  [RequireComponent(typeof(MetaManagerUpgradeChestsComponent))]
  [RequireComponent(typeof(MetaManagerTracksComponent))]
  [RequireComponent(typeof(MetaManagerRaritiesComponent))]
  [RequireComponent(typeof(MetaManagerUpgradablesComponent))]
  [RequireComponent(typeof(MetaManagerCurrenciesComponent))]
  [RequireComponent(typeof(MetaManagerSaveComponent))]
  public class MetaManager : MonoBehaviour
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ClearStaticRefs()
    => instance = null;

    private static MetaManager instance;

    public static MetaManager Instance
    {
      get
      {
        if (instance == null)
        {
          instance = FindObjectOfType<MetaManager>();
          instance.Initialize();
        }
        return instance;
      }
      private set => instance = value;
    }

    private MetaManagerUpgradeChestsComponent chestsComponent;
    private MetaManagerRaritiesComponent raritiesComponent;
    private MetaManagerTracksComponent tracksComponent;
    private MetaManagerUpgradablesComponent upgradablesComponent;
    private MetaManagerCurrenciesComponent currenciesComponent;
    private MetaManagerSaveComponent saveComponent;

    private bool isInitialized = false;

    private void Awake()
    {
      Initialize();
    }

    private void Initialize()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }
      if (isInitialized)
        return;

      chestsComponent = GetComponent<MetaManagerUpgradeChestsComponent>();
      raritiesComponent = GetComponent<MetaManagerRaritiesComponent>();
      tracksComponent = GetComponent<MetaManagerTracksComponent>();
      upgradablesComponent = GetComponent<MetaManagerUpgradablesComponent>();
      currenciesComponent = GetComponent<MetaManagerCurrenciesComponent>();
      saveComponent = GetComponent<MetaManagerSaveComponent>();

      chestsComponent.Initialize();
      raritiesComponent.Initialize();
      tracksComponent.Initialize();
      upgradablesComponent.Initialize();
      currenciesComponent.Initialize();
      saveComponent.Initialize();

      tracksComponent.OnProgressPointsChanged += (trackKind, param)
        => OnProgressPointsChanged?.Invoke(trackKind, param);

      tracksComponent.OnRewardPointsChanged += (trackKind, param)
        => OnRewardPointsChanged?.Invoke(trackKind, param);

      tracksComponent.OnStepStarted += (trackKind, param)
        => OnStepReached?.Invoke(trackKind, param);

      tracksComponent.OnStepCompleted += (trackKind, param)
        => OnStepCompleted?.Invoke(trackKind, param);

      tracksComponent.OnRewardBundleAvailable += (trackKind, param)
        => OnRewardBundleAvailable?.Invoke(trackKind, param);

      tracksComponent.OnRewardBundleClaimed += (trackKind, param)
        => OnRewardBundleClaimed?.Invoke(trackKind, param);


      upgradablesComponent.OnUnlock += (upgradableKind, entityKind)
        => OnUpgradableUnlock?.Invoke(upgradableKind, entityKind);

      upgradablesComponent.OnLevelChanged += (upgradableKind, entityKind)
        => OnUpgradableLevelChanged?.Invoke(upgradableKind, entityKind);

      upgradablesComponent.OnUpgradeAvailable += (upgradableKind, entityKind)
        => OnUpgradableUpgradeAvailable?.Invoke(upgradableKind, entityKind);

      upgradablesComponent.OnExperienceIncreased += (upgradableKind, entityKind)
        => OnUpgradableExperienceIncreased?.Invoke(upgradableKind, entityKind);


      currenciesComponent.OnCurrencyQuantityChanged += (currencyKind, oldNewValues)
        => OnCurrencyQuantityChanged?.Invoke(currencyKind, oldNewValues);

      currenciesComponent.OnCurrencyUnlocked += (currencyKind)
        => OnCurrencyUnlocked?.Invoke(currencyKind);

      saveComponent.LoadSave();

      isInitialized = true;
    }

    /*
    ** RARITIES
    */

    public RaritySettings GetRaritySettings(RarityReference rarityReference) => raritiesComponent.GetRaritySettings(rarityReference);
    public IReadOnlyList<RaritySettings> GetAllRaritySettings() => raritiesComponent.GetAllRaritySettings();

    /*
    ** CHESTS
    */

    public UpgradeChestSettings GetUpgradeChestSettings(UpgradeChestKind kind) => chestsComponent.upgradeChestSettingsByKind[kind];

    /*
    ** TRACKS
    */

    public Action<TrackKind, (int previousValue, int newValue)> OnProgressPointsChanged;
    public Action<TrackKind, (int previousValue, int newValue)> OnRewardPointsChanged;
    public Action<TrackKind, int> OnStepReached;
    public Action<TrackKind, int> OnStepCompleted;
    public Action<TrackKind, RewardBundle> OnRewardBundleAvailable;
    public Action<TrackKind, RewardBundle> OnRewardBundleClaimed;

    /// <summary>
    /// Generic method to retrieve a typed Track.
    /// Recommended to use over default GetTrack() when retrieving a specific track.
    /// </summary>
    /// <returns>A typed Track.</returns>
    public T GetTrack<T>(TrackKind key) where T : IBaseTrack
      => tracksComponent.GetTrack<T>(key);

    /// <summary>
    /// Default method to retrieve a IBaseTrack track.
    /// If you are retrieving a specific track, it is recommended to use GetTrack<TypedTrack>.
    /// </summary>
    /// <returns>A generic track. Instance can be type checked.</returns>
    public IBaseTrack GetTrack(TrackKind key)
      => tracksComponent.GetTrack<IBaseTrack>(key);
    /*
    ** UPGRADABLE ENTITIES
    */

    public Action<UpgradableKind, Enum> OnUpgradableUnlock;
    public Action<UpgradableKind, Enum> OnUpgradableLevelChanged;
    public Action<UpgradableKind, Enum> OnUpgradableExperienceIncreased;
    public Action<UpgradableKind, Enum> OnUpgradableUpgradeAvailable;

    /// <summary>
    /// Default method to retrieve a IBaseUpgradable upgradable entity.
    /// If you are retrieving a specific upgradable entity, tt is recommended to use GetUpgradable<TypedUpgradable>.
    /// </summary>
    /// <returns>A generic upgradable entity. Instance can be type checked.</returns>
    public IBaseUpgradable GetUpgradable(UpgradableKind upgradableKind, Enum entityKind)
      => upgradablesComponent.GetUpgradable<IBaseUpgradable>(upgradableKind, entityKind);

    /// <summary>
    /// Generic method to retrieve a typed Upgradable.
    /// Recommended to use over default GetUpgradable() when retrieving a specific upgradable entity.
    /// </summary>
    /// <returns>A typed Upgradable.</returns>
    public T GetUpgradable<T>(UpgradableKind upgradableKind, Enum entityKind) where T : IBaseUpgradable
      => upgradablesComponent.GetUpgradable<T>(upgradableKind, entityKind);

    /// <summary>
    /// Generic method to retrieve a typed Category Settings.
    /// </summary>
    /// <returns>A typed Category Settings.</returns>
    public T GetUpgradableCategorySettings<T>(UpgradableKind upgradableKind) where T : BaseUpgradableCategorySettings
      => upgradablesComponent.GetCategorySettings<T>(upgradableKind);


    public List<UpgradableRewardData> GetUpgradableEntitiesEligibleForReward(UpgradableKind upgradableKind)
      => upgradablesComponent.GetAllEligibeForReward(upgradableKind);

    /*
    ** CURRENCIES
    */

    public Action<CurrencyReference, (int oldValue, int newValue)> OnCurrencyQuantityChanged;
    public Action<CurrencyReference> OnCurrencyUnlocked;

    /// <summary>
    /// Method to retrieve a defined currency.
    /// </summary>
    /// <returns>A Currency instance.</returns>
    public Currency GetCurrency(CurrencyReference currencyReference) => currenciesComponent.GetCurrency(currencyReference);
    public IReadOnlyList<CurrencySettings> GetAllCurrencySettings() => currenciesComponent.GetAllCurrencySettings();

    /* 
    ** GLOBAL
    */

    public void LiveResetSave()
      => saveComponent.LiveResetSave();

    public static void DeleteSave()
      => MetaManagerSaveComponent.DeleteSave();

    public static void OpenSaveFolder()
      => MetaManagerSaveComponent.OpenSaveFolder();

#if UNITY_EDITOR
    public void Reset()
    {
      isInitialized = false;
      chestsComponent?.Reset();
      raritiesComponent?.Reset();
      tracksComponent?.Reset();
      upgradablesComponent?.Reset();
      currenciesComponent?.Reset();
      saveComponent?.Reset();
    }
#endif
  }

#if UNITY_EDITOR
  [InitializeOnLoad]
  public static class ManagerEditorReset
  {
    static ManagerEditorReset()
    {
      EditorApplication.delayCall += () =>
      {
        MetaManager.Instance.Reset();
        MetaManager.ClearStaticRefs();
      };
    }
  }
#endif
}