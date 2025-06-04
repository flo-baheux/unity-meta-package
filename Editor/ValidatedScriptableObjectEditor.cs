using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MetaPackage
{
  [CustomEditor(typeof(ValidatedScriptableObject), true)]
  public class ValidatedScriptableObjectEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      var validatedSO = (ValidatedScriptableObject)target;

      foreach (string warningMessage in validatedSO.warnings)
        EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);

      foreach (string errorMessage in validatedSO.errors)
        EditorGUILayout.HelpBox(errorMessage, MessageType.Error);

      DrawDefaultInspector();

      serializedObject.ApplyModifiedProperties();
    }
  }

  [InitializeOnLoad]
  public static class ValidatedScriptableObjectWatcher
  {
    static ValidatedScriptableObjectWatcher() => Selection.selectionChanged += OnSelectionChanged;

    private static void OnSelectionChanged()
    {
      if (Selection.activeObject is ValidatedScriptableObject validatedSO)
        validatedSO.OnValidate();
    }
  }

  [InitializeOnLoad]
  public static class ValidatedScriptablePlayModeValidator
  {
    static ValidatedScriptablePlayModeValidator() => EditorApplication.playModeStateChanged += OnPlayModeChanged;

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.EnteredPlayMode)
        if (!AreAllValidatedSOValid(out List<string> errors))
          errors.ForEach(Debug.LogError);
    }

    private static bool AreAllValidatedSOValid(out List<string> errors)
    {
      errors = new();
      var assets = AssetDatabase.FindAssets("t:ValidatedScriptableObject");
      foreach (var guid in assets)
      {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        var validatedSO = AssetDatabase.LoadAssetAtPath<ValidatedScriptableObject>(path);

        if (validatedSO != null && !validatedSO.IsValid)
          foreach (string SOError in validatedSO.errors)
            errors.Add($"{path}:\n{SOError}");
      }

      return errors.Count == 0;
    }
  }
}

