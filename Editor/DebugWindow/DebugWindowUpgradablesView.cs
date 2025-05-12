using System;
using System.Collections.Generic;
using System.Linq;
using MetaPackage;
using UnityEngine;
using UnityEngine.UIElements;
using static MetaPackageDebug.Utils;

namespace MetaPackageDebug
{
  public class DebugWindowUpgradablesView
  {
    public VisualElement BuildUpgradablesView()
    {
      try
      {
        VisualElement upgradablesView = new Box();

        upgradablesView.Add(new HelpBox("If you cannot find an entity, it's not configured properly in MetaManager", UnityEngine.UIElements.HelpBoxMessageType.Info));

        List<UpgradableKind> upgradableKinds = IBaseUpgradable.entityKindByUpgradableKind.Keys.ToList();
        List<string> upgradableKindsAsString = upgradableKinds.Select(x => x.ToString()).ToList();
        DropdownField upgradableKindDropdown = new("Selected upgradable", upgradableKindsAsString, upgradableKindsAsString[0]);
        upgradablesView.Add(upgradableKindDropdown);

        Dictionary<UpgradableKind, List<Enum>> entityKindsByUpgradableKind = new();
        Dictionary<(UpgradableKind, Enum), VisualElement> viewByKinds = new();
        Dictionary<(UpgradableKind, Enum), string> entityDropdownLabelByKind = new();

        var entityKindDropdown = new DropdownField("Selected entity");

        upgradablesView.Add(entityKindDropdown);
        foreach (UpgradableKind upgradableKind in upgradableKinds)
        {
          List<Enum> entityKinds = Enum.GetValues(IBaseUpgradable.entityKindByUpgradableKind[upgradableKind]).OfType<Enum>().ToList();
          entityKindsByUpgradableKind[upgradableKind] = entityKinds;

          foreach (Enum entityKind in entityKinds)
          {
            var data = UpgradableDataAccessor.Build(upgradableKind, entityKind);

            var upgradableView = BuildUpgradableView(data);
            upgradablesView.Add(upgradableView);
            SetVisible(upgradableView, upgradableKind.Equals(upgradableKinds[0]) && entityKind.Equals(entityKinds[0]));

            viewByKinds[(upgradableKind, entityKind)] = upgradableView;
            entityDropdownLabelByKind[(upgradableKind, entityKind)] = SanitizeForDropdown($"{entityKind} {(data != null ? $"({data.settings.displayName})" : "")}");
          }
        }

        entityKindDropdown.choices = entityKindsByUpgradableKind[upgradableKinds[0]].Select(entityKind => entityDropdownLabelByKind[(upgradableKinds[0], entityKind)]).ToList();
        entityKindDropdown.index = 0;

        upgradableKindDropdown.RegisterValueChangedCallback(e =>
        {
          UpgradableKind upgradableKind = upgradableKinds[upgradableKindDropdown.index];
          entityKindDropdown.choices = entityKindsByUpgradableKind[upgradableKind].Select(entityKind => entityDropdownLabelByKind[(upgradableKind, entityKind)]).ToList();
        });

        entityKindDropdown.RegisterValueChangedCallback(e =>
        {
          UpgradableKind selectedUpgradableKind = upgradableKinds[upgradableKindDropdown.index];
          Enum selectedEntityKind = entityKindsByUpgradableKind[selectedUpgradableKind][entityKindDropdown.index];
          foreach (((UpgradableKind upgradableKind, Enum entityKind), VisualElement value) in viewByKinds)
            SetVisible(value, upgradableKind.Equals(selectedUpgradableKind) && entityKind.Equals(selectedEntityKind));
        });

        return upgradablesView;
      }
      catch (Exception e)
      {
        Debug.LogException(e);
        return BuildErrorLabel("Failed to build upgradables view");
      }
    }

    private VisualElement BuildUpgradableView(UpgradableDataAccessor data)
    {
      VisualElement upgradableView = new Box();

      if (data == null)
        return upgradableView;

      var entityNameLabel = BuildLabel($"Entity name: {(string.IsNullOrWhiteSpace(data.settings.displayName) ? "[no display name]" : data.settings.displayName)}", 16);
      upgradableView.Add(entityNameLabel);

      var rarity = MetaManager.Instance.GetRaritySettings(data.settings.rarity);
      var entityRarityLabel = BuildLabel($"Rarity: {rarity.displayName}", 16);
      upgradableView.Add(entityRarityLabel);

      var unlockedByDefaultLabel = BuildLabel($"Unlocked by default? {(data.settings.unlockedByDefault ? "yes" : "no")}", marginTop: 0, marginBottom: 0);
      upgradableView.Add(unlockedByDefaultLabel);

      var isUnlockedLabel = BuildLabel("", marginTop: 0, marginBottom: 0);
      BindLabel(isUnlockedLabel, () => $"Currently unlocked? {(data.upgradable.IsUnlocked ? "yes" : "no")}");
      upgradableView.Add(isUnlockedLabel);

      var unlockButton = new Button(() => data.upgradable.Unlock());
      BindActionToElement(unlockButton, () =>
      {
        bool isUnlocked = data.upgradable.IsUnlocked;
        unlockButton.text = isUnlocked ? "Entity is unlocked" : "Unlock entity";
        SetVisible(unlockButton, !isUnlocked);
      });
      upgradableView.Add(unlockButton);

      var totalExperiencePointsLabel = BuildLabel("", 16);
      BindLabel(totalExperiencePointsLabel, () => $"Total experience points: {data.upgradable.Experience}");
      upgradableView.Add(totalExperiencePointsLabel);

      var levelProgressPointsView = BuildLevelProgressPointsView(data);
      upgradableView.Add(levelProgressPointsView);

      var levelNavigationView = BuildLevelNavigationView(data);
      upgradableView.Add(levelNavigationView);

      var currentLevelCostsLabel = BuildLabel("");
      BindLabel(currentLevelCostsLabel,
        () => $@"Cost for next upgrade:
        {string.Join("\n", data.GetUpgradableCosts()
            .Select(cost =>
            {
              Currency currency = MetaManager.Instance.GetCurrency(cost.currencyReference);
              return $"{cost.quantity}x {(cost.quantity == 1 ? currency.GetDisplayNameSingular() : currency.GetDisplayNamePlural())}";
            })
          )}"
      );
      upgradableView.Add(currentLevelCostsLabel);

      return upgradableView;
    }

    private VisualElement BuildLevelProgressPointsView(UpgradableDataAccessor data)
    {
      var experiencePointsView = new VisualElement();

      var currentLevelExperienceBoundsContainer = new VisualElement();
      currentLevelExperienceBoundsContainer.style.flexDirection = FlexDirection.Row;
      currentLevelExperienceBoundsContainer.style.marginTop = 5;
      experiencePointsView.Add(currentLevelExperienceBoundsContainer);

      var currentLevelExperienceLowerBound = BuildLabel("", marginTop: 0, marginBottom: 0);
      BindLabel(currentLevelExperienceLowerBound, () => $"{data.GetPreviousLevelXpToNextLevel()}");
      currentLevelExperienceLowerBound.style.flexGrow = 1;
      currentLevelExperienceLowerBound.style.minWidth = 100;
      currentLevelExperienceLowerBound.style.unityTextAlign = TextAnchor.LowerLeft;
      currentLevelExperienceBoundsContainer.Add(currentLevelExperienceLowerBound);

      var currentLevelLabel = BuildLabel("", fontSize: 16, marginTop: 0, marginBottom: 0);
      BindLabel(currentLevelLabel, () => $"Level {data.upgradable.Level} / {data.upgradable.MaxLevel}");
      currentLevelLabel.style.flexGrow = 1;
      currentLevelLabel.style.minWidth = 100;
      currentLevelLabel.style.unityTextAlign = TextAnchor.LowerCenter;
      currentLevelExperienceBoundsContainer.Add(currentLevelLabel);

      var currentLevelExperienceUpperBound = BuildLabel("", marginTop: 0, marginBottom: 0);
      BindLabel(currentLevelExperienceUpperBound, () => $"{data.GetCurrentLevelXpToNextLevel()}");
      currentLevelExperienceUpperBound.style.flexGrow = 1;
      currentLevelExperienceUpperBound.style.minWidth = 100;
      currentLevelExperienceUpperBound.style.unityTextAlign = TextAnchor.LowerRight;
      currentLevelExperienceBoundsContainer.Add(currentLevelExperienceUpperBound);

      var levelProgressBar = new ProgressBar()
      {
        lowValue = 0f,
        highValue = 1f,
      };
      BindProgressBar(levelProgressBar,
        () => data.upgradable.ExperienceRelativeToLevel,
        () => $"Relative experience ({data.upgradable.ExperienceRelativeToLevel} / {data.GetCurrentLevelXpToNextLevel()})"
      );
      experiencePointsView.Add(levelProgressBar);

      return experiencePointsView;
    }

    private VisualElement BuildLevelNavigationView(UpgradableDataAccessor data)
    {
      var navigationView = new VisualElement();
      var experiencePointsContainer = new VisualElement();
      experiencePointsContainer.style.flexDirection = FlexDirection.Row;
      experiencePointsContainer.style.marginTop = 5;

      var experiencePointsField = new IntegerField("Experience");
      experiencePointsField.style.flexGrow = 1;
      experiencePointsField.style.minWidth = 100;
      experiencePointsField.labelElement.style.minWidth = 100;
      experiencePointsField.labelElement.style.flexBasis = 100;
      experiencePointsField.value = 0;
      experiencePointsField.RegisterValueChangedCallback(x => experiencePointsField.value = Math.Max(x.newValue, 0));
      experiencePointsContainer.Add(experiencePointsField);

      var increaseButton = new Button(() => data.upgradable.IncreaseExperience(experiencePointsField.value));
      increaseButton.text = "Increase";
      experiencePointsContainer.Add(increaseButton);
      navigationView.Add(experiencePointsContainer);

      var levelChangeContainer = new VisualElement();
      levelChangeContainer.style.flexDirection = FlexDirection.Row;
      levelChangeContainer.style.marginTop = 5;

      var prevLevelButton = new Button(() => data.upgradable.ForceSetLevel(data.upgradable.Level - 1));
      prevLevelButton.text = "Prev Level";
      prevLevelButton.style.flexGrow = 1;
      levelChangeContainer.Add(prevLevelButton);

      var nextLevelButton = new Button(() => data.upgradable.ForceSetLevel(data.upgradable.Level + 1));
      nextLevelButton.text = "Next Level";
      nextLevelButton.style.flexGrow = 1;
      levelChangeContainer.Add(nextLevelButton);

      navigationView.Add(levelChangeContainer);

      return navigationView;
    }

    private class UpgradableDataAccessor
    {
      public UpgradableKind upgradableKind;
      public Enum entityKind;
      public IBaseUpgradable upgradable;
      public InternalUpgradableSettings settings;
      public Func<int> GetPreviousLevelXpToNextLevel;
      public Func<int> GetCurrentLevelXpToNextLevel;
      public Func<List<UpgradableUpgradeCost>> GetUpgradableCosts;

      private UpgradableDataAccessor(UpgradableKind upgradableKind, Enum entityKind)
      {
        this.upgradableKind = upgradableKind;
        upgradable = MetaManager.Instance.GetUpgradable(upgradableKind, entityKind);
        if (upgradable == null)
          return;
        settings = GetProp<InternalUpgradableSettings>(upgradable, "Settings");
        GetPreviousLevelXpToNextLevel = () =>
        {
          var previousLevelSettings = GetProp<UpgradableLevelSettings>(upgradable, "PreviousLevelSettings");
          if (previousLevelSettings == null)
            return 0;

          return GetField<int>(previousLevelSettings, "experienceToNextLevel");
        };
        GetCurrentLevelXpToNextLevel = () => GetField<int>(GetProp<UpgradableLevelSettings>(upgradable, "CurrentLevelSettings"), "experienceToNextLevel");
        GetUpgradableCosts = () => GetField<List<UpgradableUpgradeCost>>(GetProp<UpgradableLevelSettings>(upgradable, "CurrentLevelSettings"), "costsToUpgrade");
      }

      public static UpgradableDataAccessor Build(UpgradableKind upgradableKind, Enum entityKind)
      {
        var upgradable = MetaManager.Instance.GetUpgradable(upgradableKind, entityKind);
        if (upgradable == null)
          return null;
        return new UpgradableDataAccessor(upgradableKind, entityKind);
      }
    }
  }
}