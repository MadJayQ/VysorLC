using System;
using System.Collections;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JakesMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Vysor : BaseUnityPlugin
    {
        private RenderTexture renderTexture;
        private bool isMonitorChanged = false;
        private GameObject helmetCameraNew;
        private bool isSceneLoaded = false;
        private bool isCoroutineStarted = false;
        private int currentTransformIndex;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} starting up JakesMod!");
            renderTexture = new RenderTexture(1024, 1024, 24);
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} done!");
        }

        public void Start()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} Spawned a new vysor!");
            isCoroutineStarted = false;
            while ((UnityEngine.Object)this.helmetCameraNew == (UnityEngine.Object)null)
                this.helmetCameraNew = new GameObject("HelmetCamera");
            if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "InitScene" && SceneManager.GetActiveScene().name != "InitSceneLaunchOptions")
            {
                isSceneLoaded = true;
                StartCoroutine(LoadSceneEnter());
            }
            else
            {
                this.isSceneLoaded = false;
                this.isMonitorChanged = false;
            }
        }

        private IEnumerator LoadSceneEnter()
        {
            yield return (object)new WaitForSeconds(5f);
            this.isCoroutineStarted = true;
            if (GameObject.Find("Environment/HangarShip/Cameras/ShipCamera") != null)
            {
                if (!isMonitorChanged)
                {
                    Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} hooking!");
                    GameObject.Find("Environment/HangarShip/ShipModels2b/MonitorWall/Cube").GetComponent<MeshRenderer>().materials[2].mainTexture = GameObject.Find("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001").GetComponent<MeshRenderer>().materials[2].mainTexture;
                    GameObject.Find("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001").GetComponent<MeshRenderer>().materials[2].mainTexture = (Texture)this.renderTexture;
                    helmetCameraNew.AddComponent<Camera>();
                    helmetCameraNew.GetComponent<Camera>().targetTexture = renderTexture;
                    helmetCameraNew.GetComponent<Camera>().cullingMask = 20649983;
                    helmetCameraNew.GetComponent<Camera>().farClipPlane = 100;
                    helmetCameraNew.GetComponent<Camera>().nearClipPlane = 0.05f;
                    isMonitorChanged = true;
                }
            }
        }

        public void Update()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} update!");
            if (!isSceneLoaded || !isCoroutineStarted)
                return;
            GameObject gameObject = GameObject.Find("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001/CameraMonitorScript");
            this.currentTransformIndex = gameObject.GetComponent<ManualCameraRenderer>().targetTransformIndex;
            TransformAndName radarTarget = gameObject.GetComponent<ManualCameraRenderer>().radarTargets[this.currentTransformIndex];
            if (!radarTarget.isNonPlayer)
            {
                try
                {
                    this.helmetCameraNew.transform.SetPositionAndRotation(radarTarget.transform.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HelmetLights").position + new Vector3(0.0f, 0.0f, 0.0f), radarTarget.transform.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HelmetLights").rotation * Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    DeadBodyInfo[] objectsOfType = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
                    for (int index = 0; index < objectsOfType.Length; ++index)
                    {
                        if (objectsOfType[index].playerScript.playerUsername == radarTarget.name)
                            this.helmetCameraNew.transform.SetPositionAndRotation(((Component)objectsOfType[index]).gameObject.transform.Find("spine.001/spine.002/spine.003").position, ((Component)objectsOfType[index]).gameObject.transform.Find("spine.001/spine.002/spine.003").rotation * Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    }
                }
                catch (NullReferenceException ex)
                {

                }
            }
            else
                this.helmetCameraNew.transform.SetPositionAndRotation(radarTarget.transform.position + new Vector3(0.0f, 1.6f, 0.0f), radarTarget.transform.rotation * Quaternion.Euler(0.0f, -90f, 0.0f));
        }
    }
}
