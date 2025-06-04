using System;
using UnityEngine;

namespace MetaPackage
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public class ReadOnlyStringAttribute : PropertyAttribute { }
}