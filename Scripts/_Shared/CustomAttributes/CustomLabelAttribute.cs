using System;
using UnityEngine;

namespace MetaPackage
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public class CustomLabelAttribute : PropertyAttribute
  {
    public readonly string customLabel;

    public CustomLabelAttribute(string customLabel)
    {
      this.customLabel = customLabel;
    }
  }
}