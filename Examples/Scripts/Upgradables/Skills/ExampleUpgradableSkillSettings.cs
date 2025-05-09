using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Upgradables/ExampleSkillSettings", fileName = "ExampleSkillSettings")]
  public sealed class ExampleUpgradableSkillSettings : UpgradableSettings<ExampleUpgradableSkillKind, ExampleUpgradableSkillLevelSettings>
  {
    public override UpgradableKind UpgradableKind => UpgradableKind.ExampleSkills;
    public override IBaseUpgradable InstantiateUpgradable() => new ExampleUpgradableSkill(this);
  }
}