using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetaPackage
{
  public class ChestRewardResult
  {
    public int quantity;
    public UpgradableRewardData rewardData;
  };

  public class UpgradeChestReward : BaseReward
  {
    public UpgradeChestReward(RewardSettings settings) : base(settings)
    { }

    public List<ChestRewardResult> claimedRewards;

    public override void Claim()
    {
      claimedRewards = new();
      Dictionary<Enum, int> nbRewardsByKind = new();
      Dictionary<Enum, UpgradableRewardData> rewardDataByKind = new();

      UpgradeChestSettings upgradeChestSettings = MetaManager.Instance.GetUpgradeChestSettings(settings.upgradeChestKind);

      if (upgradeChestSettings.isCustom)
      {
        foreach (var customUpgradeChestData in upgradeChestSettings.customUpgradeChestDatas)
        {
          var entityKind = customUpgradeChestData.upgradable.EntityKindAsEnum;
          nbRewardsByKind[entityKind] = customUpgradeChestData.quantity;
          rewardDataByKind[entityKind] = customUpgradeChestData.upgradable.RewardData;
        }
      }
      else
      {
        var rarityUpgradeChestDatas = upgradeChestSettings.rarityUpgradeChestDatas;
        var rarityChestDataByRarity = rarityUpgradeChestDatas.ToDictionary(x => x.rarity, x => x);

        List<UpgradableRewardData> eligiblesRewardData = MetaManager.Instance.GetUpgradableEntitiesEligibleForReward(upgradeChestSettings.upgradableKind);
        rewardDataByKind = eligiblesRewardData.ToDictionary(e => e.entityKind, e => e);

        var eligiblesRewardDataByRarityLookup = eligiblesRewardData.ToLookup(e => e.rarityKind, e => e);

        // To apply weights in a way that evenly spread the odds if no actors are available for a given rarity, 
        // we setup a "bag". In it, we add each eligible actorKind n times, where n is rarity weight * 10.
        // If you have A, B and C and want 60% chance to draw A, 30% chance to get B and 10% to get C,
        // fill the bag with 6*A, 3*B and 1*C. Each time you draw one, you get the correct probability.
        // If you don't add C, this will automatically renormalize the probabilities.
        List<UpgradableRewardData> probabilityBag = new();
        foreach (var rarityChestData in rarityUpgradeChestDatas)
        {
          int nbToAddInBag = Mathf.CeilToInt(rarityChestData.weight * 10);
          var eligiblesForGivenRarity = eligiblesRewardDataByRarityLookup[rarityChestData.rarity];
          if (eligiblesForGivenRarity == null || eligiblesForGivenRarity.Count() == 0)
            continue;

          var rewardablesForGivenRarity = eligiblesForGivenRarity.ToList();
          for (int i = 0; i < nbToAddInBag; i++)
            probabilityBag.Add(eligiblesForGivenRarity.ElementAt(UnityEngine.Random.Range(0, eligiblesForGivenRarity.Count())));
        }

        // In case of missconfiguration, it's possible to end up in an infinite loop.
        // This prevents an unexpected Unity crash.
        int failSafeCount = 0;
        int failSafeMax = 1000;

        int rewardCount = 0;
        int nbDifferentRewardKindPicked = 0;
        // The probability bag being setup, we want to limit the number of different actors
        // that can be rewarded. We roll, but if that limit is reached, we redo the roll until
        // we pick one that was already picked.
        while (failSafeCount++ < failSafeMax && rewardCount < upgradeChestSettings.maxRewards)
        {
          var pickedInBagRewardData = probabilityBag[UnityEngine.Random.Range(0, probabilityBag.Count())];

          if (nbRewardsByKind.ContainsKey(pickedInBagRewardData.entityKind))
            nbRewardsByKind[pickedInBagRewardData.entityKind] += 1;
          else
          {
            if (nbDifferentRewardKindPicked >= upgradeChestSettings.maxOfEachReward)
              continue; // Kind already picked - restarting current roll
            nbDifferentRewardKindPicked++;
            nbRewardsByKind[pickedInBagRewardData.entityKind] = 1;
          }

          rewardCount += Math.Clamp(rarityChestDataByRarity[pickedInBagRewardData.rarityKind].rewardCountValue, 0, upgradeChestSettings.maxRewards - rewardCount);
        }

        if (failSafeCount == failSafeMax)
          Debug.LogError($"Something went wrong while picking rewards for chest {settings.upgradeChestKind}.");
      }

      foreach (var (kind, quantity) in nbRewardsByKind)
      {
        MetaManager.Instance.GetUpgradable(upgradeChestSettings.upgradableKind, kind).IncreaseExperience(quantity);
        claimedRewards.Add(new()
        {
          rewardData = rewardDataByKind[kind],
          quantity = quantity
        });
      }
    }

    public override Sprite GetSprite() => MetaManager.Instance.GetUpgradeChestSettings(settings.upgradeChestKind).icon;
    public Sprite GetTopSprite() => MetaManager.Instance.GetUpgradeChestSettings(settings.upgradeChestKind).iconTop;
    public Sprite GetBottomSprite() => MetaManager.Instance.GetUpgradeChestSettings(settings.upgradeChestKind).iconBottom;
    public override string GetText() => MetaManager.Instance.GetUpgradeChestSettings(settings.upgradeChestKind).displayName;
  }
}
