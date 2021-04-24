using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace CameraPlus.HarmonyPatches
{
	[HarmonyPatch]
	internal class LevelDataPatch
	{
		public static IDifficultyBeatmap difficultyBeatmap;
		public static bool is360Level = false;
		public static bool isModdedMap = false;
		public static bool isWallMap = false;

		static SpawnRotationProcessor spawnRotationProcessor = new SpawnRotationProcessor();

		static void Prefix(IDifficultyBeatmap difficultyBeatmap)
		{
#if DEBUG
			Logger.log.Notice("Got level data!");
#endif
			LevelDataPatch.difficultyBeatmap = difficultyBeatmap;

			is360Level = difficultyBeatmap.beatmapData.beatmapEventsData.Any(
				x => x.type.IsRotationEvent() && spawnRotationProcessor.RotationForEventValue(x.value) != 0f
			);
		}

		internal static void Reset()
		{
			is360Level = isModdedMap = isWallMap = false;
			difficultyBeatmap = null;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodBase> TargetMethods()
		{
			foreach (var t in new Type[] { typeof(StandardLevelScenesTransitionSetupDataSO), typeof(MissionLevelScenesTransitionSetupDataSO), typeof(MultiplayerLevelScenesTransitionSetupDataSO) })
				yield return t.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
		}
	}
}
