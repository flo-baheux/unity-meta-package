using System;
using System.Collections.Generic;

namespace MetaPackage
{
  [Serializable]
  public struct TriggeredBundleSettings
  {
    public TriggerEventKind eventCondition;
    // TODO: SerializeReference + GUI improvements
    public List<RewardSettings> rewards;
  }
}