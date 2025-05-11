using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static MetaPackageDebug.Utils;

namespace MetaPackageDebug
{
  enum DebugTabSections
  {
    Tracks = 0,
    Upgradables = 1,
    Currencies = 2,
    Saves = 3,
  };

  public class DebugWindow : EditorWindow
  {
    [MenuItem("Meta/Debug Window")]
    public static void ShowWindow()
    {
      EditorWindow window = GetWindow<DebugWindow>();
      window.titleContent = new GUIContent("Meta Package Debug Window");

      window.minSize = new Vector2(450, 200);
      window.maxSize = new Vector2(1920, 720);
    }

    private void OnEnable()
    {
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
      EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.EnteredPlayMode)
        CreateGUI();
      else if (state == PlayModeStateChange.EnteredEditMode)
        HandleEditorNotPlaying();
    }

    public void CreateGUI()
    {
      rootVisualElement.Clear();
      if (!EditorApplication.isPlaying)
      {
        HandleEditorNotPlaying();
        return;
      }

      var toolbar = new Toolbar();
      rootVisualElement.Add(toolbar);

      var tracksView = new DebugWindowTracksView().BuildTracksView();
      var upgradablesView = new DebugWindowUpgradablesView().BuildUpgradablesView();
      var currenciesView = new DebugWindowCurrenciesView().BuildCurrenciesView();
      var savesView = new DebugWindowSavesView().BuildSavesView();

      rootVisualElement.Add(tracksView);
      rootVisualElement.Add(upgradablesView);
      rootVisualElement.Add(currenciesView);
      rootVisualElement.Add(savesView);

      var trackToggle = new ToolbarToggle() { text = "Tracks", value = true };
      var upgradablesToggle = new ToolbarToggle() { text = "Upgradables", value = false };
      var currenciesToggle = new ToolbarToggle() { text = "Currencies", value = false };
      var savesToggle = new ToolbarToggle() { text = "Saves", value = false };

      var viewByToggle = new Dictionary<ToolbarToggle, VisualElement>()
      {
        { trackToggle, tracksView },
        { upgradablesToggle, upgradablesView },
        { currenciesToggle, currenciesView },
        { savesToggle, savesView },
      };

      List<ToolbarToggle> toggles = viewByToggle.Keys.ToList();
      foreach (var toggle in toggles)
      {
        toolbar.Add(toggle);
        SetVisible(viewByToggle[toggle], false);
        toggle.RegisterValueChangedCallback(e =>
        {
          // Cannot manually untoggle current section
          if (e.newValue == false && viewByToggle[toggle].style.display != DisplayStyle.None)
          {
            toggle.SetValueWithoutNotify(true);
            return;
          }

          // hide and untoggle everything else
          foreach (var t in toggles)
            if (t != toggle)
            {
              t.SetValueWithoutNotify(false);
              SetVisible(viewByToggle[t], false);
            }

          SetVisible(viewByToggle[toggle], true);
        });
      }

      trackToggle.value = true;
      SetVisible(tracksView, true);
    }

    private void HandleEditorNotPlaying()
    {
      rootVisualElement.Clear();

      var label = BuildLabel("Enter play mode to use the debug window.", 16, marginTop: 20, marginBottom: 20);
      label.style.unityFontStyleAndWeight = FontStyle.Bold;
      label.style.unityTextAlign = TextAnchor.MiddleCenter;

      rootVisualElement.Add(label);

      rootVisualElement.Add(DebugWindowSavesView.BuildDeleteSaveButton());
      rootVisualElement.Add(DebugWindowSavesView.BuildOpenSaveFolderButton());
    }
  }
}