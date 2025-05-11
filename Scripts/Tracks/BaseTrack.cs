using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public interface IBaseTrack
  {
    public Action<(int previousValue, int newValue)> OnProgressPointsChanged { get; set; }
    public Action<(int previousValue, int newValue)> OnRewardPointsChanged { get; set; }
    public Action<int> OnStepReached { get; set; }
    public Action<int> OnStepCompleted { get; set; }
    public Action<RewardBundle> OnRewardBundleAvailable { get; set; }
    public Action<RewardBundle> OnRewardBundleClaimed { get; set; }

    public TrackSaveData GetSaveData();
    public void LoadSaveData(TrackSaveData saveData);
    public void ResetSaveData();

    public string DisplayName { get; }
    public int ProgressPoints { get; }
    public int RewardPoints { get; }

    public int CurrentStepIndex { get; }
    public int LastCompletedStepIndex { get; }

    public bool IsCurrentStepLast { get; }
    public bool IsCurrentStepFirst { get; }

    public int CurrentStepLowerBound { get; }
    public int CurrentStepUpperBound { get; }

    public void AdjustProgressPointsBy(int adjustBy, bool force = false);
    public void AdjustProgressPointsBy(int adjustBy);
    public void AdjustRewardPointsBy(int adjustBy);

    public Sprite ProgressPointsIcon { get; }
    public string ProgressPointsDisplayNameSingular { get; }
    public string ProgressPointsDisplayNamePlural { get; }

    public Sprite RewardPointsIcon { get; }
    public string RewardPointsDisplayNameSingular { get; }
    public string RewardPointsDisplayNamePlural { get; }

    public List<RewardBundle> GetAllAvailableMilestoneBundles();
    public List<MilestoneBundle> GetAllMilestoneBundles();
    public MilestoneBundle GetFirstMilestoneBundleWithStatus(RewardBundleStateEnum status);
    public MilestoneBundle GetLastMilestoneBundleWithStatus(RewardBundleStateEnum status);
    public MilestoneBundle GetUpperboundMilestoneBundle();
    public MilestoneBundle GetLowerboundMilestoneBundle();

    public Dictionary<TriggerEventKind, int> GetTriggerEventCounts();
  }

  public interface ITrack<out T_TrackSettings, out T_Step, out T_StepSettings> : IBaseTrack
    where T_TrackSettings : BaseTrackSettings<T_StepSettings>
    where T_Step : BaseTrackStep<T_StepSettings>
    where T_StepSettings : BaseTrackStepSettings
  {
    T_TrackSettings Settings { get; }
    IEnumerable<T_Step> Steps { get; }

    T_Step PreviousStep { get; }
    T_Step CurrentStep { get; }
    T_Step NextStep { get; }
    T_Step GetStepByIndex(int index);
  }

  public abstract class BaseTrack<T_TrackSettings, T_Step, T_StepSettings> : ITrack<T_TrackSettings, T_Step, T_StepSettings>
    where T_TrackSettings : BaseTrackSettings<T_StepSettings>
    where T_Step : BaseTrackStep<T_StepSettings>
    where T_StepSettings : BaseTrackStepSettings
  {


    public BaseTrack(T_TrackSettings settings)
    {
      Settings = settings;
      List<T_Step> steps = new();

      foreach (var stepSettings in Settings.steps)
      {
        if (stepSettings == null)
          throw new InvalidOperationException($"Failed to initialize Meta Track [{Settings.TrackKind}] ({Settings.displayName}): One of the Steps is empty.");

        T_Step step = (T_Step)stepSettings.GetTrackStep(this, steps.Count());
        step.OnRewardBundleAvailable += reward => OnRewardBundleAvailable?.Invoke(reward);
        step.OnRewardBundleClaimed += reward => OnRewardBundleClaimed?.Invoke(reward);
        steps.Add(step);
      }

      Steps = steps;
    }

    public T_TrackSettings Settings { get; private set; }
    public IEnumerable<T_Step> Steps { get; private set; }

    public string DisplayName => Settings.displayName;

    private int _internalProgressPoints = 0;
    public int ProgressPoints
    {
      get => _internalProgressPoints;
      protected set => _internalProgressPoints = Math.Max(0, value);
    }

    public bool rewardPointsEnabled = false;

    private int _internalRewardPoints = 0;
    public int RewardPoints
    {
      get => rewardPointsEnabled ? _internalRewardPoints : ProgressPoints;
      protected set => _internalRewardPoints = value;
    }

    private int _currentStepIndex = 0;
    public int CurrentStepIndex
    {
      get => _currentStepIndex;
      private set => _currentStepIndex = Math.Clamp(value, 0, Steps.Count() - 1);
    }

    private int _lastCompletedStepIndex = -1;
    public int LastCompletedStepIndex
    {
      get => _lastCompletedStepIndex;
      private set => _lastCompletedStepIndex = Math.Clamp(value, -1, CurrentStepIndex);
    }

    /*
    ** EVENTS
    */

    public Action<(int previousValue, int newValue)> OnProgressPointsChanged { get; set; }
    public Action<(int previousValue, int newValue)> OnRewardPointsChanged { get; set; }
    public Action<int> OnStepReached { get; set; }
    public Action<int> OnStepCompleted { get; set; }
    public Action<RewardBundle> OnRewardBundleAvailable { get; set; }
    public Action<RewardBundle> OnRewardBundleClaimed { get; set; }

    /*
    ** IMPLEMENTATION
    */
    // Progress Points / Reward Points

    public void AdjustProgressPointsBy(int adjustBy) => AdjustProgressPointsBy(adjustBy, false);
    public void AdjustProgressPointsBy(int adjustBy, bool force = false)
    {

      if (adjustBy < 0 && !Settings.progressPointsCanDecrease)
        throw new InvalidOperationException($"Progress Points for Meta Track [{Settings.TrackKind}] ({Settings.displayName}) are not allowed to decrease.");


      int oldValue = ProgressPoints;

      if (force)
        ProgressPoints += adjustBy;
      else
        // If not in force mode, cannot go below CurrentStepLowerBound
        ProgressPoints = Math.Max(ProgressPoints + adjustBy, CurrentStepLowerBound);

      if (ProgressPoints != oldValue)
      {
        OnProgressPointsChanged?.Invoke((oldValue, ProgressPoints));

        if (Settings.enableRewardPoints)
          AdjustRewardPointsBy(adjustBy);

        EvaluateProgression();
      }
    }

    public void AdjustRewardPointsBy(int adjustBy)
    {
      if (!Settings.enableRewardPoints)
        return;

      if (adjustBy < 0)
        adjustBy = 0;

      int oldValue = RewardPoints;
      RewardPoints += adjustBy;
      if (RewardPoints != oldValue)
      {
        OnRewardPointsChanged?.Invoke((oldValue, RewardPoints));
        EvaluateProgression();
      }
    }

    private void EvaluateProgression()
    {
      CurrentStep.EvaluateMilestones();

      if (TryChangeStep())
        CurrentStep.EvaluateMilestones();
    }

    public Sprite ProgressPointsIcon => Settings.progressPointsIcon;
    public string ProgressPointsDisplayNameSingular => Settings.progressPointsDisplayNameSingular;
    public string ProgressPointsDisplayNamePlural => Settings.progressPointsDisplayNamePlural;

    public Sprite RewardPointsIcon => rewardPointsEnabled ? Settings.rewardPointsIcon : ProgressPointsIcon;
    public string RewardPointsDisplayNameSingular => rewardPointsEnabled ? Settings.rewardPointsDisplayNameSingular : ProgressPointsDisplayNameSingular;
    public string RewardPointsDisplayNamePlural => rewardPointsEnabled ? Settings.rewardPointsDisplayNamePlural : ProgressPointsDisplayNamePlural;

    // Steps

    public T_Step PreviousStep => IsCurrentStepFirst ? null : Steps.ElementAt(CurrentStepIndex - 1);
    public T_Step CurrentStep => Steps.ElementAt(CurrentStepIndex);
    public T_Step NextStep => IsCurrentStepLast ? null : Steps.ElementAt(CurrentStepIndex + 1);
    public T_Step GetStepByIndex(int index) => (index < 0 || index >= Steps.Count()) ? null : Steps.ElementAt(index);

    public bool IsCurrentStepLast => CurrentStepIndex == Steps.Count() - 1;
    public bool IsCurrentStepFirst => CurrentStepIndex == 0;

    public int CurrentStepLowerBound => IsCurrentStepFirst ? 0 : PreviousStep.Settings.progressPointsUpperBound;
    public int CurrentStepUpperBound => CurrentStep.Settings.progressPointsUpperBound;

    protected bool TryChangeStep()
    {
      bool stepChangeHappened = false;

      while (CurrentStep.IsCompleted && !IsCurrentStepLast)
      {
        if (LastCompletedStepIndex != CurrentStepIndex)
        {
          LastCompletedStepIndex += 1;
          stepChangeHappened = true;
          OnStepCompleted?.Invoke(LastCompletedStepIndex);
        }

        if (!IsCurrentStepLast)
        {
          CurrentStepIndex += 1;
          OnStepReached?.Invoke(CurrentStepIndex);
        }
      }

      while (!IsCurrentStepFirst && ProgressPoints < CurrentStepLowerBound)
      {
        CurrentStepIndex -= 1;
        OnStepReached?.Invoke(CurrentStepIndex);
        stepChangeHappened = true;
      }

      return stepChangeHappened;
    }

    // Rewards

    public List<RewardBundle> GetAllAvailableMilestoneBundles()
    {
      List<RewardBundle> allBundles = new();

      foreach (var step in Steps)
        allBundles.AddRange(step.GetAllAvailableMilestoneBundles());

      return allBundles;
    }

    public List<MilestoneBundle> GetAllMilestoneBundles()
    {
      List<MilestoneBundle> allBundles = new();

      foreach (var step in Steps)
        allBundles.AddRange(step.GetAllMilestoneBundles());

      return allBundles.OrderBy(x => x.settings.pointsRequired).ToList();
    }

    public MilestoneBundle GetLowerboundMilestoneBundle()
      => GetAllMilestoneBundles().LastOrDefault(r => RewardPoints >= r.settings.pointsRequired);

    public MilestoneBundle GetUpperboundMilestoneBundle()
      => GetAllMilestoneBundles().FirstOrDefault(r => RewardPoints < r.settings.pointsRequired);

    public MilestoneBundle GetFirstMilestoneBundleWithStatus(RewardBundleStateEnum status)
      => GetAllMilestoneBundles().FirstOrDefault(r => r.state == status);

    public MilestoneBundle GetLastMilestoneBundleWithStatus(RewardBundleStateEnum status)
      => GetAllMilestoneBundles().LastOrDefault(r => r.state == status);

    // Stats

    public Dictionary<TriggerEventKind, int> GetTriggerEventCounts()
    {
      Dictionary<TriggerEventKind, int> aggregatedEventCounts = new();
      foreach (var step in Steps)
      {
        var stepStats = step.triggerEventCounts;
        foreach ((var eventKind, var count) in stepStats)
        {
          if (!aggregatedEventCounts.ContainsKey(eventKind))
            aggregatedEventCounts[eventKind] = 0;

          aggregatedEventCounts[eventKind] += count;
        }
      }

      return aggregatedEventCounts;
    }

    // Save & Load

    public TrackSaveData GetSaveData()
    {
      List<TrackStepSaveData> trackStepsSaveData = new();
      foreach (T_Step step in Steps)
        trackStepsSaveData.Add(step.GetSaveData());
      return new()
      {
        trackKind = Settings.TrackKind,
        progressPoints = ProgressPoints,
        rewardPoints = RewardPoints,
        trackStepsSaveData = trackStepsSaveData
      };
    }

    public void LoadSaveData(TrackSaveData saveData)
    {
      ProgressPoints = saveData.progressPoints;
      RewardPoints = saveData.rewardPoints;
      if (saveData.trackStepsSaveData.Count() != Steps.Count())
        throw new InvalidOperationException($"Cannot load save for track [{saveData.trackKind}]: steps count does not match (save has {saveData.trackStepsSaveData.Count}, settings have {Steps.Count()}.");

      for (int index = 0; index < Steps.Count(); index++)
        Steps.ElementAt(index).LoadSaveData(saveData.trackStepsSaveData[index]);

      EvaluateProgression();
    }

    public void ResetSaveData()
    {
      ProgressPoints = 0;
      RewardPoints = 0;

      for (int index = 0; index < Steps.Count(); index++)
        Steps.ElementAt(index).ResetSaveData();
    }
  }
}

