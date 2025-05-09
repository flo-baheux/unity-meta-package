using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Tracks/ExampleLeagueTrack", fileName = "ExampleLeagueTrackSettings")]
  public sealed class ExampleLeagueTrackSettings : BaseTrackSettings<ExampleLeagueTrackStepSettings>
  {
    public override IBaseTrack InstantiateTrack() => new ExampleLeagueTrack(this);
    public override TrackKind TrackKind => TrackKind.ExampleLeagueTrack;
  }
}