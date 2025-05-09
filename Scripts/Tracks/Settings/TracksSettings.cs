using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/TracksSettings", fileName = "TracksSettings")]
  public class TracksSettings : ScriptableObject
  {
    public List<InternalBaseTrackSettings> trackSettingsList;
  }
}