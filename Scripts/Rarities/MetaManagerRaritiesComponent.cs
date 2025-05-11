using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerRaritiesComponent : MonoBehaviour
  {
    [SerializeField] private RaritiesSettings raritiesSettings;
    public ReadOnlyDictionary<RarityKind, RaritySettings> raritySettingsByKind;

    void Awake() => Initialize();
    void OnEnable() => Initialize();

    private bool hasBeenInitialized = false;
    public void Initialize()
    {
      if (hasBeenInitialized)
        return;

      Dictionary<RarityKind, RaritySettings> dict = new();

      if (!raritiesSettings || raritiesSettings.raritySettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No rarity settings set ({name}).");
        raritySettingsByKind = new(dict);
        return;
      }

      foreach (var raritySettings in raritiesSettings.raritySettingsList)
      {
        var instantiatedRaritySettings = ScriptableObject.Instantiate(raritySettings);
        dict[raritySettings.kind] = instantiatedRaritySettings;
      }
      raritySettingsByKind = new(dict);

      hasBeenInitialized = true;
    }
  }
}