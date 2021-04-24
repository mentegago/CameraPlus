using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using IPA.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;
using CameraPlus.Configuration;
using CameraPlus.Behaviours;
using CameraPlus.HarmonyPatches;

namespace CameraPlus.Utilities
{
    public class CameraUtilities
    {
        #region ** DefineStatic **
        internal static string profilePath = Path.Combine(UnityGame.UserDataPath, $".{Plugin.Name.ToLower()}");
        internal static string cfgPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name);
        internal static string scriptPath = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts");
        internal static string currentlySelected = "None";
        public static List<int> vmcPortList = null;

        internal static float[] mouseMoveSpeed = { -0.01f, -0.01f };//x, y
        internal static float mouseScrollSpeed = 0.5f;
        internal static float[] mouseRotateSpeed = { -0.05f, 0.05f, 1f };//x, y, z
        internal static bool movementScriptEditMode = false;

        internal static Texture2D seekBarBackground = null;
        internal static Texture2D seekBar = null;
        #endregion

        private static bool CameraExists(string cameraName)
        {
            return Plugin.cameraController.Cameras.Keys.Where(c => c == $"{cameraName}.cfg").Count() > 0;
        }

        internal static void AddNewCamera(string cameraName, Config CopyConfig = null, bool meme = false)
        {
            string path = Path.Combine(UnityGame.UserDataPath, Plugin.Name, $"{cameraName}.cfg");
            if (!Plugin.cameraController.rootConfig.ProfileLoadCopyMethod && Plugin.cameraController.currentProfile != null)
                path = Path.Combine(UnityGame.UserDataPath, "." + Plugin.Name.ToLower(), "Profiles", Plugin.cameraController.currentProfile, $"{cameraName}.cfg");

            if (!File.Exists(path))
            {
                // Try to copy their old config file into the new camera location
                if (cameraName == Plugin.MainCamera)
                {
                    string oldPath = Path.Combine(Environment.CurrentDirectory, $"{Plugin.MainCamera}.cfg");
                    if (File.Exists(oldPath))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                            Directory.CreateDirectory(Path.GetDirectoryName(path));

                        File.Move(oldPath, path);
                        Logger.log.Notice($"Copied old {Plugin.MainCamera}.cfg into new {Plugin.Name} folder in UserData");
                    }
                }

                Config config = null;
                if (CopyConfig != null)
                    File.Copy(CopyConfig.FilePath, path, true);

                config = new Config(path);
                foreach (CameraPlusInstance c in Plugin.cameraController.Cameras.Values.OrderBy(i => i.Config.layer))
                {
                    if (c.Config.layer > config.layer)
                        config.layer += (c.Config.layer - config.layer);
                    else if (c.Config.layer == config.layer)
                        config.layer++;
                }

                if (cameraName == Plugin.MainCamera)
                    config.fitToCanvas = true;

                if (meme)
                {
                    config.screenWidth = (int)Random.Range(200, Screen.width / 1.5f);
                    config.screenHeight = (int)Random.Range(200, Screen.height / 1.5f);
                    config.screenPosX = Random.Range(-200, Screen.width - config.screenWidth + 200);
                    config.screenPosY = Random.Range(-200, Screen.height - config.screenHeight + 200);
                    config.thirdPerson = Random.Range(0, 2) == 0;
                    config.renderScale = Random.Range(0.1f, 1.0f);
                    config.posx += Random.Range(-5, 5);
                    config.posy += Random.Range(-2, 2);
                    config.posz += Random.Range(-5, 5);
                    config.angx = Random.Range(0, 360);
                    config.angy = Random.Range(0, 360);
                    config.angz = Random.Range(0, 360);
                }
                else if (CopyConfig == null && cameraName != Plugin.MainCamera)
                {
                    config.screenHeight /= 4;
                    config.screenWidth /= 4;
                }

                config.Position = config.DefaultPosition;
                config.Rotation = config.DefaultRotation;
                config.FirstPersonPositionOffset = config.DefaultFirstPersonPositionOffset;
                config.FirstPersonRotationOffset = config.DefaultFirstPersonRotationOffset;
                config.Save();
                Logger.log.Notice($"Success creating new camera \"{cameraName}\"");
            }
            else
            {
                Logger.log.Notice($"Camera \"{cameraName}\" already exists!");
            }
        }

        internal static string GetNextCameraName()
        {
            int index = 1;
            string cameraName = String.Empty;
            while (true)
            {
                cameraName = $"customcamera{index.ToString()}";
                if (!CameraUtilities.CameraExists(cameraName))
                    break;

                index++;
            }
            return cameraName;
        }

        internal static bool RemoveCamera(CameraPlusBehaviour instance, bool delete = true)
        {
            try
            {
                if (Path.GetFileName(instance.Config.FilePath) != $"{Plugin.MainCamera}.cfg")
                {
                    if (Plugin.cameraController.Cameras.TryRemove(Plugin.cameraController.Cameras.Where(c => c.Value.Instance == instance && c.Key != $"{Plugin.MainCamera}.cfg")?.First().Key, out var removedEntry))
                    {
                        if (delete)
                        {
                            if (File.Exists(removedEntry.Config.FilePath))
                                File.Delete(removedEntry.Config.FilePath);
                        }

                        GL.Clear(false, true, Color.black, 0);
                        GameObject.Destroy(removedEntry.Instance.gameObject);
                        return true;
                    }
                }
                else
                {
                    Logger.log.Warn("One does not simply remove the main camera!");
                }
            }
            catch (Exception ex)
            {
                string msg
                    = ((instance != null && instance.Config != null && instance.Config.FilePath != null)
                    ? $"Could not remove camera with configuration: '{Path.GetFileName(instance.Config.FilePath)}'."
                    : $"Could not remove camera.");

                Logger.log.Error($"{msg} CameraUtilities.RemoveCamera() threw an exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
            return false;
        }

        internal static void SetAllCameraCulling()
        {
            try
            {
                foreach (CameraPlusInstance c in Plugin.cameraController.Cameras.Values.ToArray())
                {
                    c.Instance.SetCullingMask();
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Exception cameras culling! Exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
        }

        internal static void ReloadCameras()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(UnityGame.UserDataPath, Plugin.Name)))
                    Directory.CreateDirectory(Path.Combine(UnityGame.UserDataPath, Plugin.Name));

                string[] files = Directory.GetFiles(Path.Combine(UnityGame.UserDataPath, Plugin.Name));
                if (!Plugin.cameraController.rootConfig.ProfileLoadCopyMethod && Plugin.cameraController.currentProfile != null)
                    files = Directory.GetFiles(Path.Combine(UnityGame.UserDataPath, $".{Plugin.Name.ToLower()}", "Profiles", Plugin.cameraController.currentProfile));

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.EndsWith(".cfg") && !Plugin.cameraController.Cameras.ContainsKey(fileName))
                    {
                        Logger.log.Notice($"Found config {filePath}!");
                        Plugin.cameraController.Cameras.TryAdd(fileName, new CameraPlusInstance(filePath));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Exception while reloading cameras! Exception:" +
                    $" {ex.Message}\n{ex.StackTrace}");
            }
        }

        internal static string[] MovementScriptList()
        {
            string[] spath = Directory.GetFiles(Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts"), "*.json");
            string[] scriptList = new string[spath.Length];
            for (int i = 0; i < spath.Length; i++)
                scriptList[i] = Path.GetFileName(spath[i]);
            return scriptList;
        }
        internal static string CurrentMovementScript(string scriptPath)
        {
            return Path.GetFileName(scriptPath);
        }

        internal static void CreatSeekbarTexture()
        {
            seekBar = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            seekBarBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            seekBar.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 1.0f));
            seekBar.Apply();
            seekBarBackground.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 0.5f));
            seekBarBackground.Apply();
        }

        internal static void ProfileChange(String ProfileName)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(profilePath, "Profiles", ProfileName));
            if (!dir.Exists)
                return;
            ClearCameras();
            Plugin.cameraController.currentProfile = ProfileName;

            if (Plugin.cameraController.rootConfig.ProfileLoadCopyMethod && ProfileName != null)
                SetProfile(ProfileName);
            CameraUtilities.ReloadCameras();
        }

        internal static void ClearCameras()
        {
            var cs = Resources.FindObjectsOfTypeAll<CameraPlusBehaviour>();

            if (Plugin.cameraController.rootConfig.ProfileLoadCopyMethod)
            {
                foreach (var c in cs)
                    CameraUtilities.RemoveCamera(c);
            }
            foreach (var csi in Plugin.cameraController.Cameras.Values)
                GameObject.Destroy(csi.Instance.gameObject);
            Plugin.cameraController.Cameras.Clear();
        }

        public static void CreateExampleScript()
        {
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);
            string defaultScript = Path.Combine(scriptPath, "ExampleMovementScript.json");
            if (!File.Exists(defaultScript))
                File.WriteAllBytes(defaultScript, CustomUtils.GetResource(Assembly.GetExecutingAssembly(), "CameraPlus.Resources.ExampleMovementScript.json"));
        }

        #region ** Profile **

        internal static void CreateMainDirectory()
        {
            DirectoryInfo di = Directory.CreateDirectory(profilePath);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(Path.Combine(profilePath, "Profiles"));
            var a = new DirectoryInfo(Path.Combine(profilePath, "Profiles")).GetDirectories();
            if (a.Length > 0)
                currentlySelected = a.First().Name;
        }

        internal static void SaveCurrent()
        {
            string cPath = cfgPath;
            if (!Plugin.cameraController.rootConfig.ProfileLoadCopyMethod && Plugin.cameraController.currentProfile != null)
            {
                cPath = Path.Combine(profilePath, "Profiles", Plugin.cameraController.currentProfile);
            }
            DirectoryCopy(cPath, Path.Combine(profilePath, "Profiles", GetNextProfileName()), false);
        }

        internal static void SetNext(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(Path.Combine(profilePath, "Profiles")).GetDirectories();
            if (now == null)
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index < dis.Count() - 1)
                    currentlySelected = dis.ElementAtOrDefault(index + 1).Name;
                else
                    currentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
            }
        }

        internal static void TrySetLast(string now = null)
        {
            DirectoryInfo[] dis = new DirectoryInfo(Path.Combine(profilePath, "Profiles")).GetDirectories();
            if (now == null)
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
                return;
            }
            int index = 0;
            var a = dis.Where(x => x.Name == now);
            if (a.Count() > 0)
            {
                index = dis.ToList().IndexOf(a.First());
                if (index == 0 && dis.Length >= 2)
                    currentlySelected = dis.ElementAtOrDefault(dis.Count() - 1).Name;
                else if (index < dis.Count() && dis.Length >= 2)
                    currentlySelected = dis.ElementAtOrDefault(index - 1).Name;
                else
                    currentlySelected = dis.ElementAtOrDefault(0).Name;
            }
            else
            {
                currentlySelected = "None";
                if (dis.Length > 0)
                    currentlySelected = dis.First().Name;
            }
        }

        internal static void DeleteProfile(string name)
        {
            if (Directory.Exists(Path.Combine(profilePath, "Profiles", name)))
                Directory.Delete(Path.Combine(profilePath, "Profiles", name), true);
        }

        internal static string GetNextProfileName(string BaseName = "")
        {
            int index = 1;
            string folName = "CameraPlusProfile";
            string bname;
            if (BaseName == "")
                bname = "CameraPlusProfile";
            else
                bname = BaseName;
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(profilePath, "Profiles"));
            DirectoryInfo[] dirs = dir.GetDirectories($"{bname}*");
            foreach (var dire in dirs)
            {
                folName = $"{bname}{index.ToString()}";
                index++;
            }
            return folName;
        }

        internal static void SetProfile(string name)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(profilePath, "Profiles", name));
            if (!dir.Exists)
                return;
            DirectoryInfo di = new DirectoryInfo(cfgPath);
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            DirectoryCopy(dir.FullName, cfgPath, false);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                return;

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        internal static void DirectoryCreate(string sourceDirName)
        {
            if (!Directory.Exists(sourceDirName))
                Directory.CreateDirectory(sourceDirName);
        }
        #endregion

        internal static IEnumerator Spawn38Cameras()
        {
            lock (Plugin.cameraController.Cameras)
            {
                for (int i = 0; i < 38; i++)
                {
                    AddNewCamera(GetNextCameraName(), null, true);
                    ReloadCameras();

                    yield return null;
                }
            }
        }
    }
}
