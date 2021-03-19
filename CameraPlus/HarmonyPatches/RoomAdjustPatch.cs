using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	class RoomAdjustPatch
    {
		public static Vector3 position { get; private set; }
		public static Quaternion rotation { get; private set; }
		public static Vector3 eulerAngles { get; private set; }

		static void Postfix(MonoBehaviour __instance)
		{
			position = __instance.transform.position;
			rotation = __instance.transform.rotation;
			eulerAngles = __instance.transform.eulerAngles;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(VRCenterAdjust), nameof(VRCenterAdjust.Start));
			yield return AccessTools.Method(typeof(VRCenterAdjust), nameof(VRCenterAdjust.HandleRoomCenterDidChange));
		}
	}
}
