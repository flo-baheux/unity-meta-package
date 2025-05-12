using System;
using UnityEngine;

namespace MetaPackage
{
  public interface IReferenceable<T> where T : Identifiable
  {
    public T GenerateReference();
    public string ID { get; }
    public string Name { get; }

    public void ResetID();
  }

  public abstract class Referenceable<T> : IReferenceable<T>
    where T : Identifiable
  {
    [SerializeField, HideInInspector] protected string name;
    [SerializeField, ReadOnlyString] protected string _ID;
    public string ID => _ID;
    public string Name => name;

    public abstract T GenerateReference();

    public void ResetID() => _ID = Guid.NewGuid().ToString();
  }

  public abstract class ReferenceableScriptableObject<T> : ScriptableObject, IReferenceable<T>
    where T : Identifiable
  {
    [SerializeField, ReadOnlyString] protected string _ID;
    public string ID => _ID;
    public abstract string Name { get; }

    public abstract T GenerateReference();

    public void ResetID() => _ID = Guid.NewGuid().ToString();
  }
}