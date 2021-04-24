using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatLineManager), nameof(BeatLineManager.Start))]
    internal class BeatLineManagerPatch
    {
        public static BeatLineManager Instance { get; private set; }
        static void Postfix(BeatLineManager __instance)
        {
#if DEBUG
            Logger.log.Notice("BeatLineManager Start");
#endif
            Instance = __instance;
        }
    }
    [HarmonyPatch(typeof(EnvironmentSpawnRotation), "OnEnable")]
    internal class EnvironmentSpawnRotationPatch
    {
        public static EnvironmentSpawnRotation Instance { get; private set; }
        static void Postfix(EnvironmentSpawnRotation __instance)
        {
#if DEBUG
            Logger.log.Notice("EnvironmentSpawnRotation Start");
#endif
            Instance = __instance;
        }
    }
}
