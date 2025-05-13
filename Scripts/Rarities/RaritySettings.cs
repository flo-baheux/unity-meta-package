using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Rarity", fileName = "RaritySettings")]
  public class RaritySettings : ScriptableObject
  {
    public RarityKind rarityKind;
    public string displayName;
    public Color color;
  }
}