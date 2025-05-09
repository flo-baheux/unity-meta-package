namespace MetaPackage
{
  public sealed class ExampleUpgradableSkill : Upgradable<ExampleUpgradableSkillKind, ExampleUpgradableSkillSettings, ExampleUpgradableSkillLevelSettings>
  {
    public ExampleUpgradableSkill(ExampleUpgradableSkillSettings settings) : base(settings)
    {
    }
  }
}