using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	class RoomAdjustPatch
    {
		public static Vector3 position = new Vector3();
		public static Quaternion rotation = new Quaternion();

		static void Postfix(MonoBehaviour __instance)
		{
			position = __instance.transform.position;
			rotation = __instance.transform.rotation;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(VRCenterAdjust), nameof(VRCenterAdjust.Start));
			yield return AccessTools.Method(typeof(VRCenterAdjust), nameof(VRCenterAdjust.HandleRoomCenterDidChange));
		}
	}
}
