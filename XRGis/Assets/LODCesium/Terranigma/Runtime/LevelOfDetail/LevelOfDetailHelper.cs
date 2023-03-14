using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityMeshSimplifier;

namespace LODCesium.Terranigma.Runtime.LevelOfDetail
{
    public class LevelOfDetailHelper : MonoBehaviour
    {
        // Helper for system switch
        private static GameObject _parentOfAllGameObjectsWithLevelOfDetail;
        public static readonly List<LevelOfDetailSystemSwitch> LevelOfDetailSwitches = new ();
        
        private static LevelOfDetailSystem _activeLevelOfDetailSystemHelper = LevelOfDetailSystem.CustomSystem0;
        public static LevelOfDetailSystem ActiveLevelOfDetailSystemHelper
        {
            get => _activeLevelOfDetailSystemHelper;
            set
            {
                if (_activeLevelOfDetailSystemHelper != value) // * Check if System is already active
                {
                    _activeLevelOfDetailSystemHelper = value; // * If not update the active system
                    
                    // Loop all the game objects
                    foreach (var lod in LevelOfDetailSwitches)
                    {
                        lod.ActiveLevelOfDetailSystemPerGameObject = value;
                    }
                }
            }
        }
        
        // Sets the options for the creation of LODs
        public static LevelOfDetailHelper Instance { get; private set; }
        
        public SimplificationOptions simplificationOptions = SimplificationOptions.Default;
        
        public bool autoCollectRenderers = true;
        
        public string defaultSaveAssetPath = "D:/sof/Unity/RiderProjects/XRGis/XRGis/Assets/XRGiS_Project/ET_TestScene/Data/TempLOD";
        
        public string saveAssetPath = "MaxLOD/Data/TempLOD";

        public LODLevel[] levels;
        

        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            // Gets all the game objects with LOD at Start
            _parentOfAllGameObjectsWithLevelOfDetail = GameObject.Find("LODParent");
            LevelOfDetailSwitches.AddRange(_parentOfAllGameObjectsWithLevelOfDetail.GetComponentsInChildren<LevelOfDetailSystemSwitch>());
        }
        
        private void Reset()
        {
            simplificationOptions = SimplificationOptions.Default;
            autoCollectRenderers = true;
            saveAssetPath = "MaxLOD/Data/TempLOD";
            levels = new []
            {
                new LODLevel(0.3f, 1f)
                {
                    CombineMeshes = false,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ReceiveShadows = false,
                    SkinnedMotionVectors = false,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off,
                },
                new LODLevel(0.15f, 0.5f)
                {
                    CombineMeshes = false,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ReceiveShadows = false,
                    SkinnedMotionVectors = false,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
                },
                new LODLevel(0f, 0.1f)
                {
                    CombineMeshes = false,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ReceiveShadows = false,
                    SkinnedMotionVectors = false,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
                },
            };
        }

        // Toggle the keystroke to switch between LODs
        private void Update()
        {
            // Use the keystroke E to toggle the collision systems
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (ActiveLevelOfDetailSystemHelper)
                {
                    // Switch to custom system0
                    case LevelOfDetailSystem.UnityBuiltIn:
                        ActiveLevelOfDetailSystemHelper = LevelOfDetailSystem.CustomSystem0;
                        break;
                    // Switch to unity built in system
                    case LevelOfDetailSystem.CustomSystem0:
                        ActiveLevelOfDetailSystemHelper = LevelOfDetailSystem.UnityBuiltIn;
                        break;
                    case LevelOfDetailSystem.NoLod:
                        ActiveLevelOfDetailSystemHelper = LevelOfDetailSystem.UnityBuiltIn;
                        break;
                }
            }    
        }

        // Deletes the files in the LOD folder
        private void OnDestroy()
        {
            string[] filePaths = Directory.GetFiles(defaultSaveAssetPath);
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }
    }
}