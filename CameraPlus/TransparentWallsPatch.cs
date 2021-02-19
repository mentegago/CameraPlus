using HarmonyLib;
using UnityEngine;

namespace CameraPlus
{
    /*
    [HarmonyPatch(typeof(ObstacleController))]
    [HarmonyPatch("Init", MethodType.Normal)]
    public class TransparentWallsPatch
    {
        public static int WallLayerMask = 25;
        private static void Postfix(ref ObstacleController __instance)
        {
            Camera.main.cullingMask |= (1 << TransparentWallsPatch.WallLayerMask);//Enables HMD bits because layer 25 is masked by default
            Renderer mesh = __instance.gameObject?.GetComponentInChildren<Renderer>(false);
            if (mesh?.gameObject)
            {
                mesh.gameObject.layer = WallLayerMask;
            }
        }
    }
    */
    [HarmonyPatch(typeof(StretchableObstacle), nameof(StretchableObstacle.SetSizeAndColor))]
    class TransparentWallsPatch
    {
        public static int WallLayerMask = 25;
        static void Postfix(Transform ____obstacleCore, ref ParametricBoxFakeGlowController ____obstacleFakeGlow)
        {
            Camera.main.cullingMask |= (1 << TransparentWallsPatch.WallLayerMask);//Enables HMD bits because layer 25 is masked by default
            if (____obstacleCore != null)
            {
                ____obstacleCore.gameObject.layer = WallLayerMask;

                // No-Bloom inner wall texture thingy
                if (____obstacleFakeGlow.enabled)
                    ____obstacleCore.GetChild(0).gameObject.layer = WallLayerMask;
            }

            //____obstacleFakeGlow.enabled = false;
        }
    }
}
