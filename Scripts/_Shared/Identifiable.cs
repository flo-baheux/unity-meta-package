using System;
using UnityEngine;

namespace MetaPackage
{
  public class Identifiable : IEquatable<Identifiable>
  {
    [SerializeField] private string _name;
    [SerializeField] private string _ID;

    public string ID => _ID;
    public string Name => _name;

    public Identifiable(string ID, string name)
    {
      _ID = ID;
      _name = name;
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as Identifiable);
    }

    public bool Equals(Identifiable other)
    {
      if (other is null) return false;
      return ID == other.ID;
    }

    public override int GetHashCode()
    {
      return ID != null ? ID.GetHashCode() : 0;
    }

    public static bool operator ==(Identifiable first, Identifiable second)
    {
      if (first is null || second is null) return false;
      return first.Equals(second);
    }

    public static bool operator !=(Identifiable first, Identifiable second)
    {
      return !(first == second);
    }
  }
}