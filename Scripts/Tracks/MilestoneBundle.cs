using System;

namespace MetaPackage
{
  public class MilestoneBundle : RewardBundle
  {
    public MilestoneBundleSettings settings;

    public MilestoneBundle(MilestoneBundleSettings settings)
    : base(settings.rewards)
    {
      this.settings = settings;
    }

    public MilestoneBundleSaveData GetSaveData()
      => new()
      {
        ID = settings.ID,
        state = state
      };

    public void LoadSaveData(MilestoneBundleSaveData saveData)
    {
      if (saveData.ID != settings.ID)
        throw new InvalidOperationException($"ID mismatch: expected {settings.ID} but got {saveData.ID}.");

      state = saveData.state;
    }

    public void ResetSaveData() {
      state = RewardBundleStateEnum.Locked;
    }

    public override string ToString()
      => $"(ID={settings.ID}) - {base.ToString()}";
  }
}
