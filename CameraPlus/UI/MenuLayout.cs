using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    internal class MenuLayout
    {
        internal float amountMove = 0.1f;
        internal float amountRot = 0.1f;

        internal void DiplayMenu(CameraPlusBehaviour parentBehaviour, ContextMenu contextMenu, Vector2 menuPos)
        {
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 25, 300, 30), new GUIContent("Reset Camera Position and Rotation")))
            {
                parentBehaviour.Config.Position = parentBehaviour.Config.DefaultPosition;
                parentBehaviour.Config.Rotation = parentBehaviour.Config.DefaultRotation;
                parentBehaviour.Config.FirstPersonPositionOffset = parentBehaviour.Config.DefaultFirstPersonPositionOffset;
                parentBehaviour.Config.FirstPersonRotationOffset = parentBehaviour.Config.DefaultFirstPersonRotationOffset;
                parentBehaviour.ThirdPersonPos = parentBehaviour.Config.DefaultPosition;
                parentBehaviour.ThirdPersonRot = parentBehaviour.Config.DefaultRotation;
                parentBehaviour.Config.Save();
                parentBehaviour.CloseContextMenu();
            }

            //Layer
            GUI.Box(new Rect(menuPos.x, menuPos.y + 55, 90, 50), "Layer: " + parentBehaviour.Config.layer);
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 75, 40, 30), new GUIContent("-")))
            {
                parentBehaviour.Config.layer--;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 45, menuPos.y + 75, 40, 30), new GUIContent("+")))
            {
                parentBehaviour.Config.layer++;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //FOV
            GUI.Box(new Rect(menuPos.x + 90, menuPos.y + 55, 90, 50), "FOV: " + parentBehaviour.Config.fov);
            if (GUI.Button(new Rect(menuPos.x + 95, menuPos.y + 75, 40, 30), new GUIContent("-")))
            {
                parentBehaviour.Config.fov--;
                parentBehaviour.SetFOV();
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 135, menuPos.y + 75, 40, 30), new GUIContent("+")))
            {
                parentBehaviour.Config.fov++;
                parentBehaviour.SetFOV();
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Fit Canvas
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 55, 120, 50), "Fit to Canvas");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 75, 55, 30), new GUIContent("Enable"), parentBehaviour.Config.fitToCanvas ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.fitToCanvas = true;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 75, 55, 30), new GUIContent("Disable"), !parentBehaviour.Config.fitToCanvas ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.fitToCanvas = false;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }

            //Render Scale
            GUI.Box(new Rect(menuPos.x, menuPos.y + 105, 120, 50), "Render Scale: " + parentBehaviour.Config.renderScale.ToString("F1"));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 125, 55, 30), new GUIContent("-")))
            {
                parentBehaviour.Config.renderScale -= 0.1f;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 125, 55, 30), new GUIContent("+")))
            {
                parentBehaviour.Config.renderScale += 0.1f;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Mouse Drag
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 105, 120, 50), "Mouse Drag");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 125, 55, 30), new GUIContent("Enable"), parentBehaviour.mouseMoveCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.mouseMoveCamera = true;
                parentBehaviour.mouseMoveCameraSave = true;
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 125, 55, 30), new GUIContent("Disable"), !parentBehaviour.mouseMoveCamera ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.mouseMoveCamera = false;
                parentBehaviour.mouseMoveCameraSave = false;
            }
            //Turn to Head
            GUI.Box(new Rect(menuPos.x + 180, menuPos.y + 155, 120, 50), "Turn to Head");
            if (GUI.Button(new Rect(menuPos.x + 185, menuPos.y + 175, 55, 30), new GUIContent("Enable"), parentBehaviour.Config.turnToHead ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.turnToHead = true;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 240, menuPos.y + 175, 55, 30), new GUIContent("Disable"), !parentBehaviour.Config.turnToHead ? contextMenu.CustomEnableStyle : contextMenu.CustomDisableStyle))
            {
                parentBehaviour.Config.turnToHead = false;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Amount of Movemnet
            GUI.Box(new Rect(menuPos.x, menuPos.y + 155, 175, 50), "Amount movement : " + amountMove.ToString("F2"));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 175, 55, 30), new GUIContent("0.01")))
            {
                amountMove = 0.01f;
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 175, 55, 30), new GUIContent("0.10")))
            {
                amountMove = 0.1f;
                parentBehaviour.CreateScreenRenderTexture();
            }
            if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 175, 55, 30), new GUIContent("1.00")))
            {
                amountMove = 1.0f;
                parentBehaviour.CreateScreenRenderTexture();
            }
            //X Position
            GUI.Box(new Rect(menuPos.x, menuPos.y + 205, 100, 50), "X Pos :" +
                (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posx.ToString("F2") : parentBehaviour.Config.firstPersonPosOffsetX.ToString("F2")));
            if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 225, 45, 30), new GUIContent("-")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posx -= amountMove;
                else
                    parentBehaviour.Config.firstPersonPosOffsetX -= amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 225, 45, 30), new GUIContent("+")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posx += amountMove;
                else
                    parentBehaviour.Config.firstPersonPosOffsetX += amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Y Position
            GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 205, 100, 50), "Y Pos :" +
                (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posy.ToString("F2") : parentBehaviour.Config.firstPersonPosOffsetY.ToString("F2")));
            if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 225, 45, 30), new GUIContent("-")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posy -= amountMove;
                else
                    parentBehaviour.Config.firstPersonPosOffsetY -= amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 225, 45, 30), new GUIContent("+")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posy += amountMove;
                else
                    parentBehaviour.Config.firstPersonPosOffsetY += amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            //Z Position
            GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 205, 100, 50), "Z Pos :" +
                (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.posz.ToString("F2") : parentBehaviour.Config.firstPersonPosOffsetZ.ToString("F2")));
            if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 225, 45, 30), new GUIContent("-")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posz -= amountMove;
                else
                    parentBehaviour.Config.firstPersonPosOffsetZ -= amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 225, 45, 30), new GUIContent("+")))
            {
                if (parentBehaviour.Config.thirdPerson)
                    parentBehaviour.Config.posz += amountMove;
                else
                    parentBehaviour.Config.firstPersonPosOffsetZ += amountMove;
                parentBehaviour.CreateScreenRenderTexture();
                parentBehaviour.Config.Save();
            }
            if (!parentBehaviour.Config.turnToHead)
            {
                //Amount of Rotation
                GUI.Box(new Rect(menuPos.x, menuPos.y + 255, 290, 50), "Amount rotation : " + amountRot.ToString("F2"));
                if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 275, 50, 30), new GUIContent("0.01")))
                {
                    amountRot = 0.01f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 60, menuPos.y + 275, 50, 30), new GUIContent("0.10")))
                {
                    amountRot = 0.1f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 115, menuPos.y + 275, 50, 30), new GUIContent("1.00")))
                {
                    amountRot = 1.0f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 170, menuPos.y + 275, 50, 30), new GUIContent("10")))
                {
                    amountRot = 10.0f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 275, 50, 30), new GUIContent("45")))
                {
                    amountRot = 45.0f;
                    parentBehaviour.CreateScreenRenderTexture();
                }
                //X Rotation
                GUI.Box(new Rect(menuPos.x, menuPos.y + 305, 100, 50), "X Rot :" +
                    (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.angx.ToString("F2") : parentBehaviour.Config.firstPersonRotOffsetX.ToString("F2")));
                if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 325, 45, 30), new GUIContent("-")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.angx -= amountRot;
                    else
                        parentBehaviour.Config.firstPersonRotOffsetX -= amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 325, 45, 30), new GUIContent("+")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.angx += amountRot;
                    else
                        parentBehaviour.Config.firstPersonRotOffsetX += amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                //Y Rotation
                GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 305, 100, 50), "Y Rot :" +
                    (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.angy.ToString("F2") : parentBehaviour.Config.firstPersonRotOffsetY.ToString("F2")));
                if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 325, 45, 30), new GUIContent("-")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.angy -= amountRot;
                    else
                        parentBehaviour.Config.firstPersonRotOffsetY -= amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 325, 45, 30), new GUIContent("+")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.angy += amountRot;
                    else
                        parentBehaviour.Config.firstPersonRotOffsetY += amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                //Z Rotation
                GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 305, 100, 50), "Z Rot :" +
                    (parentBehaviour.Config.thirdPerson ? parentBehaviour.Config.angz.ToString("F2") : parentBehaviour.Config.firstPersonRotOffsetZ.ToString("F2")));
                if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 325, 45, 30), new GUIContent("-")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.angz -= amountRot;
                    else
                        parentBehaviour.Config.firstPersonRotOffsetZ -= amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 325, 45, 30), new GUIContent("+")))
                {
                    if (parentBehaviour.Config.thirdPerson)
                        parentBehaviour.Config.angz += amountRot;
                    else
                        parentBehaviour.Config.firstPersonRotOffsetZ += amountRot;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
            }
            else
            {
                //Turn to Head Offset
                GUI.Box(new Rect(menuPos.x, menuPos.y + 255, 300, 70), "Turn to Head Offset");
                //X Position
                GUI.Box(new Rect(menuPos.x, menuPos.y + 275, 100, 50), $"X Pos:{parentBehaviour.Config.turnToHeadOffsetX.ToString("F2")}");
                if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 295, 45, 30), new GUIContent("-")))
                {
                    parentBehaviour.Config.turnToHeadOffsetX -= amountMove;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 295, 45, 30), new GUIContent("+")))
                {
                    parentBehaviour.Config.turnToHeadOffsetX += amountMove;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                //Y Position
                GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 275, 100, 50), $"Y Pos :{parentBehaviour.Config.turnToHeadOffsetY.ToString("F2")}");
                if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 295, 45, 30), new GUIContent("-")))
                {
                    parentBehaviour.Config.turnToHeadOffsetY -= amountMove;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 295, 45, 30), new GUIContent("+")))
                {
                    parentBehaviour.Config.turnToHeadOffsetY += amountMove;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                //Z Position
                GUI.Box(new Rect(menuPos.x + 200, menuPos.y + 275, 100, 50), $"Z Pos :{parentBehaviour.Config.turnToHeadOffsetZ.ToString("F2")}");
                if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 295, 45, 30), new GUIContent("-")))
                {
                    parentBehaviour.Config.turnToHeadOffsetZ -= amountMove;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 250, menuPos.y + 295, 45, 30), new GUIContent("+")))
                {
                    parentBehaviour.Config.turnToHeadOffsetZ += amountMove;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
            }
            if (!parentBehaviour.Config.fitToCanvas)
            {
                if (GUI.Button(new Rect(menuPos.x, menuPos.y + 360, 75, 65), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenLeftDock.png")))
                {
                    parentBehaviour.Config.screenPosX = 0;
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 360, 75, 30), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenTopLeftDock.png")))
                {
                    parentBehaviour.Config.screenPosX = 0;
                    parentBehaviour.Config.screenPosY = Screen.height - (Screen.height / 2);
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 75, menuPos.y + 395, 75, 30), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenBottomLeftDock.png")))
                {
                    parentBehaviour.Config.screenPosX = 0;
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 360, 75, 30), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenTopRightDock.png")))
                {
                    parentBehaviour.Config.screenPosX = Screen.width - (Screen.width / 3);
                    parentBehaviour.Config.screenPosY = Screen.height - (Screen.height / 2);
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 395, 75, 30), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenBottomRightDock.png")))
                {
                    parentBehaviour.Config.screenPosX = Screen.width - (Screen.width / 3);
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height / 2;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
                if (GUI.Button(new Rect(menuPos.x + 225, menuPos.y + 360, 75, 65), CustomUtils.LoadTextureFromResources("CameraPlus.Resources.ScreenRightDock.png")))
                {
                    parentBehaviour.Config.screenPosX = Screen.width - (Screen.width / 3);
                    parentBehaviour.Config.screenPosY = 0;
                    parentBehaviour.Config.screenWidth = Screen.width / 3;
                    parentBehaviour.Config.screenHeight = Screen.height;
                    parentBehaviour.CreateScreenRenderTexture();
                    parentBehaviour.Config.Save();
                }
            }
            //Close
            if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Layout Menu")))
            {
                contextMenu.MenuMode = ContextMenu.MenuState.MenuTop;
            }

        }
    }
}
