namespace MetaPackage
{
  public sealed class ExampleTrophyRoadTrack : BaseTrack<ExampleTrophyRoadTrackSettings, ExampleTrophyRoadTrackStep, ExampleTrophyRoadTrackStepSettings>
  {
    public ExampleTrophyRoadTrack(ExampleTrophyRoadTrackSettings settings) : base(settings)
    { }

    public static ExampleTrophyRoadTrack GetLeagueTrack() => MetaManager.Instance.GetTrack<ExampleTrophyRoadTrack>(TrackKind.ExampleTrophyRoadTrack);
  }
}
