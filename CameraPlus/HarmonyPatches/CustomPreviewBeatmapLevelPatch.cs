using System.IO;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(CustomPreviewBeatmapLevel), nameof(CustomPreviewBeatmapLevel.GetPreviewAudioClipAsync))]
    internal class CustomPreviewBeatmapLevelPatch
    {
        public static string customLevelPath = string.Empty;
        static void Postfix(CustomPreviewBeatmapLevel __instance)
        {
#if DEBUG
            Logger.log.Notice($"Selected CustomLevel Path :\n {__instance.customLevelPath}");
#endif
            if (File.Exists(Path.Combine(__instance.customLevelPath, "SongScript.json")))
                customLevelPath = Path.Combine(__instance.customLevelPath, "SongScript.json");
            else
                customLevelPath = string.Empty;
        }
    }
}
