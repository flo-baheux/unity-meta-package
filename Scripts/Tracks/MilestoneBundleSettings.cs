using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  [Serializable]
  public class MilestoneBundleSettings
  {
    [HideInInspector, SerializeField] private string name;

    [ReadOnlyString, SerializeField] private string _ID;
    [HideInInspector] public string ID => _ID;

    public int pointsRequired;
    public List<RewardSettings> rewards;

    public void Init()
    {
      _ID = Guid.NewGuid().ToString();
      pointsRequired = 0;
      rewards = new();
      Refresh();
    }

    public void Refresh()
    {
      name = $"{pointsRequired}";
    }
  }
}