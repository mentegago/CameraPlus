using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	internal class DisableSmoothCamera
	{
		static bool Prefix()
		{
			return false;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(SmoothCameraController), nameof(SmoothCameraController.ActivateSmoothCameraIfNeeded));
			yield return AccessTools.Method(typeof(SmoothCamera), "OnEnable");
		}
	}
}
