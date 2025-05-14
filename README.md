# unity-meta-package
A set of configurable system features for meta progression. 
Includes: 
- currencies
- rarities
- upgradables (skills, heroes, any entity with levels)
- tracks (leagues, account level, any progression with points, steps and rewards)
- rewards
- upgradable chests (random distribution of experience for upgradables depending on their rarity)

Note: 
Not a "Unity Package" because for now it requires you to edit files from the package (enums).
UI components have a dependency on DOTWeen, feel free to comment out the code, use it, or remove the UI folder if you're not using it!

[Design note](http://www.laywelin.com/Meta-Package-Initial-design-draft-1ee058cee40180e880dbea7dcb5a2186)

by Florian "Laywelin" Baheux: http://www.laywelin.com

# Setup info

## How to add package in your project

1. Download [latest package version](https://github.com/flo-baheux/unity-meta-package/releases/latest)
2. Import inside your Unity project, within Assets folder
3. Place the `MetaManager` prefab in your scene (It contains a MetaManager singleton)

The meta package encompass multiple entities. While most of the time it only requires updating an enum, some entities need more setup. Details below.
Each entity has a Scriptable Object (SO) settings list, available in `MetaPackage/Databases` 

<aside>

The `Examples` folder contains one folder for each entity with default settings. It is recommended that you remove the examples after your setup is done.

</aside>

## Currencies

Currencies can be used for anything in the game. However, by default, currencies can be rewarded via track rewards, set as cost to upgrade an upgradable, and spent for said upgrades.
A currency can be locked or unlocked by default, can be unlocked via rewards, and have an optional quantity limit.

### How to setup

To define a new currency, you need to:

1. add an entry in the  `CurrencyKind` enum inside `MetaPackage/Databases/CurrencyKind.cs`
2. Create a new CurrencySettings SO (right click in Project - `Create/MetaPackage/Currency`) and set the Currency Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Databases/CurrencyDatabase`

The currency is now available from anywhere in the project.

A default currency “Coins” is already setup as an example and can be used.

```csharp
Currency currency = MetaManager.Instance.GetCurrency(CurrencyKind.Coins);
currency.Unlock(); // If not unlocked by default
currency.AdjustQuantity(-10); // positive/negative integer
currency.GetDisplayNameSingular(); // "Coin"
currency.GetDisplayNamePlural(); // "Coins" (fallback to singular if not set)
currency.GetSingleIcon(); // Sprite for one coin
currency.GetMultipleIcon(); // Sprite for n coins (fallback to single if not set)
currency.OnQuantityChanged; // Event (oldValue, newValue)
currency.OnUnlocked; // Event

MetaManager.OnCurrencyQuantityChanged; // Event (CurrencyKind, (oldValue, newValue))
MetaManager.OnCurrencyUnlocked; // Event (CurrencyKind)
```

## Rarities

Rarities are linked to upgradables, as an upgradable have settings and levels based on rarity.
(For instance, Rare upgradables can be configured with 5 levels and specific costs to upgrade)

### How to setup

To add a new rarity, you need to: 

1. add an entry in the `RarityKind` enum inside `MetaPackage/Databases/RarityKind.cs`
2. Create a new RaritySettings SO (right click in Project - `Create/MetaPackage/Rarity`) and set the Rarity Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Databases/RarityDatabase`

The rarity is now available from anywhere in the project.

A default set of rarities are already setup as an example and can be used:
Uncommon, Common, Rare, Epic, Legendary

It’s of course possible to remove rarities from the RarityDatabase list and RarityKind enum to restrict the list.

```csharp
RaritySettings raritySettings = MetaManager.Instance.GetRaritySettings(RarityKind.Rare);
```

## Chests

Chests are a mean to reward experience points to upgradables, in a randomly fashion.

### How to setup

To add a new chest, you need to:

1. add an entry in the `ChestKind` enum inside `MetaPackage/Databases/ChestKind.cs`
2. Create a new ChestSettings SO (right click in Project - `Create/MetaPackage/UpgradeChest`) and set the Chest Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Databases/UpgradeChestDatabase`

The chest is now available from anywhere in the project.

A default Normal chest is already setup as an example and can be used.

```csharp
ChestSettings chestSettings = MetaManager.Instance.GetUpgradeChestSettings(UpgradeChestKind.Normal);
```

## Tracks / Steps

A track represents progression, either as personal account growth (like experience points and account levels) or relative to other players (like leagues). 
A game may feature multiple meta tracks simultaneously, such as daily rewards combined with competitive leagues.

It's possible to extend tracks and steps settings, as well as tracks and steps instances, allowing one to add features and settings 
while keeping the base features related to progression and art integration.
A track and a step have "Settings" which are what's configured in the editor, and an instance with live data and methods.

**Progress Point (PP):** Points measuring player's progress, such as experience points or trophies. Can increase or decrease.
**Reward Points (RP) [Optional]:** If enabled, points used to unlock rewards. When PP increases, RP increases but never decreases. Bounds by current step's bounds. If disabled, fallsback to PP.
**Step:** Represents leagues, arenas, or levels. Each step can have milestones and/or triggers to get rewards.
**Milestone:** A bundle of rewards that gets unlocked when reaching the specified quantity of points.
**Trigger:** An event associated with a meta track. Triggers can occur when PP/RP crosses a threshold or when specific conditions are met. Triggers may grant rewards or advance players to the next step. They can be configured to automatically claim rewards or require manual claiming.
**Bundle:** A collection of reward items that can be retrieved and claimed via triggers or manually.
Has a state: `not_available`, `available`, or `claimed`. A bundle contains **n** rewards and can only be claimed **once**.
**Reward:** A reward is what's inside a bundle, and has settings allowing you to retrieve a display name, an icon, and the associated entity's settings.
Without package customization, can be: track points, currencies, currency unlock, upgrade chests, upgradable unlocks.

### How to setup

For simplicity's sake, let's say you want to create a `CustomTrack` track.
To define a new track and associated steps, you need to:

1. Add an entry in the `TrackKind` enum inside `MetaPackage/Databases/TrackKind.cs`
2. Copy paste the content of the `MetaPackage/Examples/Scripts/Upgradables/ExampleSkills` anywhere in your project
3. Edit the newly created files by replacing `ExampleLeague` everywhere with what suits you

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

4. Create a new TrackSettings SO (right click in Project - `Create/MetaPackage/Tracks/CustomTrackSettings`)
5. Create a new TrackStepSettings SO (right click in Project - `Create/MetaPackage/Tracks/Steps/CustomTrackStepSettings`)
6. Inside the created TrackSettings SO, add a step and link the TrackStepSettings SO
7. Add the created TrackSettings SO inside the list in `MetaPackage/Databases/TrackDatabase`

The track is now available from anywhere in the project.

```csharp
Track track = MetaManager.Instance.GetTrack<CustomTrack>(TrackKind.CustomTrack);
```

All default capabilities [can be found here](./Scripts/Tracks/BaseTrack.cs)

## Upgradables

An Upgradable is an entity that has a rarity and levels. 
Each level has an XP requirement and a cost to upgrade to the next level, and leveling up requires a manual upgrade action.
Upgrade XP can be given manually, or via chests and is optional.
Upgrade cost is related to currencies and is optional.

An upgradable cannot "downgrade".
It can be either locked or unlocked by default, and if locked cannot be in an upgrade chest (ignored for custom chests).
It can be eligible for reward or not (ignored for custom chests).

### How to setup

For simplicity's sake, let's say you want to create an `UpgradableWeapon` weapon.
To define a new upgradable kind and be able to define a list of upgradables, you need to:

1. Add an entry in the `UpgradableKind` enum inside `MetaPackage/Databases/UpgradableKind.cs`
2. Copy paste the content of the `MetaPackage/Examples/Scripts/Upgradables/ExampleSkills` anywhere in your project
3. Edit the newly created files by replacing `ExampleUpgradableSkill` everywhere with what suits you

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

4. Create a new UpgradableCategorySettings SO (right click in Project - `Create/MetaPackage/Upgradables/UpgradableWeaponCategorySettings`)
5. Create a new UpgradableSettings SO (right click in Project - `Create/MetaPackage/Tracks/Steps/UpgradableWeaponSettings`)
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