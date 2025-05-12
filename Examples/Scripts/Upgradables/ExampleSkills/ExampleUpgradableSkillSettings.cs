using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Upgradables/ExampleUpgradableSkillSettings", fileName = "ExampleUpgradableSkillSettings")]
  public sealed class ExampleUpgradableSkillSettings : UpgradableSettings<ExampleUpgradableSkillKind, ExampleUpgradableSkillLevelSettings>
  {
    public override UpgradableKind UpgradableKind => UpgradableKind.ExampleUpgradableSkill;
    public override IBaseUpgradable InstantiateUpgradable() => new ExampleUpgradableSkill(this);
  }
}