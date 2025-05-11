using System;
using System.Collections.Generic;

namespace MetaPackage
{
  [Serializable]
  public class UpgradablesSaveData
  {
    public List<UpgradableSaveData> upgradablesSaveData;
  }

  [Serializable]
  public class UpgradableSaveData
  {
    public int upgradableKindAsInt;
    public int entityKindAsInt;
    public int level;
    public int experience;
    public bool isUnlocked;
  }
}