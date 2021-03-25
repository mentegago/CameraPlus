using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using IPA.Utilities;
using LogLevel = IPA.Logging.Logger.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.InteropServices;
using CameraPlus.HarmonyPatches;

namespace CameraPlus
{
    public class CameraMovement : MonoBehaviour
    {
        protected CameraPlusBehaviour _cameraPlus;
        protected bool dataLoaded = false;
        protected CameraData data = new CameraData();
        protected Vector3 StartPos = Vector3.zero;
        protected Vector3 EndPos = Vector3.zero;
        protected Vector3 StartRot = Vector3.zero;
        protected Vector3 EndRot = Vector3.zero;
        protected float StartFOV = 0;
        protected float EndFOV = 0;
        protected bool easeTransition = true;
        protected float movePerc;
        protected int eventID;
        protected float movementStartTime, movementEndTime, movementNextStartTime;
        protected DateTime movementStartDateTime, movementEndDateTime, movementDelayEndDateTime;
        protected bool _paused = false;
        protected DateTime _pauseTime;

        public class Movements
        {
            public Vector3 StartPos;
            public Vector3 StartRot;
            public float StartFOV;
            public Vector3 EndPos;
            public Vector3 EndRot;
            public float EndFOV;
            public float Duration;
            public float Delay;
            public bool EaseTransition = true;
        }
        public class CameraData
        {
            public bool ActiveInPauseMenu = true;
            public List<Movements> Movements = new List<Movements>();
            
            public bool LoadFromJson(string jsonString)
            {
                Movements.Clear();
                MovementScriptJson movementScriptJson=null;
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string sepCheck = (sep == "." ? "," : ".");
                try
                {
                    movementScriptJson = JsonConvert.DeserializeObject<MovementScriptJson>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Log($"JSON file syntax error. {ex.Message}", LogLevel.Error);
                }
                if (movementScriptJson != null && movementScriptJson.Jsonmovement !=null)
                {
                    if (movementScriptJson.ActiveInPauseMenu != null)
                        ActiveInPauseMenu = System.Convert.ToBoolean(movementScriptJson.ActiveInPauseMenu);

                    foreach (JSONMovement jsonmovement in movementScriptJson.Jsonmovement)
                    {
                        Movements newMovement = new Movements();
                        StartPos startPos = jsonmovement.startPos;
                        StartRot startRot = jsonmovement.startRot;
 
                        if (startPos.x != null) newMovement.StartPos = new Vector3(float.Parse(startPos.x.Contains(sepCheck) ? startPos.x.Replace(sepCheck, sep) : startPos.x), 
                                                                                    float.Parse(startPos.y.Contains(sepCheck) ? startPos.y.Replace(sepCheck, sep) : startPos.y), 
                                                                                    float.Parse(startPos.z.Contains(sepCheck) ? startPos.z.Replace(sepCheck, sep) : startPos.z));
                        if (startRot.x != null) newMovement.StartRot = new Vector3(float.Parse(startRot.x.Contains(sepCheck) ? startRot.x.Replace(sepCheck, sep) : startRot.x),
                                                                                    float.Parse(startRot.y.Contains(sepCheck) ? startRot.y.Replace(sepCheck, sep) : startRot.y),
                                                                                    float.Parse(startRot.z.Contains(sepCheck) ? startRot.z.Replace(sepCheck, sep) : startRot.z));
                        if (startPos.FOV != null)
                            newMovement.StartFOV = float.Parse(startPos.FOV.Contains(sepCheck) ? startPos.FOV.Replace(sepCheck, sep) : startPos.FOV);
                        else
                            newMovement.StartFOV = 0;
                        EndPos endPos = jsonmovement.endPos;
                        EndRot endRot = jsonmovement.endRot;

                        if (endPos.x != null) newMovement.EndPos = new Vector3(float.Parse(endPos.x), float.Parse(endPos.y), float.Parse(endPos.z));
                        if (endRot.x != null) newMovement.EndRot = new Vector3(float.Parse(endRot.x), float.Parse(endRot.y), float.Parse(endRot.z));
                        if (endPos.x != null) newMovement.EndPos = new Vector3(float.Parse(endPos.x.Contains(sepCheck) ? endPos.x.Replace(sepCheck, sep) : endPos.x),
                                                                                    float.Parse(endPos.y.Contains(sepCheck) ? endPos.y.Replace(sepCheck, sep) : endPos.y),
                                                                                    float.Parse(endPos.z.Contains(sepCheck) ? endPos.z.Replace(sepCheck, sep) : endPos.z));
                        if (endRot.x != null) newMovement.EndRot = new Vector3(float.Parse(endRot.x.Contains(sepCheck) ? endRot.x.Replace(sepCheck, sep) : endRot.x),
                                                                                    float.Parse(endRot.y.Contains(sepCheck) ? endRot.y.Replace(sepCheck, sep) : endRot.y),
                                                                                    float.Parse(endRot.z.Contains(sepCheck) ? endRot.z.Replace(sepCheck, sep) : endRot.z));
                        if (endPos.FOV != null)
                            newMovement.EndFOV = float.Parse(endPos.FOV.Contains(sepCheck) ? endPos.FOV.Replace(sepCheck, sep) : endPos.FOV);
                        else
                            newMovement.EndFOV = 0;

                        if (jsonmovement.Delay != null) newMovement.Delay = float.Parse(jsonmovement.Delay.Contains(sepCheck) ? jsonmovement.Delay.Replace(sepCheck,sep) : jsonmovement.Delay);
                        if (jsonmovement.Duration != null) newMovement.Duration = Mathf.Clamp(float.Parse(jsonmovement.Duration.Contains(sepCheck) ? jsonmovement.Duration.Replace(sepCheck, sep) : jsonmovement.Duration), 0.01f, float.MaxValue); // Make sure duration is at least 0.01 seconds, to avoid a divide by zero error
                        
                        if (jsonmovement.EaseTransition != null)
                            newMovement.EaseTransition = System.Convert.ToBoolean(jsonmovement.EaseTransition);

                        Movements.Add(newMovement);
                    }
                    return true;
                }
                return false;
            }
        }

        public virtual void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (to.name == "GameCore")
            {
                var gp = Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault();
                if (gp && dataLoaded && !data.ActiveInPauseMenu)
                {
                    gp.didResumeEvent += Resume;
                    gp.didPauseEvent += Pause;
                }
            }
        }

        protected void Update()
        {
            if (!dataLoaded || _paused) return;

            if (_cameraPlus.Config.movementAudioSync)
            {
                if (AudioTimeSyncControllerPatch.Instance == null)
                    return;

                if (movementNextStartTime <= AudioTimeSyncControllerPatch.Instance.songTime)
                    UpdatePosAndRot();

                float difference = movementEndTime - movementStartTime;
                float current = AudioTimeSyncControllerPatch.Instance.songTime - movementStartTime;
                if (difference != 0)
                    movePerc = Mathf.Clamp(current / difference, 0, 1);
            }
            else
            {
                if (movePerc == 1 && movementDelayEndDateTime <= DateTime.Now)
                    UpdatePosAndRot();

                long differenceTicks = (movementEndDateTime - movementStartDateTime).Ticks;
                long currentTicks = (DateTime.Now - movementStartDateTime).Ticks;
                movePerc = Mathf.Clamp((float)currentTicks / (float)differenceTicks, 0, 1);
            }
            _cameraPlus.ThirdPersonPos = LerpVector3(StartPos, EndPos, Ease(movePerc));
            _cameraPlus.ThirdPersonRot = LerpVector3(StartRot, EndRot, Ease(movePerc));
            _cameraPlus.FOV(Mathf.Lerp(StartFOV,EndFOV,Ease(movePerc)));
        }

        protected Vector3 LerpVector3(Vector3 from, Vector3 to, float percent)
        {
            return new Vector3(Mathf.LerpAngle(from.x, to.x, percent), Mathf.LerpAngle(from.y, to.y, percent), Mathf.LerpAngle(from.z, to.z, percent));
        }

        public virtual bool Init(CameraPlusBehaviour cameraPlus, bool useSongSpecificScript)
        {
            _cameraPlus = cameraPlus;
            Plugin.Instance.ActiveSceneChanged += OnActiveSceneChanged;

            if (useSongSpecificScript && File.Exists(Path.Combine(CustomPreviewBeatmapLevelPatch.customLevelPath, "SongScript.json")))
            {
                Logger.Log($"{Path.Combine(CustomPreviewBeatmapLevelPatch.customLevelPath, "SongScript.json")}", LogLevel.Notice);
                return LoadCameraData(Path.Combine(CustomPreviewBeatmapLevelPatch.customLevelPath, "SongScript.json"));
            }
            else
                return LoadCameraData(cameraPlus.Config.movementScriptPath);
        }

        public virtual void Shutdown()
        {
            Plugin.Instance.ActiveSceneChanged -= OnActiveSceneChanged;
            Destroy(this);
        }

        public void Pause()
        {
            if (_paused) return;

            _paused = true;
            _pauseTime = DateTime.Now;
        }

        public void Resume()
        {
            if (!_paused) return;

            _paused = false;
        }

        protected bool LoadCameraData(string pathFile)
        {
            string path="";
            if (File.Exists(pathFile))
                path = pathFile;
            else if (File.Exists(Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts", pathFile)))
                path = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts", pathFile);
            else if (File.Exists(Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts", Path.GetFileName(pathFile))))
                path = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts", Path.GetFileName(pathFile));

            if (File.Exists(path))
            {
                string jsonText = File.ReadAllText(path);
                if (data.LoadFromJson(jsonText))
                {
                    Logger.Log("Populated CameraData");

                    if (data.Movements.Count == 0)
                    {
                        Logger.Log("No movement data!");
                        return false;
                    }
                    eventID = 0;
                    UpdatePosAndRot();
                    dataLoaded = true;

                    Logger.Log($"Found {data.Movements.Count} entries in: {path}");
                    return true;
                }
            }
            return false;
        }

        protected void FindShortestDelta(ref Vector3 from, ref Vector3 to)
        {
            if(Mathf.DeltaAngle(from.x, to.x) < 0)
                from.x += 360.0f;
            if (Mathf.DeltaAngle(from.y, to.y) < 0)
                from.y += 360.0f;
            if (Mathf.DeltaAngle(from.z, to.z) < 0)
                from.z += 360.0f;
        }

        protected void UpdatePosAndRot()
        {
            if (eventID >= data.Movements.Count)
                eventID = 0;

            easeTransition = data.Movements[eventID].EaseTransition;

            StartRot = new Vector3(data.Movements[eventID].StartRot.x, data.Movements[eventID].StartRot.y, data.Movements[eventID].StartRot.z);
            StartPos = new Vector3(data.Movements[eventID].StartPos.x, data.Movements[eventID].StartPos.y, data.Movements[eventID].StartPos.z);

            EndRot = new Vector3(data.Movements[eventID].EndRot.x, data.Movements[eventID].EndRot.y, data.Movements[eventID].EndRot.z);
            EndPos = new Vector3(data.Movements[eventID].EndPos.x, data.Movements[eventID].EndPos.y, data.Movements[eventID].EndPos.z);

            if (data.Movements[eventID].StartFOV != 0)
                StartFOV = data.Movements[eventID].StartFOV;
            else
                StartFOV = _cameraPlus.Config.fov;
            if (data.Movements[eventID].EndFOV != 0)
                EndFOV = data.Movements[eventID].EndFOV;
            else
                EndFOV = _cameraPlus.Config.fov;

            FindShortestDelta(ref StartRot, ref EndRot);

            if (_cameraPlus.Config.movementAudioSync)
            {
                movementStartTime = movementNextStartTime;
                movementEndTime = movementNextStartTime + data.Movements[eventID].Duration;
                movementNextStartTime = movementEndTime + data.Movements[eventID].Delay;
            }
            else
            {
                movementStartDateTime = DateTime.Now;
                movementEndDateTime = movementStartDateTime.AddSeconds(data.Movements[eventID].Duration);
                movementDelayEndDateTime = movementStartDateTime.AddSeconds(data.Movements[eventID].Duration + data.Movements[eventID].Delay);
            }

            eventID++;
        }

        protected float Ease(float p)
        {
            if (!easeTransition)
                return p;

            if (p < 0.5f) //Cubic Hopefully
            {
                return 4 * p * p * p;
            }
            else
            {
                float f = ((2 * p) - 2);
                return 0.5f * f * f * f + 1;
            }
        }

        public static void CreateExampleScript()
        {
            string path = Path.Combine(UnityGame.UserDataPath, Plugin.Name, "Scripts");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string defaultScript = Path.Combine(path, "ExampleMovementScript.json");
            if (!File.Exists(defaultScript))
                File.WriteAllBytes(defaultScript, Utils.GetResource(Assembly.GetExecutingAssembly(), "CameraPlus.Resources.ExampleMovementScript.json"));
        }
    }
}
