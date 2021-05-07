using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuProfile
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 140, 30), new GUIContent("<")))
                CameraUtilities.TrySetLast(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 25, 140, 30), new GUIContent(">")))
                CameraUtilities.SetNext(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 60, 230, 60), new GUIContent("Currently Selected:\n" + CameraUtilities.currentlySelected)))
                CameraUtilities.SetNext(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 125, 140, 30), new GUIContent("Save")))
                CameraUtilities.SaveCurrent();
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 125, 140, 30), new GUIContent("Delete")))
            {
                if (!Plugin.cameraController.rootConfig.ProfileLoadCopyMethod)
                    CameraUtilities.ProfileChange(null);
                CameraUtilities.DeleteProfile(CameraUtilities.currentlySelected);
                CameraUtilities.TrySetLast();
            }
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 165, 300, 30), new GUIContent("Load Selected")))
                CameraUtilities.ProfileChange(CameraUtilities.currentlySelected);
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 200, 145, 30), new GUIContent("SceneProfile On"), Plugin.cameraController.rootConfig.ProfileSceneChange ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                Plugin.cameraController.rootConfig.ProfileSceneChange = true;
                Plugin.cameraController.rootConfig.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 200, 145, 30), new GUIContent("SceneProfile Off"), !Plugin.cameraController.rootConfig.ProfileSceneChange ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                Plugin.cameraController.rootConfig.ProfileSceneChange = false;
                Plugin.cameraController.rootConfig.Save();
            }

            if (Plugin.cameraController.rootConfig.ProfileSceneChange)
            {
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 230, 270, 30), "Menu Scene  : " + (Plugin.cameraController.rootConfig.MenuProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 260, 270, 30), "Game Scene  : " + (Plugin.cameraController.rootConfig.GameProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 290, 270, 30), "Game 90/360 : " + (Plugin.cameraController.rootConfig.RotateProfile), contextMenu.ProfileStyle);
                GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 320, 270, 30), "Multiplayer : " + (Plugin.cameraController.rootConfig.MultiplayerProfile), contextMenu.ProfileStyle);
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 230, 30, 30), "X"))
                {
                    if (Plugin.cameraController.rootConfig.MenuProfile != string.Empty)
                        Plugin.cameraController.rootConfig.MenuProfile = string.Empty;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 260, 30, 30), "X"))
                {
                    if (Plugin.cameraController.rootConfig.GameProfile != string.Empty)
                        Plugin.cameraController.rootConfig.GameProfile = string.Empty;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 290, 30, 30), "X"))
                {
                    if (Plugin.cameraController.rootConfig.RotateProfile != string.Empty)
                        Plugin.cameraController.rootConfig.RotateProfile = string.Empty;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 320, 30, 30), "X"))
                {
                    if (Plugin.cameraController.rootConfig.MultiplayerProfile != string.Empty)
                        Plugin.cameraController.rootConfig.MultiplayerProfile = string.Empty;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 350, 145, 25), new GUIContent("Menu Selected")))
                {
                    Plugin.cameraController.rootConfig.MenuProfile = CameraUtilities.currentlySelected;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 375, 145, 25), new GUIContent("Game Selected")))
                {
                    Plugin.cameraController.rootConfig.GameProfile = CameraUtilities.currentlySelected;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 375, 145, 25), new GUIContent("90/360 Selected")))
                {
                    Plugin.cameraController.rootConfig.RotateProfile = CameraUtilities.currentlySelected;
                    Plugin.cameraController.rootConfig.Save();
                }
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 400, 145, 25), new GUIContent("Multiplay Selected")))
                {
                    Plugin.cameraController.rootConfig.MultiplayerProfile = CameraUtilities.currentlySelected;
                    Plugin.cameraController.rootConfig.Save();
                }
            }
            /*
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 390, 290, 30), new GUIContent(Plugin.Instance._rootConfig.ProfileLoadCopyMethod ? "To Folder Reference Method" : "To File Copy Method")))
            {
                Plugin.Instance._rootConfig.ProfileLoadCopyMethod = !Plugin.Instance._rootConfig.ProfileLoadCopyMethod;
                Plugin.Instance._rootConfig.Save();
                Plugin.Instance._profileChanger.ProfileChange(null);
            }
            */
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Profile Menu")))
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;

        }

    }
}
