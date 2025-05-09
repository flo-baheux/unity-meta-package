using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  public abstract class InternalBaseTrackSettings : ScriptableObject
  {
    [Header("Track name")]
    public string displayName;

    [Header("Progress Points (PP)")]
    [HelpBox("Represents player's progression in the track.\nThose points are used to climb up steps, and by default to unlock rewards.", HelpBoxMessageType.Info)]
    [CustomLabel("PP display name (singular)")]
    public string progressPointsDisplayNameSingular;
    [CustomLabel("PP display name (plural)")]
    public string progressPointsDisplayNamePlural;
    [CustomLabel("PP icon")]
    public Sprite progressPointsIcon;
    [CustomLabel("PP can decrease")]
    public bool progressPointsCanDecrease;

    [Header("Reward Points (RP)")]
    [HelpBox("If enabled, will use RP to unlock rewards instead of PP.\nIncrease with PP but cannot decrease.\nWhen disabled, reward points = progress points.", HelpBoxMessageType.Info)]
    public bool enableRewardPoints;

    [ShowIf("enableRewardPoints", true)]
    [CustomLabel("RP display name (singular)")]
    public string rewardPointsDisplayNameSingular;
    [ShowIf("enableRewardPoints", true)]
    [CustomLabel("RP display name (plural)")]
    public string rewardPointsDisplayNamePlural;
    [ShowIf("enableRewardPoints", true)]
    [CustomLabel("RP icon")]
    public Sprite rewardPointsIcon;

    public abstract TrackKind TrackKind { get; }
    public abstract IBaseTrack InstantiateTrack();
  }

  public abstract class BaseTrackSettings<T_StepSettings> : InternalBaseTrackSettings
    where T_StepSettings : BaseTrackStepSettings
  {
    [Header("Steps")]
    [HelpBox("Usually represents levels, arenas, leagues, etc.\n", HelpBoxMessageType.Info)]
    public List<T_StepSettings> steps;
  }
}