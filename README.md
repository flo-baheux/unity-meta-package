# unity-meta-package
A set of configurable system features for meta progression. 
Includes: 
- currencies
- rarities
- upgradables (skills, heroes, any entity with levels)
- tracks (leagues, account level, any progression with points, steps and rewards)
- rewards
- upgradable chests (random distribution of experience for upgradables depending on their rarity)

by Florian "Laywelin" Baheux: http://www.laywelin.com

Note: 
Not a "Unity Package" because for now it requires you to edit files from the package (enums).
UI components have a dependency on DOTWeen, feel free to comment out the code, use it, or remove the UI folder if you're not using it!

# Setup info

## How to add package in project

1. Download [latest package version](https://github.com/flo-baheux/unity-meta-package/releases/latest)
2. Import inside your Unity project, within Assets folder
3. Place the `MetaManager` prefab in your scene (It contains a MetaManager singleton)

## Entities

The meta package encompass multiple entities. While most of the time it only requires updating an enum, some entities need more setup. Details below.
Each entity has a Scriptable Object (SO) settings list, available in `MetaPackage/Settings` 

<aside>

The `Examples` folder contains one folder for each entity with default settings. It is recommended that you remove the examples after your setup is done.

</aside>

## Currencies

Currencies can be used for anything in the game. However, by default, currencies can be rewarded via track rewards, set as cost to upgrade an upgradable, and spent for said upgrades.

### How to setup

To define a new currency, you need to:

1. add an entry in the  `CurrencyKind` enum inside `MetaPackage/Scripts/Currencies/CurrencyKind.cs`
2. Create a new CurrencySettings SO (right click in Project - `Create/MetaPackage/Currency`) and set the Currency Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Settings/Currencies`

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

1. add an entry in the `RarityKind` enum inside `MetaPackage/Scripts/Rarities/RarityKind.cs`
2. Create a new RaritySettings SO (right click in Project - `Create/MetaPackage/Rarity`) and set the Rarity Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Settings/Rarities`

The rarity is now available from anywhere in the project.

A default set of rarities are already setup as an example and can be used:
Uncommon, Common, Rare, Epic, Legendary

It’s of course possible to remove rarities from the RaritiesSettings list and RarityKind enum to restrict the list.

```csharp
RaritySettings raritySettings = MetaManager.Instance.GetRaritySettings(RarityKind.Rare);
```

## Chests

Chests are a mean to reward experience points to upgradables, in a randomly fashion.

### How to setup

To add a new chest, you need to:

1. add an entry in the `ChestKind` enum inside `MetaPackage/Scripts/Chests/ChestKind.cs`
2. Create a new ChestSettings SO (right click in Project - `Create/MetaPackage/Chest`) and set the Chest Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Settings/Chests`

The chest is now available from anywhere in the project.

A default Normal chest is already setup as an example and can be used.

```csharp
ChestSettings chestSettings = MetaManager.Instance.GetChestSettings(ChestKind.Normal);
```

## Tracks / Steps

For simplicity's sake, let's say you want to create a `CustomTrack` track.
To define a new track and associated steps, you need to:

1. Add an entry in the `TrackKind` enum inside `MetaPackage/Scripts/Tracks/TrackKind.cs`
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
7. Add the created TrackSettings SO inside the list in `MetaPackage/Settings/Tracks`

The track is now available from anywhere in the project.

```csharp
Track track = MetaManager.Instance.GetTrack<CustomTrack>(TrackKind.CustomTrack);
```

All default capabilities [can be found here](./Scripts/Tracks/BaseTrack.cs)

## Upgradables

For simplicity's sake, let's say you want to create an `UpgradableWeapon` weapon.
To define a new upgradable kind and be able to define a list of upgradables, you need to:

1. Add an entry in the `UpgradableKind` enum inside `MetaPackage/Scripts/Upgradables/UpgradableKind.cs`
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
8. Add the created UpgradableCategorySettings SO inside the list in `MetaPackage/Settings/UpgradableCategories`

The upgradable is now available from anywhere in the project.

```csharp
  UpgradableFireSword upgradableFireSword = MetaManager.Instance.GetUpgradable<UpgradableFireSword>(
    UpgradableKind.UpgradableWeapon,
    UpgradableWeaponKind.FireSword
  );
```

All default capabilities [can be found here](./Scripts/Upgradables/UpgradableEntity.cs)
