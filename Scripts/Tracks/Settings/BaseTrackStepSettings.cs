using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  public abstract class BaseTrackStepSettings : ScriptableObject
  {
    public string displayName;
    [CustomLabel("Total points required to complete")]
    [HelpBox("Move to next step when Progress Points reaches this value.")]
    public int progressPointsUpperBound;

    [Space(20)]
    [HelpBox("Milestones are rewards unlocked (only once) when condition is met.", HelpBoxMessageType.Info)]
    public List<MilestoneBundleSettings> milestones;

    [Space(20)]
    [HelpBox("Triggers are rewards unlocked when the specified event is triggered.", HelpBoxMessageType.Info)]
    public List<TriggeredBundleSettings> triggers;

    public abstract IBaseTrackStep GetTrackStep(IBaseTrack parentTrack, int indexInTrack);


    [SerializeField, HideInInspector] private int _milestonePreviousCount = 0;
    public void OnValidate()
    {
      if (_milestonePreviousCount < milestones.Count)
        for (int i = _milestonePreviousCount; i < milestones.Count; i++)
          milestones[i].Init();
      _milestonePreviousCount = milestones.Count;

      milestones.ForEach(m => m.Refresh());
    }
  }
}