using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  // [CreateAssetMenu(menuName = "MetaPackage/Upgradables/UpgradableCategoriesSettings", fileName = "UpgradableCategoriesSettings")]
  public class UpgradableCategoriesSettings : ScriptableObject
  {
    public List<BaseUpgradableCategorySettings> upgradableCategorySettingsList;
  }
}