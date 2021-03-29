using System;
using System.Threading.Tasks;
using UnityEngine;
using LogLevel = IPA.Logging.Logger.Level;

namespace CameraPlus.VMCProtocol
{
    public class ExternalSender : MonoBehaviour
    {
        public static ExternalSender Instance = null;
        private Task task;
        public Camera camera = null;
        public bool update = false;
        private OscClient Client;
        private bool stopThread = false;

        public void SendCameraData(string address="127.0.0.1", int port = 39540)
        {
            if (Instance != null)
            {
                Logger.Log($"Instance of {GetType().Name} already exists, destroying.", LogLevel.Warning);
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
            stopThread = false;
            this.Client = new OscClient(address, port);
            Logger.Log($"Instance of {GetType().Name} Starting.", LogLevel.Notice);
            this.task = Task.Run(() => SendData());
        }

        private void SendData()
        {
            while (true)
            {
                try
                {
                    if (camera && update)
                    {
                        Client.Send("/VMC/Ext/Cam", "Camera", new float[] {
                             camera.transform.position.x, camera.transform.position.y, camera.transform.position.z,
                             camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z, camera.transform.rotation.w,
                             camera.fieldOfView});
                        update = false;
                    }
                    if (stopThread)
                        break;
                }
                catch (Exception e)
                {
                    Logger.Log($"{camera.name} ExternalSender Thread : {e}", LogLevel.Error);
                }
            }
        }
        private void OnDestroy()
        {
            stopThread = true;
            Task.WaitAll(task);
        }
    }
}
