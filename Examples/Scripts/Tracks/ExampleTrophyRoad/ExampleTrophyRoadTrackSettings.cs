using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Tracks/ExampleTrophyRoadTrack", fileName = "ExampleTrophyRoadTrackSettings")]
  public sealed class ExampleTrophyRoadTrackSettings : BaseTrackSettings<ExampleTrophyRoadTrackStepSettings>
  {
    public override IBaseTrack InstantiateTrack() => new ExampleTrophyRoadTrack(this);
    public override TrackKind TrackKind => TrackKind.ExampleTrophyRoadTrack;
  }
}