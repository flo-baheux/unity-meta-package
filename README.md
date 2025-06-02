# unity-meta-package
Configurable meta progression game system. Includes: 
- currencies
- rarities
- upgradables (skills, heroes, etc.)
- tracks (leagues, account level, etc.)
- rewards
- upgradable chests (XP distribution based on rarity)

Note: 
Not a "Unity Package" per se - requires you to edit enum files.

[Design note](http://www.laywelin.com/Meta-Package-Initial-design-draft-1ee058cee40180e880dbea7dcb5a2186)

by [Florian "Laywelin" Baheux](http://www.laywelin.com)

# Setup info

## How to add package in your project

1. [Download latest release](https://github.com/flo-baheux/unity-meta-package/releases/latest)
2. Import into `Assets/`
3. Add `MetaManager` prefab to your scene

`MetaPackage/Databases`: contains ScriptableObject (SO) config lists and entity kind enums.
`MetaPackage/Examples`: Example setup per entity. Recommended to remove after your setup is done.

## Currencies

Used for costs and rewards. Can be locked/unlocked, capped or not, and spent.

### How to setup

To define a new currency, you need to:

1. Edit `CurrencyKind` enum in `MetaPackage/Databases/CurrencyKind.cs`
2. Create CurrencySettings SO (right click in Project - `Create > MetaPackage > Currency`) and set CurrencyKind in settings
3. Add created SO to `MetaPackage/Databases/CurrencyDatabase`

The currency is now available from anywhere in the project.
A default currency `ExampleCoins` can be found as an example.

```csharp
Currency currency = MetaManager.Instance.GetCurrency(CurrencyKind.Coins);

currency.Unlock();
currency.AdjustQuantity(-10);
currency.GetDisplayNameSingular(); // "Coin"
currency.GetDisplayNamePlural(); // "Coins" (fallback to singular if left empty)
currency.GetSingleIcon(); // Sprite for one coin
currency.GetMultipleIcon(); // Sprite for n coins (fallback to single if left empty)

currency.OnQuantityChanged;
currency.OnUnlocked;

MetaManager.OnCurrencyQuantityChanged;
MetaManager.OnCurrencyUnlocked;
```

## Rarities

Rarities are linked to upgradables, as an upgradable have settings and levels based on rarity.
(For instance, Rare upgradables can be configured with 5 levels and specific costs to upgrade)

### How to setup

To add a new rarity, you need to: 

1. Edit `RarityKind` enum in `MetaPackage/Databases/RarityKind.cs`
2. Create RaritySettings SO (right click in Project - `Create > MetaPackage > Rarity`) and set RarityKind in settings
3. Add created SO to `MetaPackage/Databases/RarityDatabase`

The rarity is now available from anywhere in the project.
A default set of rarities can be found as examples: Common, Rare, Epic

```csharp
RaritySettings raritySettings = MetaManager.Instance.GetRaritySettings(RarityKind.Rare);
```

## Upgrade Chests

Give XP to upgradables (configurable weighted algorithm) based on rarity.

### How to setup

To add a new chest, you need to:

1. Edit `ChestKind` enum in `MetaPackage/Databases/ChestKind.cs`
2. Create ChestSettings SO (right click in Project - `Create > MetaPackage > UpgradeChest`) and set UpgradeChestKind accordingly in settings
3. Add created SO to `MetaPackage/Databases/UpgradeChestDatabase`

The chest is now available from anywhere in the project.
A default Normal chest is already setup as an example.

```csharp
ChestSettings chestSettings = MetaManager.Instance.GetUpgradeChestSettings(UpgradeChestKind.Normal);
```

## Tracks / Steps

Handles meta progression (account level, leagues, etc.).

It's possible to extend tracks and steps settings, as well as tracks and steps instances, allowing one to add features and settings.
A track and a step have "Settings" which are what's configured in the editor, and an instance with live data.

### Concepts

**Track:** Progress path

**Step:** Level/Stage inside track (Leagues, Arenas, etc.)

**Progress Point (PP):** XP-like value. Can increase or decrease.

**Reward Points (RP):** XP-like value adjacent used to reach milestones. Cannot decrease. If disabled, fallsback to PP.

**Milestone:** A bundle of rewards that gets available and can be claimed upon reaching points threshold.

**Trigger:** Event that gets manually triggered to get a Bundle. Main example is Victory / Defeat

**Bundle:** Set of rewards. Can be retrieved and claimed via milestones and triggers.
Has a state: `not_available`, `available`, or `claimed`. A bundle contains **n** rewards and can only be claimed **once**.

**Reward:** Track points, currency, chest, unlock, etc. Can be extended for custom rewards.

### How to setup

For simplicity's sake, let's say you want to create a `CustomTrack` track.
To define a new track and associated steps, you need to:

1. Edit `TrackKind` enum in `MetaPackage/Databases/TrackKind.cs`
2. Copy from `MetaPackage/Examples/Scripts/Tracks/ExampleLeague`
3. Replace all `ExampleLeague`

example for ExampleLeagueTrack:
```csharp
// FROM
  public sealed class ExampleLeagueTrack : BaseTrack<ExampleLeagueTrackSettings, ExampleLeagueTrackStep, ExampleLeagueTrackStepSettings>
  {
    public ExampleLeagueTrack(ExampleLeagueTrackSettings settings) : base(settings)
    { }

    public static ExampleLeagueTrack GetLeagueTrack() => MetaManager.Instance.GetTrack<ExampleLeagueTrack>(TrackKind.ExampleLeagueTrack);
  }

// TO
  public sealed class CustomTrack : BaseTrack<CustomTrackSettings, CustomTrackStep, CustomTrackStepSettings>
  {
    public CustomTrack(CustomTrackSettings settings) : base(settings) { }
    public static CustomTrack GetLeagueTrack() => MetaManager.Instance.GetTrack<CustomTrack>(TrackKind.CustomTrack);
  }
```

4. Create a new TrackSettings SO (right click in Project - `Create > MetaPackage > Tracks > CustomTrackSettings`)
5. Create a new TrackStepSettings SO (right click in Project - `Create > MetaPackage > Tracks > Steps > CustomTrackStepSettings`)
6. Inside the created TrackSettings SO, add a step and link the TrackStepSettings SO
7. Add the created TrackSettings SO inside the list in `MetaPackage/Databases/TrackDatabase`

The track is now available from anywhere in the project.

```csharp
Track track = MetaManager.Instance.GetTrack<CustomTrack>(TrackKind.CustomTrack);
```

All default capabilities [can be found here](./Scripts/Tracks/BaseTrack.cs)

## Upgradables

Entities with rarity + levels. Highly extendable.
Levels have an XP requirement and cost to upgrade. Upgrade requires a manual action.
Upgrade XP is provided via chests or custom scripting and is optional.
Upgrade cost using Currencies.

An upgradable cannot "downgrade". Can be locked/unlocked by default, not eligible for rewards if locked.
Can be set non-eligible for rewards.

### How to setup

For simplicity's sake, let's say you want to create an `UpgradableWeapon` weapon.
To define a new upgradable kind and be able to define a list of upgradables, you need to:

1. Edit `UpgradableKind` enum in `MetaPackage/Databases/UpgradableKind.cs`
2. Copy from `MetaPackage/Examples/Scripts/Upgradables/ExampleSkills`
3. Replace all `ExampleUpgradableSkill`

example for ExampleUpgradableSkill:
```csharp
// FROM
  public sealed class ExampleUpgradableSkill : Upgradable<ExampleUpgradableSkillKind, ExampleUpgradableSkillSettings, ExampleUpgradableSkillLevelSettings>
  {
    public ExampleUpgradableSkill(ExampleUpgradableSkillSettings settings) : base(settings)
    {
    }
  }

// TO
  public sealed class UpgradableWeapon : Upgradable<UpgradableWeaponKind, UpgradableWeaponSettings, UpgradableWeaponLevelSettings>
  {
    public UpgradableWeapon(ExampleUpgradableSkillSettings settings) : base(settings)
    {
    }
  }
```

4. Create a new UpgradableCategorySettings SO (right click in Project - `Create > MetaPackage > Upgradables > UpgradableWeaponCategorySettings`)
5. Create a new UpgradableSettings SO (right click in Project - `Create > MetaPackage > Tracks > Steps > UpgradableWeaponSettings`)
6. Inside the created UpgradableCategorySettings SO, link the new UpgradableSettings SO
7. Inside the created UpgradableSettings SO, link the new UpgradableCategorySettings SO (It's a two way relationship!)
8. Add the created UpgradableCategorySettings SO inside the list in `MetaPackage/Databases/UpgradableCategories`

The upgradable is now available from anywhere in the project.

```csharp
  UpgradableFireSword upgradableFireSword = MetaManager.Instance.GetUpgradable<UpgradableFireSword>(
    UpgradableKind.UpgradableWeapon,
    UpgradableWeaponKind.FireSword
  );
```

All default capabilities [can be found here](./Scripts/Upgradables/Upgradable.cs)

# Extending the package

The package is made to be easy to edit and add project specific features. 
Here is a non-exhaustive section to help you with it.

## Adding a new reward kind

Reminder: A reward is something that can be granted once reashing a threshold in a track.
It's also possible to design game systems that grants rewards manually.

1. Edit `RewardKind` enum in `MetaPackage/Scripts/Rewards/RewardKind.cs`
2. Make sure the settings you want are available within `MetaPackage/Scripts/Rewards/RewardSettings.cs`
3. Create a new reward extending `BaseReward` 

Example:
```csharp
  public class UpgradableExperienceReward : BaseReward
  {
    public UpgradableExperienceReward(RewardSettings settings) : base(settings)
    { }

    public override void Claim()
    {
      var upgradable = MetaManager.Instance.GetUpgradable(settings.upgradable.UpgradableKind, settings.upgradable.EntityKindAsEnum);
      upgradable.IncreaseExperience(settings.quantity);
    }

    public override Sprite GetSprite() => settings.upgradable.icon;

    public override string GetText() => $"{settings.quantity}xp for {settings.upgradable.displayName}";
  }
```

4. Add a new case in `RewardFactory` in `MetaPackage/Scripts/Rewards/RewardFactory.cs`