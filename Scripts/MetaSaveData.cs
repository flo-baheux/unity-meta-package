using System;

namespace MetaPackage
{
  [Serializable]
  public class MetaSaveData
  {
    public TracksSaveData tracksSaveData;
    public UpgradablesSaveData upgradablesSaveData;
    public CurrenciesSaveData currenciesSaveData;
  }
}