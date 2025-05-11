using System;
using MetaPackage;
using UnityEngine;
using UnityEngine.UIElements;
using static MetaPackageDebug.Utils;

namespace MetaPackageDebug
{
  public class DebugWindowSavesView
  {
    public VisualElement BuildSavesView()
    {
      try
      {
        VisualElement savesView = new Box();

        savesView.Add(BuildLiveResetSaveButton());
        savesView.Add(BuildOpenSaveFolderButton());

        return savesView;
      }
      catch (Exception e)
      {
        Debug.LogException(e);
        return BuildErrorLabel("Failed to build saves view");
      }
    }

    private static Button BuildLiveResetSaveButton()
    {
      var button = new Button(() => MetaManager.Instance.LiveResetSave());
      button.text = "Reset meta save";
      button.style.height = 30;
      button.style.fontSize = 14;
      return button;
    }

    public static Button BuildOpenSaveFolderButton() {
      var button = new Button(() => MetaManager.OpenSaveFolder());
      button.text = "Open save folder";
      button.style.height = 30;
      button.style.fontSize = 14;
      return button;
    }

    public static Button BuildDeleteSaveButton()
    {
      var button = new Button(() => MetaManager.DeleteSave());
      button.text = "Delete meta save";
      button.style.height = 30;
      button.style.fontSize = 14;
      return button;
    }
  }
}