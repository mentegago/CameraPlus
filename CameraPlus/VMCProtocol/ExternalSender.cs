using System;
using System.Threading;
using UnityEngine;
using LogLevel = IPA.Logging.Logger.Level;

namespace CameraPlus.VMCProtocol
{
    public class ExternalSender : MonoBehaviour
    {
        public static ExternalSender Instance { get; private set; }
        private System.Threading.Thread thread;
        public Camera camera = null;
        public bool update = false;
        private OscClient Client;
        public  void Awake(){}

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
            this.Client = new OscClient(address, port);
            Logger.Log($"Instance of {GetType().Name} Starting.", LogLevel.Warning);
            this.thread = new System.Threading.Thread(new ThreadStart(() =>
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
                    }
                    catch (Exception e)
                    {
                        Logger.Log($"{camera.name} ExternalSender Coroutine {e}", LogLevel.Error);
                    }
                }
            }));
            this.thread.Start();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

            this.thread.Abort();
            this.Client.Dispose();
        }
    }
}
