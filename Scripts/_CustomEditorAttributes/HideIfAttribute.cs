using System;
using UnityEngine;

namespace MetaPackage
{
  // TODO: Mutualize HideIf / ShowIf
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public class HideIfAttribute : PropertyAttribute
  {
    public readonly string conditionField;
    public readonly object compareWith;

    public HideIfAttribute(string conditionField, object compareWith = null)
    {
      this.conditionField = conditionField;
      this.compareWith = compareWith;
    }
  }
}