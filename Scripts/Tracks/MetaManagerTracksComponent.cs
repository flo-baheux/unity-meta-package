using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class MetaManagerTracksComponent : MonoBehaviour
  {
    [SerializeField] private TracksSettings tracksSettings;
    private ReadOnlyDictionary<TrackKind, IBaseTrack> metaTrackByKeyName;
    public List<TrackKind> ConfiguredTrackKinds => metaTrackByKeyName.Keys.ToList();

    private bool hasBeenInitialized = false;
    public void Initialize()
    {
      if (hasBeenInitialized)
        return;

      Dictionary<TrackKind, IBaseTrack> dict = new();

      if (!tracksSettings || tracksSettings.trackSettingsList.Count == 0)
      {
        Debug.LogWarning($"[Meta Manager] - No track settings are set ({name}).");
        metaTrackByKeyName = new(dict);
        return;
      }

      foreach (var trackSettings in tracksSettings.trackSettingsList)
      {
        var instanciatedTrackSettings = ScriptableObject.Instantiate(trackSettings);

        IBaseTrack track = instanciatedTrackSettings.InstantiateTrack();
        dict[instanciatedTrackSettings.TrackKind] = track;

        track.OnProgressPointsChanged += param => OnProgressPointsChanged?.Invoke(instanciatedTrackSettings.TrackKind, param);
        track.OnRewardPointsChanged += param => OnRewardPointsChanged?.Invoke(instanciatedTrackSettings.TrackKind, param);
        track.OnStepReached += param => OnStepStarted?.Invoke(instanciatedTrackSettings.TrackKind, param);
        track.OnStepCompleted += param => OnStepCompleted?.Invoke(instanciatedTrackSettings.TrackKind, param);
        track.OnRewardBundleAvailable += param => OnRewardBundleAvailable?.Invoke(instanciatedTrackSettings.TrackKind, param);
        track.OnRewardBundleClaimed += param => OnRewardBundleClaimed?.Invoke(instanciatedTrackSettings.TrackKind, param);
      }
      metaTrackByKeyName = new(dict);

      hasBeenInitialized = true;
    }

    public Action<TrackKind, (int previousValue, int newValue)> OnProgressPointsChanged;
    public Action<TrackKind, (int previousValue, int newValue)> OnRewardPointsChanged;
    public Action<TrackKind, int> OnStepStarted;
    public Action<TrackKind, int> OnStepCompleted;
    public Action<TrackKind, RewardBundle> OnRewardBundleAvailable;
    public Action<TrackKind, RewardBundle> OnRewardBundleClaimed;

    public T GetTrack<T>(TrackKind key) where T : IBaseTrack
    {
      if (!metaTrackByKeyName.TryGetValue(key, out IBaseTrack metaTrack))
        throw new KeyNotFoundException($"Meta track with key {key} not found. Please make sure all tracks have their key properly set.");

      if (metaTrack is T typedTrack)
        return typedTrack;

      throw new InvalidCastException($"Meta track with key {key} is not of type {typeof(T)}.");
    }

    public TracksSaveData GetSaveData() => new()
    {
      tracksSaveData = metaTrackByKeyName.Values.Select(track => track.GetSaveData()).ToList(),
    };

    public void LoadSaveData(TracksSaveData saveData)
    {
      foreach (TrackSaveData trackSaveData in saveData.tracksSaveData)
        if (metaTrackByKeyName.TryGetValue(trackSaveData.trackKind, out IBaseTrack track))
          track.LoadSaveData(trackSaveData);
    }

    public void ResetSaveData()
    {
      foreach (var track in metaTrackByKeyName.Values)
        track.ResetSaveData();
    }
  }
}