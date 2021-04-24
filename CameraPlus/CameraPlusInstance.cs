using System.IO;
using UnityEngine;
using CameraPlus.Behaviours;
using CameraPlus.Configuration;

namespace CameraPlus
{
    internal class CameraPlusInstance
    {
        internal Config Config;
        internal CameraPlusBehaviour Instance;

        internal CameraPlusInstance(string configPath)
        {
            Config = new Config(configPath);

            var gameObj = new GameObject($"CamPlus_{Path.GetFileName(configPath)}");
            Instance = gameObj.AddComponent<CameraPlusBehaviour>();
            Instance.Init(Config);
        }
    }
}
