using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Tracks/Steps/ExampleTrophyRoadTrackStep", fileName = "ExampleTrophyRoadTrackStepSettings")]
  public class ExampleTrophyRoadTrackStepSettings : BaseTrackStepSettings
  {
    public override IBaseTrackStep GetTrackStep(IBaseTrack parentTrack, int indexInTrack)
      => new ExampleTrophyRoadTrackStep(this, (ExampleTrophyRoadTrack)parentTrack, indexInTrack);
  }
}