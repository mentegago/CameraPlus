using System.Reflection;
using IPA;
using HarmonyLib;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace CameraPlus
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "CameraPlus";
        public static string MainCamera => "cameraplus";

        private Harmony _harmony;
        internal static CameraPlusController cameraController;
        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            instance = this;
            Logger.log = logger;
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            _harmony = new Harmony("com.brian91292.beatsaber.cameraplus");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            cameraController= new GameObject("CameraPlusController").AddComponent<CameraPlusController>();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            if(cameraController)
                GameObject.Destroy(cameraController);
        }
    }
}
