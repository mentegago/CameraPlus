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
        public Vector3 position = new Vector3();
        public Quaternion rotation = new Quaternion();
        public bool update = false;
        private OscClient Client=null;
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
            if (this.Client!=null)
                Logger.Log($"Instance of {GetType().Name} Starting.", LogLevel.Notice);
            else
                Logger.Log($"Instance of {GetType().Name} Not Starting.", LogLevel.Error);
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
                             position.x, position.y, position.z,
                             rotation.x, rotation.y, rotation.z, rotation.w,
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
