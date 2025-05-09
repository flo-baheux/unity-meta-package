using System;
using UnityEngine;

namespace MetaPackage
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public class ShowIfAttribute : PropertyAttribute
  {
    public readonly string conditionField;
    public readonly object compareWith;

    public ShowIfAttribute(string conditionField, object compareWith = null)
    {
      this.conditionField = conditionField;
      this.compareWith = compareWith;
    }
  }
}