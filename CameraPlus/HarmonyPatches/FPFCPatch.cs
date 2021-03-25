using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch]
    internal class FPFCPatch
    {
		public static FirstPersonFlyingController instance { get; private set; }
		public static bool isInstanceFPFC => instance != null;
		static void Postfix(FirstPersonFlyingController __instance)
		{
			instance = __instance;
		}
		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(FirstPersonFlyingController), "Start");
			yield return AccessTools.Method(typeof(FirstPersonFlyingController), "OnEnable");
			yield return AccessTools.Method(typeof(FirstPersonFlyingController), "OnDisable");
		}
	}
}
