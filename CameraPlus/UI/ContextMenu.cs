using System;
using System.Text.RegularExpressions;
using UnityEngine;
using CameraPlus.Camera2Utils;
using CameraPlus.Behaviours;
using CameraPlus.Utilities;

namespace CameraPlus.UI
{
    public class ContextMenu : MonoBehaviour
    {
        internal enum MenuState
        {
            MenuTop,
            DisplayObject,
            Layout,
            Multiplayer,
            Profile,
            MovementScript,
            Camera2Converter,
            ExternalLink
        }
        internal Vector2 menuPos
        {
            get
            {
                return new Vector2(
                   Mathf.Min(mousePosition.x / (Screen.width / 1600f), (Screen.width * ( 0.806249998f / (Screen.width / 1600f)))),
                   Mathf.Min((Screen.height - mousePosition.y) / (Screen.height / 900f), (Screen.height * (0.555555556f / (Screen.height / 900f))))
                    );
            }
        }
        internal Vector2 mousePosition;
        internal bool showMenu;
        internal MenuState MenuMode = MenuState.MenuTop;
        internal CameraPlusBehaviour parentBehaviour;
        internal string[] scriptName;
        internal int scriptPage;
        internal Texture2D texture = null;
        internal Texture2D Cameratexture = null;
        internal GUIStyle CustomEnableStyle = null;
        internal GUIStyle CustomDisableStyle = null;
        internal GUIStyle ProfileStyle = null;

        private MenuDisplayObject menuDisplayObject;
        private MenuLayout menuLayout;
        private MenuMultiplayer menuMultiplayer;

        private string ipNum =@"(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)";

        public void EnableMenu(Vector2 mousePos, CameraPlusBehaviour parentBehaviour)
        {
            this.enabled = true;
            mousePosition = mousePos;
            showMenu = true;
            this.parentBehaviour = parentBehaviour;
            MenuMode = 0;
            scriptName = null;
            scriptPage = 0;

            menuDisplayObject = new MenuDisplayObject();
            menuLayout = new MenuLayout();
            menuMultiplayer = new MenuMultiplayer();

            if (this.parentBehaviour.Config.LockScreen)
                texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Lock.png");
            else
                texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png");
            if (this.parentBehaviour.Config.LockCamera || this.parentBehaviour.Config.LockCameraDrag)
                Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png");
            else
                Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraUnlock.png");
        }
        public void DisableMenu()
        {
            if (!this) return;
            this.enabled = false;
            showMenu = false;
        }
        void OnGUI()
        {

            if (showMenu)
            {
                Vector3 scale;
                float originalWidth = 1600f;
                float originalHeight = 900f;

                scale.x = Screen.width / originalWidth;
                scale.y = Screen.height / originalHeight;
                scale.z = 1;
                Matrix4x4 originalMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);
                //Layer boxes for Opacity
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), $"CameraPlus {parentBehaviour.name}");
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), $"CameraPlus {parentBehaviour.name}");
                GUI.Box(new Rect(menuPos.x - 5, menuPos.y, 310, 470), $"CameraPlus {parentBehaviour.name}");

                CustomEnableStyle = new GUIStyle(GUI.skin.button);
                CustomEnableStyle.normal.background = CustomEnableStyle.active.background;
                CustomEnableStyle.hover.background = CustomEnableStyle.active.background;
                CustomDisableStyle = new GUIStyle(GUI.skin.button);
                ProfileStyle = new GUIStyle(GUI.skin.box);
                ProfileStyle.alignment = UnityEngine.TextAnchor.MiddleLeft;

                if (MenuMode == MenuState.MenuTop)
                {
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 25, 30, 30), texture))
                    {
                        parentBehaviour.Config.LockScreen = !parentBehaviour.Config.LockScreen;
                        parentBehaviour.Config.Save();
                        if (this.parentBehaviour.Config.LockScreen)
                            texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.Lock.png");
                        else
                            texture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.UnLock.png");
                    }
                    GUI.Box(new Rect(menuPos.x + 35, menuPos.y + 25, 115, 30), new GUIContent(parentBehaviour.Config.LockScreen ? "Locked Screen" : "Unlocked Screen"), ProfileStyle);

                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 25, 30, 30), Cameratexture))
                    {
                        if (!parentBehaviour.Config.LockCamera && !parentBehaviour.Config.LockCameraDrag)
                        {
                            parentBehaviour.Config.LockCamera = true;
                            parentBehaviour.Config.LockCameraDrag = false;
                            Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png");
                        }
                        else if (parentBehaviour.Config.LockCamera && !parentBehaviour.Config.LockCameraDrag)
                        {
                            parentBehaviour.Config.LockCamera = false;
                            parentBehaviour.Config.LockCameraDrag = true;
                            Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraLock.png");
                        }
                        else
                        {
                            parentBehaviour.Config.LockCamera = false;
                            parentBehaviour.Config.LockCameraDrag = false;
                            Cameratexture = CustomUtils.LoadTextureFromResources("CameraPlus.Resources.CameraUnlock.png");
                        }
                        parentBehaviour.Config.Save();
                    }
                    GUI.Box(new Rect(menuPos.x + 185, menuPos.y + 25, 115, 30), new GUIContent(parentBehaviour.Config.LockCameraDrag ? "ResetDrag Camera" : (parentBehaviour.Config.LockCamera ? "Locked Camera" : "Unlocked Camera")), ProfileStyle);

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 60, 145, 60), new GUIContent("Add New Camera")))
                    {
                        lock (Plugin.cameraController.Cameras)
                        {
                            string cameraName = CameraUtilities.GetNextCameraName();
                            Logger.log.Notice($"Adding new config with name {cameraName}.cfg");
                            CameraUtilities.AddNewCamera(cameraName);
                            CameraUtilities.ReloadCameras();
                            parentBehaviour.CloseContextMenu();
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 60, 145, 60), new GUIContent("Duplicate\nSelected Camera")))
                    {
                        lock (Plugin.cameraController.Cameras)
                        {
                            string cameraName = CameraUtilities.GetNextCameraName();
                            Logger.log.Notice($"Adding {cameraName}");
                            CameraUtilities.AddNewCamera(cameraName, parentBehaviour.Config);
                            CameraUtilities.ReloadCameras();
                            parentBehaviour.CloseContextMenu();
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 130, 145, 50), new GUIContent("Remove\nSelected Camera")))
                    {
                        lock (Plugin.cameraController.Cameras)
                        {
                            if (CameraUtilities.RemoveCamera(parentBehaviour))
                            {
                                parentBehaviour._isCameraDestroyed = true;
                                parentBehaviour.CreateScreenRenderTexture();
                                parentBehaviour.CloseContextMenu();
                                Logger.log.Notice("Camera removed!");
                            }
                        }
                    }

                    //First Person, Third Person, 360degree
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 190, 300, 55), "Camera Mode");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 210, 95, 30), new GUIContent("First Person"), !parentBehaviour.Config.thirdPerson ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = false;
                        parentBehaviour.Config.use360Camera = false;
                        parentBehaviour.ThirdPerson = parentBehaviour.Config.thirdPerson;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 105, menuPos.y + 210, 95, 30), new GUIContent("Third Person"), (parentBehaviour.Config.thirdPerson && !parentBehaviour.Config.use360Camera) ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = true;
                        parentBehaviour.Config.use360Camera = false;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 205, menuPos.y + 210, 95, 30), new GUIContent("360 degree"), (parentBehaviour.Config.thirdPerson && parentBehaviour.Config.use360Camera) ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.thirdPerson = true;
                        parentBehaviour.Config.use360Camera = true;
                        parentBehaviour.ThirdPersonPos = parentBehaviour.Config.Position;
                        parentBehaviour.ThirdPersonRot = parentBehaviour.Config.Rotation;

                        parentBehaviour.CreateScreenRenderTexture();
                        parentBehaviour.Config.Save();
                    }

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 250, 145, 40), new GUIContent("Display Object")))
                        MenuMode = MenuState.DisplayObject;
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 250, 145, 40), new GUIContent("Layout")))
                        MenuMode = MenuState.Layout;
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 295, 145, 40), new GUIContent("Multiplayer")))
                        MenuMode = MenuState.Multiplayer;
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 340, 145, 40), new GUIContent("Profile Saver")))
                        MenuMode = MenuState.Profile;
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 340, 145, 40), new GUIContent("MovementScript")))
                    {
                        MenuMode = MenuState.MovementScript;
                        scriptName = CameraUtilities.MovementScriptList();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 385, 145, 40), new GUIContent("Camera2 Converter")))
                    {
                        MenuMode = MenuState.Camera2Converter;
                        Camera2ConfigExporter.Init();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 385, 145, 40), new GUIContent("External linkage")))
                        MenuMode = MenuState.ExternalLink;
                    /*
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 345, 300, 30), new GUIContent(parentBehaviour.Config.Orthographics ? "Perspective" : "Orthographics")))
                    {
                        parentBehaviour.Config.Orthographics = !parentBehaviour.Config.Orthographics;
                        parentBehaviour.Config.Save();
                        parentBehaviour.CloseContextMenu();
                        parentBehaviour.CreateScreenRenderTexture();
                    }
                    */
                    /*
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 385, 300, 30), new GUIContent("Spawn 38 Cameras")))
                    {
                        parentBehaviour.StartCoroutine(CameraUtilities.Spawn38Cameras());
                        parentBehaviour.CloseContextMenu();
                    }*/
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Menu")))
                        parentBehaviour.CloseContextMenu();
                }
                else if (MenuMode == MenuState.DisplayObject)
                    menuDisplayObject.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Layout)
                    menuLayout.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Multiplayer)
                    menuMultiplayer.DiplayMenu(parentBehaviour, this, menuPos);
                else if (MenuMode == MenuState.Profile)
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
                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 200, 145, 30), new GUIContent("SceneProfile On"), Plugin.cameraController.rootConfig.ProfileSceneChange ? CustomEnableStyle : CustomDisableStyle))
                    {
                        Plugin.cameraController.rootConfig.ProfileSceneChange = true;
                        Plugin.cameraController.rootConfig.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 200, 145, 30), new GUIContent("SceneProfile Off"), !Plugin.cameraController.rootConfig.ProfileSceneChange ? CustomEnableStyle : CustomDisableStyle))
                    {
                        Plugin.cameraController.rootConfig.ProfileSceneChange = false;
                        Plugin.cameraController.rootConfig.Save();
                    }

                    if (Plugin.cameraController.rootConfig.ProfileSceneChange)
                    {
                        GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 230, 270, 30), "Menu Scene  : " + (Plugin.cameraController.rootConfig.MenuProfile), ProfileStyle);
                        GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 260, 270, 30), "Game Scene  : " + (Plugin.cameraController.rootConfig.GameProfile), ProfileStyle);
                        GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 290, 270, 30), "Game 90/360 : " + (Plugin.cameraController.rootConfig.RotateProfile), ProfileStyle);
                        GUI.Box(new Rect(menuPos.x + 30, menuPos.y + 320, 270, 30), "Multiplayer : " + (Plugin.cameraController.rootConfig.MultiplayerProfile), ProfileStyle);
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
                        MenuMode = 0;
                }
                else if (MenuMode == MenuState.MovementScript)
                {
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 55), new GUIContent("MovementScript Method"));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 45, 145, 30), new GUIContent("Tick UnityTimer"), !parentBehaviour.Config.movementAudioSync ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.movementAudioSync = false;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 45, 145, 30), new GUIContent("Tick AudioTimer"), parentBehaviour.Config.movementAudioSync ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.movementAudioSync = true;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                    }

                    GUI.Box(new Rect(menuPos.x, menuPos.y + 80, 300, 55), new GUIContent("Song-specific script"));
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 100, 145, 30), new GUIContent("Enable"), parentBehaviour.Config.songSpecificScript ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.songSpecificScript = true;
                        parentBehaviour.SetCullingMask();
                        parentBehaviour.Config.Save();
                        parentBehaviour.AddMovementScript();
                    }
                    if (GUI.Button(new Rect(menuPos.x + 150, menuPos.y + 100, 145, 30), new GUIContent("Disable"), !parentBehaviour.Config.songSpecificScript ? CustomEnableStyle : CustomDisableStyle))
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
                        if (scriptPage > 0) scriptPage--;
                    }
                    GUI.Box(new Rect(menuPos.x + 80, menuPos.y + 140, 140, 30), new GUIContent($"{scriptPage + 1} / {Math.Ceiling(Decimal.Parse(scriptName.Length.ToString()) / 5)}"));
                    if (GUI.Button(new Rect(menuPos.x + 220, menuPos.y + 140, 80, 30), new GUIContent(">")))
                    {
                        if (scriptPage < Math.Ceiling(Decimal.Parse(scriptName.Length.ToString()) / 5) - 1) scriptPage++;
                    }
                    for (int i = scriptPage * 5; i < scriptPage * 5 + 5; i++)
                    {
                        if (i < scriptName.Length)
                        {
                            if (GUI.Button(new Rect(menuPos.x, menuPos.y + (i - scriptPage * 5) * 30 + 170, 300, 30), new GUIContent(scriptName[i]), CameraUtilities.CurrentMovementScript(parentBehaviour.Config.movementScriptPath) == scriptName[i] ? CustomEnableStyle : CustomDisableStyle))
                            {
                                parentBehaviour.Config.movementScriptPath = scriptName[i];
                                parentBehaviour.Config.Save();
                                parentBehaviour.AddMovementScript();
                            }
                        }
                    }
                    if (GUI.Button(new Rect(menuPos.x + 50, menuPos.y + 330, 200, 40), new GUIContent("Movement Off"), CameraUtilities.CurrentMovementScript(parentBehaviour.Config.movementScriptPath) == string.Empty ? CustomEnableStyle : CustomDisableStyle))
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
                        MenuMode = 0;
                    }
                }
                else if (MenuMode == MenuState.Camera2Converter)
                {
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 120), "Select scene to import from Camera2");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 50, 140, 25), new GUIContent("<")))
                        Camera2ConfigExporter.TrySceneSetLast(Camera2ConfigExporter.currentlyScenesSelected);
                    if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 50, 140, 25), new GUIContent(">")))
                        Camera2ConfigExporter.SetSceneNext(Camera2ConfigExporter.currentlyScenesSelected);
                    if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 80, 230, 60), new GUIContent("Currently Selected:\n" + Camera2ConfigExporter.currentlyScenesSelected)))
                        Camera2ConfigExporter.SetSceneNext(Camera2ConfigExporter.currentlyScenesSelected);

                    GUI.Box(new Rect(menuPos.x, menuPos.y + 160, 300, 120), "Select profile Export to Scene in Camera2");
                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 185, 140, 25), new GUIContent("<")))
                        CameraUtilities.TrySetLast(CameraUtilities.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x + 155, menuPos.y + 185, 140, 25), new GUIContent(">")))
                        CameraUtilities.SetNext(CameraUtilities.currentlySelected);
                    if (GUI.Button(new Rect(menuPos.x + 30, menuPos.y + 215, 230, 60), new GUIContent("Currently Selected:\n" + CameraUtilities.currentlySelected)))
                        CameraUtilities.SetNext(CameraUtilities.currentlySelected);

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 290, 295, 25), new GUIContent("Export to Selected Scene")))
                        Camera2ConfigExporter.ExportCamera2Scene();

                    if (GUI.Button(new Rect(menuPos.x + 5, menuPos.y + 320, 295, 25), new GUIContent("Import to New Profile")))
                        Camera2ConfigExporter.LoadCamera2Scene();


                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close Camera2 Convert Menu")))
                        MenuMode = 0;
                }
                else if (MenuMode == MenuState.ExternalLink)
                {
                    GUI.Box(new Rect(menuPos.x, menuPos.y + 25, 300, 65), "VMCProtocol");
                    if (parentBehaviour.Config.fitToCanvas)
                    {
                        if (GUI.Button(new Rect(menuPos.x, menuPos.y + 45, 100, 40), new GUIContent("Sender"), parentBehaviour.Config.VMCProtocolMode == "sender" ? CustomEnableStyle : CustomDisableStyle))
                        {
                            parentBehaviour.Config.VMCProtocolMode = "sender";
                            parentBehaviour.Config.Save();
                            parentBehaviour.DestoryVMCProtocolObject();
                            parentBehaviour.InitExternalSender();
                        }

                    }
                    else
                        GUI.Box(new Rect(menuPos.x, menuPos.y + 45, 100, 40), new GUIContent("Require\nFitToCanvas"));
                    if (Plugin.cameraController.existsVMCAvatar)
                    {
                        if (GUI.Button(new Rect(menuPos.x + 100, menuPos.y + 45, 100, 40), new GUIContent("Receiver"), parentBehaviour.Config.VMCProtocolMode == "receiver" ? CustomEnableStyle : CustomDisableStyle))
                        {
                            parentBehaviour.Config.VMCProtocolMode = "receiver";
                            parentBehaviour.Config.Save();
                            parentBehaviour.DestoryVMCProtocolObject();
                            parentBehaviour.InitExternalReceiver();
                        }
                    }
                    else
                        GUI.Box(new Rect(menuPos.x + 100, menuPos.y + 45, 100, 40), new GUIContent("Require\nVMCAvatar Mod"));

                    if (GUI.Button(new Rect(menuPos.x + 200, menuPos.y + 45, 100, 40), new GUIContent("Disable"), parentBehaviour.Config.VMCProtocolMode == "disable" ? CustomEnableStyle : CustomDisableStyle))
                    {
                        parentBehaviour.Config.VMCProtocolMode = "disable";
                        parentBehaviour.Config.Save();
                        parentBehaviour.DestoryVMCProtocolObject();
                    }

                    if (parentBehaviour.Config.VMCProtocolMode == "sender")
                    {
                        GUI.Box(new Rect(menuPos.x, menuPos.y + 90, 150, 45), new GUIContent("Address"));
                        var addr = GUI.TextField(new Rect(menuPos.x, menuPos.y + 110, 150, 25), parentBehaviour.Config.VMCProtocolAddress);
                        if (Regex.IsMatch(addr, ("^" + ipNum + "\\." + ipNum + "\\." + ipNum + "\\." + ipNum + "$")))
                        {
                            parentBehaviour.Config.VMCProtocolAddress = addr;
                            parentBehaviour.Config.Save();
                        }
                        GUI.Box(new Rect(menuPos.x + 150, menuPos.y + 90, 150, 45), new GUIContent("Port"));
                        var port = GUI.TextField(new Rect(menuPos.x + 150, menuPos.y + 110, 150, 25), parentBehaviour.Config.VMCProtocolPort.ToString());
                        if (int.TryParse(port, out int result))
                        {
                            parentBehaviour.Config.VMCProtocolPort = result;
                            parentBehaviour.Config.Save();
                        }
                    }

                    if (GUI.Button(new Rect(menuPos.x, menuPos.y + 430, 300, 30), new GUIContent("Close External linkage Menu")))
                        MenuMode = 0;
                }
                GUI.matrix = originalMatrix;
            }
        }
    }
}
