using System.Collections.Generic;
using UnityEngine;

namespace MetaPackage
{
  public abstract class ValidatedScriptableObject : ScriptableObject
  {
#if UNITY_EDITOR
    public bool IsValid => errors.Count == 0;

    [HideInInspector] public List<string> warnings = new();
    [HideInInspector] public List<string> errors = new();

    public void OnValidate()
    {
      warnings.Clear();
      errors.Clear();
      CustomValidation();
    }

    public abstract void CustomValidation();
#endif
  }
}