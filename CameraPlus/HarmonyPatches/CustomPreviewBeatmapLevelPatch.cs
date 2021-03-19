using HarmonyLib;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomPreviewBeatmapLevel), nameof(CustomPreviewBeatmapLevel.GetPreviewAudioClipAsync))]
    internal class CustomPreviewBeatmapLevelPatch
    {
        public static string customLevelPath = string.Empty;
        static void Postfix(CustomPreviewBeatmapLevel __instance)
        {
#if DEBUG
            //Logger.Log($"Selected CustomLevel Path :\n {__instance.customLevelPath}", LogLevel.Notice);
#endif
            customLevelPath = __instance.customLevelPath;
        }
    }
}
