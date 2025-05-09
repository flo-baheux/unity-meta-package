using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace MetaPackageDebug
{
  public static class Utils
  {

    public static void BindLabel(Label label, Func<string> getValue, float refreshRate = 0.5f)
      => label.schedule.Execute(() => label.text = getValue()).Every((long)(refreshRate * 1000));

    public static void BindActionToElement(VisualElement element, Action action, float refreshRate = 0.5f)
      => element.schedule.Execute(action).Every((long)(refreshRate * 1000));

    public static void BindProgressBar(ProgressBar bar, Func<float> getValue, Func<string> getTitle, float refreshRate = 0.5f)
    {
      _ = bar.schedule.Execute(() =>
      {
        bar.value = getValue();
        bar.title = getTitle();
      }).Every((long)(refreshRate * 1000));
    }

    public static void SetVisible(VisualElement e, bool isVisible)
      => e.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;

    public static T GetProp<T>(object instance, string propName)
      => (T)instance.GetType().GetProperty(propName).GetValue(instance);

    public static void SetPrivateProperty<T>(object instance, string fieldName, T newValue)
      => instance.GetType().BaseType.GetProperty(fieldName).GetSetMethod(true).Invoke(instance, new object[] { newValue });

    public static T GetField<T>(object instance, string fieldName)
      => (T)instance.GetType().GetField(fieldName).GetValue(instance);

    public static void SetPrivateField<T>(object instance, string fieldName, T newValue)
      => instance.GetType().BaseType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, newValue);

    public static T CallMethod<T>(object instance, string methodName, params object[] args)
      => (T)instance.GetType().GetMethod(methodName).Invoke(instance, args);

    public static VisualElement BuildSeparator(int height = 2, Color? color = null)
    {
      VisualElement separator = new();
      separator.style.height = height;
      separator.style.backgroundColor = color ?? new Color(0.5f, 0.5f, 0.5f);
      separator.style.marginTop = 20;
      separator.style.marginBottom = 20;
      separator.style.marginLeft = 20;
      separator.style.marginRight = 20;
      return separator;
    }

    public static Label BuildLabel(string text, int fontSize = 12, int marginTop = 5, int marginBottom = 5)
    {
      var label = new Label(text);
      label.style.fontSize = fontSize;
      label.style.marginTop = marginTop;
      label.style.marginBottom = marginBottom;
      return label;
    }

    public static string SanitizeForDropdown(string choice) => choice.Replace("/", " \u2215 ");

    public static VisualElement BuildErrorLabel(string error)
    {
      var label = BuildLabel($"{error}\nSee console errors.", 16, marginTop: 10, marginBottom: 10);
      label.style.backgroundColor = Color.red;
      label.style.color = Color.white;
      label.style.unityTextAlign = TextAnchor.MiddleCenter;
      return label;
    }
  }
}