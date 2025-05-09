# unity-meta-package
A packaged set of configurable system features for meta progression. Includes currencies, rarities, upgradables (skills, heroes, any entity with levels), tracks (leagues, account level, any progression with points, steps and rewards), rewards, chests with random distribution of experience for upgradables depending on their rarity.

by Florian "Laywelin" Baheux: http://www.laywelin.com

# Setup info

## How to add package in project

1. Download latest package version [ADD LINK]
2. Import inside Unity project
3. Place `MetaManager` prefab (root of package folder) in scene

## Entities

The meta package encompass multiple entities. While most of the time it only requires updating an enum, some entities need more setup. Details below.
Each entity has a Scriptable Object settings list, available in `MetaPackage/Settings` 

<aside>

The `Examples` folder contains one folder for each entity with default settings. It is recommended that you remove the examples after your setup is done.

</aside>

## Currencies

Currencies can be used for anything in the game. However, by default, currencies can be rewarded via track rewards, set as cost to upgrade an upgradable, and spent for said upgrades.

### How to setup

To define a new currency, you need to:

1. add an entry in the  `CurrencyKind` enum inside `MetaPackage/Scripts/Currencies/CurrencyKind.cs`
2. Create a new CurrencySettings SO (right click in Project - `Create/MetaPackage/Currency`) and set the Currency Kind accordingly
3. Add the newly created SO inside the list in `MetaPackage/Settings/CurrenciesSettings`

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
3. Add the newly created SO inside the list in `MetaPackage/Settings/RaritiesSettings`

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
3. Add the newly created SO inside the list in `MetaPackage/Settings/ChestsSettings`

The chest is now available from anywhere in the project.

A default Normal chest is already setup as an example and can be used.

```csharp
ChestSettings chestSettings = MetaManager.Instance.GetChestSettings(ChestKind.Normal);
```

## Tracks / Steps

WIP

## Upgradables

WIP