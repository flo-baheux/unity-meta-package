using System.Collections.Generic;
using System.Linq;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/TrackDatabase", fileName = "TrackDatabase")]
  public class TrackDatabase : ValidatedScriptableObject
  {
    public List<InternalBaseTrackSettings> trackSettingsList;

#if UNITY_EDITOR
    public override void CustomValidation()
    {
      if (HasDuplicateTrackKind(out string duplicateErrorMessage))
        errors.Add(duplicateErrorMessage);
    }

    private bool HasDuplicateTrackKind(out string duplicateErrorMessage)
    {
      bool duplicateFound = false;
      duplicateErrorMessage = "Cannot contain twice the same track.";
      Dictionary<TrackKind, List<InternalBaseTrackSettings>> trackSettingsByTrack = new();
      foreach (var trackSettings in trackSettingsList)
      {
        var trackKind = trackSettings.TrackKind;
        if (!trackSettingsByTrack.ContainsKey(trackKind))
          trackSettingsByTrack[trackKind] = new();
        else
          duplicateFound = true;

        trackSettingsByTrack[trackKind].Add(trackSettings);
      }

      if (duplicateFound)
        foreach ((var trackKind, var trackSettingsList) in trackSettingsByTrack)
          if (trackSettingsList.Count > 1)
            duplicateErrorMessage += $"\n{trackKind} - in {string.Join(',', trackSettingsList.Select(x => x.name))}";

      return duplicateFound;
    }
#endif
  }
}