using UnityEngine;

namespace MetaPackage
{
  [CreateAssetMenu(menuName = "MetaPackage/Rarity", fileName = "RaritySettings")]
  public class RaritySettings: ScriptableObject
  {
    public RarityKind kind;
    public string displayName;
    public Color color;
  }
}