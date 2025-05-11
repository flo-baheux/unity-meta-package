namespace MetaPackage
{
  public sealed class ExampleLeagueTrack : BaseTrack<ExampleLeagueTrackSettings, ExampleLeagueTrackStep, ExampleLeagueTrackStepSettings>
  {
    public ExampleLeagueTrack(ExampleLeagueTrackSettings settings) : base(settings)
    { }

    public static ExampleLeagueTrack GetLeagueTrack() => MetaManager.Instance.GetTrack<ExampleLeagueTrack>(TrackKind.ExampleLeagueTrack);
  }
}
