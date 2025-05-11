using System;
using System.Collections.Generic;
using System.Linq;
using MetaPackage;
using UnityEngine;
using UnityEngine.UIElements;

using static MetaPackageDebug.Utils;

namespace MetaPackageDebug
{
  public class DebugWindowTracksView
  {
    public VisualElement BuildTracksView()
    {
      VisualElement tracksView = new Box();

      try
      {
        tracksView.Add(new HelpBox("If you cannot find a track, it's not configured properly in MetaManager", UnityEngine.UIElements.HelpBoxMessageType.Info));

        var trackKinds = MetaManager.Instance.GetComponent<MetaManagerTracksComponent>().ConfiguredTrackKinds;
        var trackKindsAsString = trackKinds.Select(x => x.ToString()).ToList();

        var tracksDropdown = new DropdownField("Selected track", trackKindsAsString, trackKindsAsString[0]);
        tracksView.Add(tracksDropdown);

        Dictionary<TrackKind, VisualElement> viewByKind = new();
        foreach (var trackKind in trackKinds)
        {
          var trackView = BuildTrackView(trackKind);
          SetVisible(trackView, trackKind == trackKinds[0]);
          viewByKind[trackKind] = trackView;
          tracksView.Add(trackView);
        }

        tracksDropdown.RegisterValueChangedCallback(e =>
        {
          foreach ((Enum key, var value) in viewByKind)
            SetVisible(value, (TrackKind)key == (TrackKind)Enum.Parse(typeof(TrackKind), e.newValue));
        });

        return tracksView;
      }
      catch (Exception e)
      {
        Debug.LogException(e);
        return BuildErrorLabel("Failed to build tracks view");
      }
    }

    private VisualElement BuildTrackView(TrackKind trackKind)
    {

      VisualElement trackView = new Box();

      var data = new TrackDataAccessor(trackKind);

      var trackNameLabel = BuildLabel($"Track name: {(string.IsNullOrWhiteSpace(data.settings.displayName) ? "[no display name]" : data.settings.displayName)}", 16);
      trackView.Add(trackNameLabel);

      var stepListLabel = BuildLabel($"steps:\n{string.Join("\n", data.steps.Select(step => $"- {GetProp<string>(step, "DisplayName")}"))}");
      trackView.Add(stepListLabel);

      var trackProgressPoints = BuildLabel("");
      BindLabel(trackProgressPoints, () => $"Progress Points (PP): {data.track.ProgressPoints}");
      trackView.Add(trackProgressPoints);

      var progressPointsView = BuildTrackProgressPointsView(data);
      trackView.Add(progressPointsView);

      var navigationView = BuildTrackNavigationView(data);
      trackView.Add(navigationView);

      trackView.Add(BuildSeparator());

      var triggerEventsView = BuildTriggerEventsView(data);
      trackView.Add(triggerEventsView);

      return trackView;
    }

    private VisualElement BuildTrackProgressPointsView(TrackDataAccessor data)
    {
      var progressPointsView = new VisualElement();

      var trackCurrentStepBoundsContainer = new VisualElement();
      trackCurrentStepBoundsContainer.style.flexDirection = FlexDirection.Row;
      trackCurrentStepBoundsContainer.style.marginTop = 5;
      progressPointsView.Add(trackCurrentStepBoundsContainer);

      var trackCurrentStepLowerBound = BuildLabel("", marginTop: 0, marginBottom: 0);
      BindLabel(trackCurrentStepLowerBound, () => $"{data.GetCurrentStepLowerBound()}");
      trackCurrentStepLowerBound.style.flexGrow = 1;
      trackCurrentStepLowerBound.style.minWidth = 100;
      trackCurrentStepLowerBound.style.unityTextAlign = TextAnchor.LowerLeft;
      trackCurrentStepBoundsContainer.Add(trackCurrentStepLowerBound);

      var trackCurrentStep = BuildLabel("", fontSize: 16, marginTop: 0, marginBottom: 0);
      BindLabel(trackCurrentStep, () => $"{(string.IsNullOrWhiteSpace(data.GetCurrentStepSettings().displayName) ? "[no name]" : data.GetCurrentStepSettings().displayName)} ({data.GetCurrentStepIndex() + 1} / {data.steps.Count()})");
      trackCurrentStep.style.flexGrow = 1;
      trackCurrentStep.style.minWidth = 100;
      trackCurrentStep.style.unityTextAlign = TextAnchor.LowerCenter;
      trackCurrentStepBoundsContainer.Add(trackCurrentStep);

      var trackCurrentStepUpperBound = BuildLabel("", marginTop: 0, marginBottom: 0);
      BindLabel(trackCurrentStepUpperBound, () => $"{data.GetCurrentStepUpperBound()}");
      trackCurrentStepUpperBound.style.flexGrow = 1;
      trackCurrentStepUpperBound.style.minWidth = 100;
      trackCurrentStepUpperBound.style.unityTextAlign = TextAnchor.LowerRight;
      trackCurrentStepBoundsContainer.Add(trackCurrentStepUpperBound);

      var trackProgressBar = new ProgressBar()
      {
        lowValue = 0f,
        highValue = 1f,
      };
      BindProgressBar(trackProgressBar,
        () =>
        {
          var currentStep = data.GetCurrentStep();
          if (currentStep == null)
            return 0f;
          return GetProp<float>(currentStep, "ProgressPointsProgress01");
        },
        () => $"Step Progression ({data.track.ProgressPoints - data.GetCurrentStepLowerBound()} / {data.GetCurrentStepUpperBound() - data.GetCurrentStepLowerBound()})"
      );
      progressPointsView.Add(trackProgressBar);

      return progressPointsView;
    }

    private VisualElement BuildTrackNavigationView(TrackDataAccessor data)
    {
      var navigationView = new VisualElement();
      var progressPointsContainer = new VisualElement();
      progressPointsContainer.style.flexDirection = FlexDirection.Row;
      progressPointsContainer.style.marginTop = 5;

      var progressPointsField = new IntegerField("Progress Points");
      progressPointsField.style.flexGrow = 1;
      progressPointsField.style.minWidth = 100;
      progressPointsField.labelElement.style.minWidth = 100;
      progressPointsField.labelElement.style.flexBasis = 100;
      progressPointsField.value = 0;
      progressPointsContainer.Add(progressPointsField);

      var adjustButton = new Button(() => data.track.AdjustProgressPointsBy(progressPointsField.value, force: true));
      adjustButton.text = "Adjust";
      progressPointsContainer.Add(adjustButton);
      navigationView.Add(progressPointsContainer);


      var stepMoveContainer = new VisualElement();
      stepMoveContainer.style.flexDirection = FlexDirection.Row;
      stepMoveContainer.style.marginTop = 5;

      var prevStepButton = new Button(() =>
      {
        var previousStep = GetProp<object>(data.track, "PreviousStep");
        if (previousStep == null)
          return;
        var previousStepLowerBound = GetProp<int>(previousStep, "ProgressPointsLowerBound");
        int deltaToPreviousStepLowerBound =
          previousStepLowerBound == 0 ?
            data.track.ProgressPoints
            : data.track.ProgressPoints - previousStepLowerBound;
        data.track.AdjustProgressPointsBy(-deltaToPreviousStepLowerBound, force: true);
      });
      prevStepButton.text = "Prev Step";
      prevStepButton.style.flexGrow = 1;
      stepMoveContainer.Add(prevStepButton);

      var nextStepButton = new Button(() => data.track.AdjustProgressPointsBy(data.GetCurrentStepUpperBound() - data.track.ProgressPoints, force: true));
      nextStepButton.text = "Next Step";
      nextStepButton.style.flexGrow = 1;
      stepMoveContainer.Add(nextStepButton);

      navigationView.Add(stepMoveContainer);

      return navigationView;
    }


    private VisualElement BuildTriggerEventsView(TrackDataAccessor data)
    {
      var triggerEventsView = new VisualElement();
      triggerEventsView.style.flexDirection = FlexDirection.Row;

      var triggerEventDropdown = new EnumField("Trigger Event", TriggerEventKind.Victory);
      triggerEventDropdown.style.flexGrow = 1;
      triggerEventDropdown.style.minWidth = 100;
      triggerEventDropdown.labelElement.style.minWidth = 100;
      triggerEventDropdown.labelElement.style.flexBasis = 100;
      triggerEventsView.Add(triggerEventDropdown);

      var nextStepButton = new Button(() =>
      {
        var reward = CallMethod<TriggeredBundle>(data.GetCurrentStep(), "ActivateEventTrigger", triggerEventDropdown.value);
        if (reward == null)
          return;
        reward.TryClaim();
      });
      nextStepButton.text = "Trigger";
      nextStepButton.style.flexGrow = 1;
      triggerEventsView.Add(nextStepButton);

      return triggerEventsView;
    }

    private class TrackDataAccessor
    {
      public TrackKind trackKind;
      public IBaseTrack track;
      public InternalBaseTrackSettings settings;
      public IEnumerable<object> steps;
      public Func<object> GetCurrentStep;
      public Func<int> GetCurrentStepIndex;
      public Func<BaseTrackStepSettings> GetCurrentStepSettings;
      public Func<int> GetCurrentStepLowerBound;
      public Func<int> GetCurrentStepUpperBound;

      public TrackDataAccessor(TrackKind trackKind)
      {
        this.trackKind = trackKind;
        track = MetaManager.Instance.GetTrack(trackKind);
        settings = GetProp<InternalBaseTrackSettings>(track, "Settings");
        steps = GetProp<IEnumerable<object>>(track, "Steps");
        GetCurrentStep = () => GetProp<object>(track, "CurrentStep");
        GetCurrentStepIndex = () => GetProp<int>(track, "CurrentStepIndex");
        GetCurrentStepSettings = () => GetProp<BaseTrackStepSettings>(GetCurrentStep(), "Settings");
        GetCurrentStepLowerBound = () => GetProp<int>(GetCurrentStep(), "ProgressPointsLowerBound");
        GetCurrentStepUpperBound = () => GetProp<int>(GetCurrentStep(), "ProgressPointsUpperBound");
      }
    }
  }
}