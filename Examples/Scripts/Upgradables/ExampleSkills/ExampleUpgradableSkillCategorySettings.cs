using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Upgradables/ExampleUpgradableSkillCategorySettings", fileName = "ExampleUpgradableSkillCategorySettings")]
  public sealed class ExampleUpgradableSkillCategorySettings : UpgradableCategorySettings<ExampleUpgradableSkillKind, ExampleUpgradableSkillLevelSettings>
  {
    public override UpgradableKind upgradableKind { get => UpgradableKind.ExampleUpgradableSkill; }
  }
}