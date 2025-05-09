using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public interface IBaseTrackStep { }

  public interface ITrackStep<out T_StepSettings> : IBaseTrackStep
  {
    T_StepSettings Settings { get; }
    public int indexInTrack { get; }
  };

  public class BaseTrackStep<T_StepSettings> : ITrackStep<T_StepSettings>
    where T_StepSettings : BaseTrackStepSettings
  {
    public BaseTrackStep(T_StepSettings settings, ITrack<BaseTrackSettings<T_StepSettings>, BaseTrackStep<T_StepSettings>, T_StepSettings> parentTrack, int indexInTrack)
    {
      Settings = settings;
      this.parentTrack = parentTrack;
      this.indexInTrack = indexInTrack;

      foreach (var milestoneSettings in Settings.milestones)
      {
        MilestoneBundle milestone = new(milestoneSettings);
        milestone.OnAvailable += m => OnRewardBundleAvailable?.Invoke(m);
        milestone.OnClaimed += m => OnRewardBundleClaimed?.Invoke(m);
        milestoneRewardBundles.Add(milestone);
      }
    }

    public T_StepSettings Settings { get; private set; }
    public ITrack<BaseTrackSettings<T_StepSettings>, BaseTrackStep<T_StepSettings>, T_StepSettings> parentTrack { get; protected set; }
    public int indexInTrack { get; protected set; }

    public string DisplayName => Settings.displayName;
    public List<MilestoneBundle> milestoneRewardBundles = new();
    public List<TriggeredBundle> triggerRewardBundles = new();
    public Dictionary<TriggerEventKind, int> triggerEventCounts = new();

    public Action<RewardBundle> OnRewardBundleAvailable;
    public Action<RewardBundle> OnRewardBundleClaimed;

    public int ProgressPointsLowerBound => indexInTrack == 0 ? 0 : parentTrack.GetStepByIndex(indexInTrack - 1).Settings.progressPointsUpperBound;
    public int ProgressPointsUpperBound => Settings.progressPointsUpperBound;

    public bool IsCompleted => parentTrack.ProgressPoints >= Settings.progressPointsUpperBound;
    public bool HasBeenReached => parentTrack.ProgressPoints >= ProgressPointsLowerBound;
    public bool IsInProgress => HasBeenReached && !IsCompleted;

    /*
    ** IMPLEMENTATION
    */

    public float ProgressPointsProgress01
    {
      get
      {
        float current = parentTrack.ProgressPoints - ProgressPointsLowerBound;
        float max = Settings.progressPointsUpperBound - ProgressPointsLowerBound;
        return Mathf.Clamp01(current / max);
      }
    }

    public float RewardPointsProgress01
    {
      get
      {
        float current = parentTrack.RewardPoints - ProgressPointsLowerBound;
        float max = Settings.progressPointsUpperBound - ProgressPointsLowerBound;
        return Mathf.Clamp01(current / max);
      }
    }

    // Rewards

    public List<RewardBundle> GetAllAvailableMilestoneBundles()
    {
      List<RewardBundle> allBundles = new();
      allBundles.AddRange(milestoneRewardBundles);
      allBundles.AddRange(triggerRewardBundles);
      return allBundles.FindAll(r => r.state == RewardBundleStateEnum.Available);
    }

    public List<MilestoneBundle> GetAllMilestoneBundles() => new(milestoneRewardBundles);

    public MilestoneBundle GetFirstMilestoneBundleWithStatus(RewardBundleStateEnum status)
      => GetAllMilestoneBundles().FirstOrDefault(r => r.state == status);

    public MilestoneBundle GetLowerboundMilestoneBundleForCurrentPoints()
      => GetAllMilestoneBundles().LastOrDefault(r => parentTrack.RewardPoints >= r.settings.pointsRequired);

    public MilestoneBundle GetUpperboundMilestoneBundleForCurrentPoints()
      => GetAllMilestoneBundles().FirstOrDefault(r => parentTrack.RewardPoints < r.settings.pointsRequired);

    public MilestoneBundle GetLastMilestoneBundleWithStatus(RewardBundleStateEnum status)
      => GetAllMilestoneBundles().LastOrDefault(r => r.state == status);

    public List<RewardSettings> GetRewardsForEvent(TriggerEventKind triggerEvent)
    {
      TriggeredBundleSettings? triggeredBundleSettings = Settings.triggers.Find(x => x.eventCondition == triggerEvent);
      if (triggeredBundleSettings == null)
        return null;

      return triggeredBundleSettings.Value.rewards;
    }

    public RewardSettings GetSpecificRewardForEvent(TriggerEventKind triggerEvent, Predicate<RewardSettings> rewardPredicate)
    {
      var eventRewards = GetRewardsForEvent(triggerEvent);
      if (eventRewards == null)
        return null;

      return eventRewards.Find(rewardPredicate);
    }

    public void EvaluateMilestones()
    {
      int points = parentTrack.RewardPoints;

      foreach (var milestoneRewardBundle in milestoneRewardBundles)
        if (points >= milestoneRewardBundle.settings.pointsRequired)
          milestoneRewardBundle.TrySetAvailable();
    }

    public TriggeredBundle ActivateEventTrigger(TriggerEventKind triggerEvent)
    {
      int rewardSettingIndex = Settings.triggers.FindIndex(t => t.eventCondition == triggerEvent);
      if (rewardSettingIndex == -1)
        return null;

      triggerEventCounts[triggerEvent] = triggerEventCounts.TryGetValue(triggerEvent, out int count) ? count + 1 : 1;
      var triggeredRewardBundleSettings = Settings.triggers[rewardSettingIndex];

      TriggeredBundle rewardBundle = new(triggeredRewardBundleSettings);
      rewardBundle.OnAvailable += (r) => OnRewardBundleAvailable?.Invoke(r);
      rewardBundle.OnClaimed += (r) => OnRewardBundleClaimed?.Invoke(r);
      triggerRewardBundles.Add(rewardBundle);
      rewardBundle.TrySetAvailable();
      return rewardBundle;
    }

    public virtual TrackStepSaveData GetSaveData()
    {
      List<MilestoneBundleSaveData> milestonesSaveData = new();
      List<TriggeredRewardBundleSaveData> triggersSaveData = new();

      foreach (var milestoneRewardBundle in milestoneRewardBundles)
        milestonesSaveData.Add(milestoneRewardBundle.GetSaveData());

      foreach (var triggerRewardBundle in triggerRewardBundles)
        triggersSaveData.Add(triggerRewardBundle.GetSaveData());

      return new()
      {
        milestonesSaveData = milestonesSaveData,
        triggersSaveData = triggersSaveData,
        eventCountData = triggerEventCounts
          .Select(x => new EventCountSaveData { eventName = x.Key, count = x.Value })
          .ToList(),
      };
    }

    public virtual void LoadSaveData(TrackStepSaveData saveData)
    {
      var milestonesAsDict = milestoneRewardBundles.ToDictionary(x => x.settings.ID, x => x);

      foreach (var milestoneSaveData in saveData.milestonesSaveData)
        if (milestonesAsDict.ContainsKey(milestoneSaveData.ID))
          milestonesAsDict[milestoneSaveData.ID].LoadSaveData(milestoneSaveData);

      triggerRewardBundles = new();
      foreach (var triggerSaveData in saveData.triggersSaveData)
        triggerRewardBundles.Add(new(triggerSaveData));

      triggerEventCounts = saveData.eventCountData.ToDictionary(x => x.eventName, x => x.count);
    }

    public void ResetSaveData()
    {
      foreach (var milestone in milestoneRewardBundles)
        milestone.ResetSaveData();

      triggerRewardBundles = new();
      triggerEventCounts = new();
    }
  }
}
