using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuMovementScript
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 55), new GUIContent("MovementScript Method"));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 145, 30), new GUIContent("Tick UnityTimer"), !parentBehaviour.Config.movementAudioSync ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.movementAudioSync = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 45, 145, 30), new GUIContent("Tick AudioTimer"), parentBehaviour.Config.movementAudioSync ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.movementAudioSync = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            GUI.Box(new Rect(menuPos.x, menuPos.y + 80, 300, 55), new GUIContent("Song-specific script"));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 100, 145, 30), new GUIContent("Enable"), parentBehaviour.Config.songSpecificScript ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.songSpecificScript = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
                parentBehaviour.AddMovementScript();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 100, 145, 30), new GUIContent("Disable"), !parentBehaviour.Config.songSpecificScript ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.songSpecificScript = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
                if (parentBehaviour.Config.movementScriptPath == string.Empty)
                    parentBehaviour.ClearMovementScript();
                else
                    parentBehaviour.AddMovementScript();
            }

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 140, 80, 30), new GUIContent("<")))
            {
                if (contextMenu.scriptPage > 0) contextMenu.scriptPage--;
            }
            GUI.Box(new Rect(menuPos.x + 80, menuPos.y + 140, 140, 30), new GUIContent($"{contextMenu.scriptPage + 1} / {Math.Ceiling(Decimal.Parse(contextMenu.scriptName.Length.ToString()) / 5)}"));
            if (GUI.Button(new Rect(menuPos.x + 220, menuPos.y + 140, 80, 30), new GUIContent(">")))
            {
                if (contextMenu.scriptPage < Math.Ceiling(Decimal.Parse(contextMenu.scriptName.Length.ToString()) / 5) - 1) contextMenu.scriptPage++;
            }
            for (int i = contextMenu.scriptPage * 5; i < contextMenu.scriptPage * 5 + 5; i++)
            {
                if (i < contextMenu.scriptName.Length)
                {
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + (i - contextMenu.scriptPage * 5) * 30 + 170, 300, 30), new GUIContent(contextMenu.scriptName[i]), CameraUtilities.CurrentMovementScript(parentBehaviour.Config.movementScriptPath) == contextMenu.scriptName[i] ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
                    {
                        parentBehaviour.Config.movementScriptPath = contextMenu.scriptName[i];
                        parentBehaviour.Config.Save();
                        parentBehaviour.AddMovementScript();
                    }
                }
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 330, 200, 40), new GUIContent("Movement Off"), CameraUtilities.CurrentMovementScript(parentBehaviour.Config.movementScriptPath) == string.Empty ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                if (parentBehaviour.Config.movementScriptPath != string.Empty)
                {
                    parentBehaviour.Config.movementScriptPath = String.Empty;
                    parentBehaviour.Config.Save();
                    parentBehaviour.ClearMovementScript();
                }
            }
            /*
            if (GUI.Button(new Rect(menuPos.x , menuPos.y + 390, 300, 30), new GUIContent("Movement Script Record Mode")))
            {
                parentBehaviour.scriptEditMode = true;
                parentBehaviour.mouseMoveCamera = true;
                parentBehaviour.mouseMoveCameraSave = false;
                parentBehaviour.CloseContextMenu();
            }
            */
            //Close
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close MovementScript Menu")))
            {
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;
            }

        }

    }
}
