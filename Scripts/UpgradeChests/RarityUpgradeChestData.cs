using System;

namespace MetaPackage
{
  [Serializable]
  public class RarityUpgradeChestData
  {
    public RarityKind rarity;
    [HelpBox("How much weight for this rarity when picking rewards")]
    public float weight;
    [HelpBox("How much it contributes towards Max Rewards / Max Of Each Reward")]
    public int rewardCountValue;
  }


}