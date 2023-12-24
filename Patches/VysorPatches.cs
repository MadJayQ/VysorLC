using HarmonyLib;
using UnityEngine;

namespace JakesMod.Patches
{
    [HarmonyPatch]
    internal class VysorPatches
    {
        public static void InitVysorCameras()
        {
            GameObject.Find("Environment/HangarShip/Cameras/ShipCamera").AddComponent<Vysor>();
        }

        [HarmonyPatch(typeof (StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void InitCamera(ref ManualCameraRenderer __instance)
        {
            InitVysorCameras();
        }
    }

}