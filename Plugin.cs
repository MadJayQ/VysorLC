using HarmonyLib;
using BepInEx;
using JakesMod.Patches;

namespace JakesMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class PluginInit : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        void Awake()
        {
            harmony.PatchAll(typeof(VysorPatches));
        }
    }

}