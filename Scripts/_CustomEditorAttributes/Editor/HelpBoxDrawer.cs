// Credits: https://discussions.unity.com/t/helpattribute-allows-you-to-use-helpbox-in-the-unity-inspector-window/659414/6

using UnityEngine;
using UnityEditor;

namespace MetaPackage
{

  [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
  public class HelpBoxDrawer : DecoratorDrawer
  {
    GUIStyle helpBoxStyle;

    public override float GetHeight()
    {
      var helpBoxAttribute = attribute as HelpBoxAttribute;
      if (helpBoxAttribute == null || helpBoxStyle == null) return base.GetHeight();
      return Mathf.Max(
        helpBoxAttribute.messageType == HelpBoxMessageType.None ? 20f : 40f,
        helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 4
      );
    }

    public override void OnGUI(Rect position)
    {
      var helpBoxAttribute = attribute as HelpBoxAttribute;
      if (helpBoxAttribute == null) return;
      EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
      helpBoxStyle = (GUI.skin != null) ? new(GUI.skin.GetStyle("helpbox")) : null;
    }

    private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
    {
      switch (helpBoxMessageType)
      {
        default:
        case HelpBoxMessageType.None: return MessageType.None;
        case HelpBoxMessageType.Info: return MessageType.Info;
        case HelpBoxMessageType.Warning: return MessageType.Warning;
        case HelpBoxMessageType.Error: return MessageType.Error;
      }
    }
  }
}
