using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.Awake))]
    internal class AudioTimeSyncControllerPatch
    {
        public static AudioTimeSyncController Instance = null;
        static void Postfix(AudioTimeSyncController __instance)
        {
#if DEBUG
            Logger.Log("AudioTimeSyncController Awake");
#endif
            Instance = __instance;
        }
    }

    [HarmonyPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.StopSong))]
    internal class AudioTimeSyncControllerPatch2
    {
        static void Postfix()
        {
            AudioTimeSyncControllerPatch.Instance = null;
        }

    }
}
