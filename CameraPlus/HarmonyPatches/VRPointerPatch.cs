using VRUIControls;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(VRPointer), nameof(VRPointer.OnEnable))]
    internal class VRPointerPatch
    {
        public static VRPointer Instance { get; private set; }
        static void Postfix(VRPointer __instance)
        {
#if DEBUG
            Logger.Log("VRPointer Start");
#endif
            Instance = __instance;
        }
    }
}
