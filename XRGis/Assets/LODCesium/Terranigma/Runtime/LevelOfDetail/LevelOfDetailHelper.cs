using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityMeshSimplifier.Plugins.leHighPerformanceMeshSimplifier.MeshSimplifierSingle;

namespace LODCesium.Terranigma.Runtime.LevelOfDetail
{
    public class LevelOfDetailHelper : MonoBehaviour
    {
        // Helper for system switch
        public static GameObject parentOfAllGameObjectsWithLevelOfDetail;
        public static readonly List<LevelOfDetailSystemSwitch> LevelOfDetailSwitches = new ();

        [SerializeField] private LevelOfDetailSystem activeLevelOfDetailSystem = LevelOfDetailSystem.NoLod;
        private LevelOfDetailSystem _previousActiveLevelOfDetailSystem;
        private LevelOfDetailAutomaticSystem _levelOfDetailAutomaticSystem;
        
        public static LevelOfDetailSystem ActiveLevelOfDetailSystem
        {
            get => Instance.activeLevelOfDetailSystem;
            private set
            {;
                Instance.activeLevelOfDetailSystem = value; // * If not update the active system
                // Loop all the game objects
                foreach (var lod in LevelOfDetailSwitches)
                {
                    lod.ActiveLevelOfDetailSystemPerGameObject = value;
                }
            }
        }
        
        // Sets the options for the creation of LODs
        public static LevelOfDetailHelper Instance { get; private set; }
        
        public SimplificationOptions simplificationOptions = SimplificationOptions.Default;
        
        public bool autoCollectRenderers = true;
        
        public string defaultSaveAssetPath = "D:/sof/Unity/RiderProjects/XRGis/XRGis/Assets/XRGiS_Project/ET_TestScene/Data/TempLOD";
        
        public string saveAssetPath = "XRGiS_Project/ET_TestScene/Data/TempLOD"; //MaxLOD/Data/TempLOD

        public LODLevel[] levels;

        public void SetInitialSwitchState()
        {
            ActiveLevelOfDetailSystem = activeLevelOfDetailSystem;
        }

        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            
            _previousActiveLevelOfDetailSystem = Instance.activeLevelOfDetailSystem;

            Instance.activeLevelOfDetailSystem = activeLevelOfDetailSystem;
            ActiveLevelOfDetailSystem = Instance.activeLevelOfDetailSystem;
            
            _levelOfDetailAutomaticSystem = GetComponent<LevelOfDetailAutomaticSystem>();
        }

        public void PopulateLodList()
        {
            LevelOfDetailSwitches.AddRange(parentOfAllGameObjectsWithLevelOfDetail.GetComponentsInChildren<LevelOfDetailSystemSwitch>());
        }
        private void Update()
        {
            if (Instance.activeLevelOfDetailSystem != _previousActiveLevelOfDetailSystem)
            {
                _previousActiveLevelOfDetailSystem = ActiveLevelOfDetailSystem;
                ActiveLevelOfDetailSystem = Instance.activeLevelOfDetailSystem;
            }

            if (_levelOfDetailAutomaticSystem.isActiveAndEnabled)
            {
                if (_levelOfDetailAutomaticSystem.IsInBounds()) // if it is in bounds we need to activate the customLOD system
                {
                    Instance.activeLevelOfDetailSystem = LevelOfDetailSystem.CustomSystem0;
                }
                else
                {
                    Instance.activeLevelOfDetailSystem = LevelOfDetailSystem.UnityBuiltIn;
                }
            }


        }
        
        private void Reset()
        {
            simplificationOptions = SimplificationOptions.Default;
            autoCollectRenderers = true;
            saveAssetPath = "XRGiS_Project/ET_TestScene/Data/TempLOD";
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