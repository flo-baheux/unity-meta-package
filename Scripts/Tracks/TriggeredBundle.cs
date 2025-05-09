namespace MetaPackage
{
  public class TriggeredBundle : RewardBundle
  {
    TriggeredBundleSettings settings;

    public TriggeredBundle(TriggeredBundleSettings settings)
    : base(settings.rewards)
    {
      this.settings = settings;
    }

    public TriggeredBundle(TriggeredRewardBundleSaveData saveData)
    : base(saveData.settings.rewards)
    {
      settings = saveData.settings;
      state = saveData.state;
    }


    public TriggeredRewardBundleSaveData GetSaveData()
      => new()
      {
        state = state,
        settings = settings,
      };
  }
}
