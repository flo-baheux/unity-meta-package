using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerSaveComponent : MetaManagerComponent
  {
    public bool optimizeSaveFile = false;
    public static string SaveFilePath => $"{Application.persistentDataPath}/meta.data";
    private bool isLoadingSave = false;

    MetaManagerTracksComponent tracksComponent;
    MetaManagerUpgradablesComponent upgradablesComponent;
    MetaManagerCurrenciesComponent currenciesComponent;

    protected override void Setup()
    {
      tracksComponent = GetComponent<MetaManagerTracksComponent>();
      upgradablesComponent = GetComponent<MetaManagerUpgradablesComponent>();
      currenciesComponent = GetComponent<MetaManagerCurrenciesComponent>();

      tracksComponent.OnProgressPointsChanged += (_, _) => Save();
      tracksComponent.OnRewardPointsChanged += (_, _) => Save();
      tracksComponent.OnStepStarted += (_, _) => Save();
      tracksComponent.OnStepCompleted += (_, _) => Save();
      tracksComponent.OnRewardBundleAvailable += (_, _) => Save();
      tracksComponent.OnRewardBundleClaimed += (_, _) => Save();

      upgradablesComponent.OnUnlock += (_, _) => Save();
      upgradablesComponent.OnLevelChanged += (_, _) => Save();
      upgradablesComponent.OnUpgradeAvailable += (_, _) => Save();
      upgradablesComponent.OnExperienceIncreased += (_, _) => Save();

      currenciesComponent.OnCurrencyQuantityChanged += (_, _) => Save();
      currenciesComponent.OnCurrencyUnlocked += (_) => Save();
    }

    public void Save()
    {
      if (isLoadingSave)
        return;

      MetaSaveData saveData = new()
      {
        tracksSaveData = tracksComponent.GetSaveData(),
        upgradablesSaveData = upgradablesComponent.GetSaveData(),
        currenciesSaveData = currenciesComponent.GetSaveData(),
      };

      try
      {
        if (optimizeSaveFile)
          File.WriteAllBytes(SaveFilePath, SaveDataToByteArray(saveData));
        else
          File.WriteAllText(SaveFilePath, JsonUtility.ToJson(saveData));
      }
      catch (Exception e)
      {
        Debug.LogError("[Meta Manager] - Failed to save data. Error: " + e.Message);
      }
    }

    public void LoadSave()
    {
      if (!File.Exists(SaveFilePath))
      {
        Debug.LogWarning($"[Meta Manager] - No save file found ({SaveFilePath})");
        return;
      }

      isLoadingSave = true;

      try
      {
        MetaSaveData saveData = null;
        if (optimizeSaveFile)
        {
          byte[] rawSaveData = File.ReadAllBytes(SaveFilePath);
          saveData = ByteArrayToSaveData(rawSaveData);
        }
        else
        {
          string stringSaveData = File.ReadAllText(SaveFilePath);
          saveData = JsonUtility.FromJson<MetaSaveData>(stringSaveData);
        }

        tracksComponent.LoadSaveData(saveData.tracksSaveData);
        upgradablesComponent.LoadSaveData(saveData.upgradablesSaveData);
        currenciesComponent.LoadSaveData(saveData.currenciesSaveData);

        isLoadingSave = false;
        Save();
      }
      catch (Exception e)
      {
        isLoadingSave = false;
        Debug.LogError("[Meta Manager] - Failed to load save data. Error: " + e.Message);
        Debug.LogException(e);
      }
    }


    private byte[] SaveDataToByteArray(MetaSaveData saveData)
    {
      string json = JsonUtility.ToJson(saveData);
      byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

      using MemoryStream outputStream = new();
      using (GZipStream gz = new(outputStream, CompressionMode.Compress))
      {
        gz.Write(jsonBytes, 0, jsonBytes.Length);
      }
      return outputStream.ToArray();
    }

    private MetaSaveData ByteArrayToSaveData(byte[] rawSaveData)
    {
      using (MemoryStream compressedStream = new(rawSaveData))
      using (GZipStream gz = new(compressedStream, CompressionMode.Decompress))
      using (MemoryStream decompressedStream = new())
      {
        gz.CopyTo(decompressedStream);
        decompressedStream.Position = 0;

        string json = System.Text.Encoding.UTF8.GetString(decompressedStream.ToArray());
        MetaSaveData saveData = JsonUtility.FromJson<MetaSaveData>(json);
        return saveData;
      }
    }

    public void LiveResetSave()
    {
      DeleteSave();
      tracksComponent.ResetSaveData();
      upgradablesComponent.ResetSaveData();
      currenciesComponent.ResetSaveData();
    }

    public static void DeleteSave()
      => File.Delete(SaveFilePath);

    public static void OpenSaveFolder()
      => Application.OpenURL("file:///" + Path.GetDirectoryName(SaveFilePath));
  }
}