using UnityEngine;
using CameraPlus.Behaviours;

namespace CameraPlus.UI
{
    internal class MenuDisplayObject
    {
        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour,ContextMenu contextMenu, Vector2 menuPos)
        {
            //Display Preview Quad
            GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 55), "Display ThirdPersonCamera PrevewQuad");

            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 145, 30), new GUIContent("Hide CameraQuad"), !parentBehaviour.Config.showThirdPersonCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.showThirdPersonCamera = false;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }

            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 45, 145, 30), new GUIContent("Display CameraQuad"), parentBehaviour.Config.showThirdPersonCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.showThirdPersonCamera = true;
                parentBehaviour.Config.Save();
                parentBehaviour.CreateScreenRenderTexture();
            }

            //FirstPerson Camera Upright
            GUI.Box(new Rect(menuPos.x, menuPos.y + 80, 300, 55), "Force FirstPerson Camera Upright");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 100, 145, 30), new GUIContent("Disable Upright"), !parentBehaviour.Config.forceFirstPersonUpRight ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.forceFirstPersonUpRight = false;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 100, 145, 30), new GUIContent("Enable Upright"), parentBehaviour.Config.forceFirstPersonUpRight ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.forceFirstPersonUpRight = true;
                parentBehaviour.Config.Save();
            }

            //TransportWall
            GUI.Box(new Rect(menuPos.x, menuPos.y + 135, 300, 55), "Transport Wall");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 155, 145, 30), new GUIContent("Disable Transportwall"), !parentBehaviour.Config.transparentWalls ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.transparentWalls = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 155, 145, 30), new GUIContent("Enable Transportwall"), parentBehaviour.Config.transparentWalls ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.transparentWalls = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            //Debri
            GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 300, 55), "Display Debri");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 95, 30), new GUIContent("Link InGame"), parentBehaviour.Config.debri == "link" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.debri = "link";
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 210, 95, 30), new GUIContent("Forced Display"), parentBehaviour.Config.debri == "show" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.debri = "show";
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 210, 95, 30), new GUIContent("Forced Hide"), parentBehaviour.Config.debri == "hide" ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.debri = "hide";
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            //Display UI
            GUI.Box(new Rect(menuPos.x, menuPos.y + 245, 300, 55), "Display Official UI");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 265, 145, 30), new GUIContent("Show UI"), !parentBehaviour.Config.HideUI ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.HideUI = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 265, 145, 30), new GUIContent("Hide UI"), parentBehaviour.Config.HideUI ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.HideUI = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            //Display Custom and VMC Avatar
            GUI.Box(new Rect(menuPos.x, menuPos.y + 300, 300, 55), "Display CustomAvatar/VMCAvatar");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 320, 145, 30), new GUIContent("Show Avatar"), parentBehaviour.Config.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.avatar = true;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 320, 145, 30), new GUIContent("Hide Avatar"), !parentBehaviour.Config.avatar ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.avatar = false;
                parentBehaviour.SetCullingMask();
                parentBehaviour.Config.Save();
            }

            //Camera Tracking to NoodleExtensions AssignPlayerToTrack
            GUI.Box(new Rect(menuPos.x, menuPos.y + 355, 300, 55), "Tracking NoodleExtension PlayerToTrack");
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 375, 145, 30), new GUIContent("Tracking On"), parentBehaviour.Config.NoodleTrack ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.NoodleTrack = true;
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 375, 145, 30), new GUIContent("Tracking Off"), !parentBehaviour.Config.NoodleTrack ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.NoodleTrack = false;
                parentBehaviour.Config.Save();
            }

            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close CameraMode Menu")))
            {
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;
            }
        }
    }
}
