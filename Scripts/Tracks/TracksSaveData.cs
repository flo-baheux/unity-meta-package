using System;
using System.Collections.Generic;

namespace MetaPackage
{
  // TODO: Make it easy to extend?
  [Serializable]
  public class TracksSaveData
  {
    public List<TrackSaveData> tracksSaveData;
  }

  [Serializable]
  public class TrackSaveData
  {
    public TrackKind trackKind;
    public int progressPoints;
    public int rewardPoints;
    public List<TrackStepSaveData> trackStepsSaveData;
  }

  [Serializable]
  public class TrackStepSaveData
  {
    public List<MilestoneBundleSaveData> milestonesSaveData;
    public List<TriggeredRewardBundleSaveData> triggersSaveData;
    public List<EventCountSaveData> eventCountData;
  }

  [Serializable]
  public class MilestoneBundleSaveData
  {
    public string ID;
    public RewardBundleStateEnum state;
  }

  [Serializable]
  public class TriggeredRewardBundleSaveData
  {
    public RewardBundleStateEnum state;
    public TriggeredBundleSettings settings;
  }

  [Serializable]
  public class EventCountSaveData
  {
    public TriggerEventKind eventName;
    public int count;
  }
}