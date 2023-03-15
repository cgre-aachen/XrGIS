using System.Collections.Generic;
using LODCesium.Terranigma.Runtime.Geolocation;
using LODCesium.Terranigma.Runtime.LevelOfDetail;
using LODCesium.Terranigma.Runtime.ScanLoading;
using UnityEngine;
using UnityMeshSimplifier.Plugins.leHighPerformanceMeshSimplifier.MeshSimplifierSingle;

namespace LODCesium.Terranigma.Runtime
{
    public class Main : MonoBehaviour
    {
        private List<GameObject> _goList;
        
        public bool instantiateScans;
        public bool generateLevelOfDetail;
        public static bool generateLevelOfDetailInformation;
        public bool reuseScans;
        public bool geoReference;
        public bool usePreGeneratedLod;
        public bool useOptimizedMeshSimplifier;
        
        [HideInInspector]
        public float timeToLoadLOD;
        [HideInInspector]
        public bool LODGenerationFinished;

        private void _addbounds(List<GameObject> goList)
        {
            foreach (var go in goList)
            {
                // Format the different LOD levels to be in the same position and scale
                GameObject child0 = go.transform.GetChild(1).gameObject; // _UMS_LODs_
                GameObject grandChild2 = child0.transform.GetChild(2).gameObject; // Level02
                GameObject greatGrandchild2 = grandChild2.transform.GetChild(0).gameObject; // 000_static_default
                MeshRenderer meshRenderer2 = greatGrandchild2.GetComponent<MeshRenderer>();
                Bounds bounds = meshRenderer2.bounds;
                LevelOfDetailAutomaticSystem.bounds.Add(bounds);
            };
        }
        
        private List<GameObject> _addGameObjects(List<LevelOfDetailSystemSwitch> lodSystemSwitches)
        {
            List<GameObject> goList = new List<GameObject>();
            foreach (var lodSystemSwitch in lodSystemSwitches)
            {
                goList.Add(lodSystemSwitch.gameObject);
            }
            return goList;
        }

        private List<GameObject> _allLodContainer;

        private async void Awake()
        {
            generateLevelOfDetailInformation = generateLevelOfDetail;
            LevelOfDetailAutomaticSystem.bounds = new List<Bounds>();
            _allLodContainer = new List<GameObject>();
            
            LevelOfDetailHelper.parentOfAllGameObjectsWithLevelOfDetail = GameObject.Find("LODParent");

            if (generateLevelOfDetail) // Check if instantiateScans is true when we generate LOD
            {
                instantiateScans = true;
            }
            
            if (geoReference) // Check if instantiateScans is true when we Georeference
            {
                instantiateScans = true;
            }
            
            if (instantiateScans) // Instantiates scans
            {
                _goList = InstantiateFromList.InstantiateAllFromList();
            }
            
            if (generateLevelOfDetail) // Generates LOD
            {
                var startTime = Time.time;
                if (useOptimizedMeshSimplifier) // Check if we use the optimized mesh simplifier
                {
                    LODGenerator.useOptimizedMeshSimplifier = true;
                }
                else
                {
                    LODGenerator.useOptimizedMeshSimplifier = false;
                }
                _goList = await LevelOfDetailInterface.GenerateLevelOfDetail(_goList);
                LODGenerationFinished = true;
                
                var endTime = Time.time;
                timeToLoadLOD = endTime - startTime;
            }
            
            if (geoReference) // Spatial reference is set
            {
                GeolocationInterface.GeoReference(_goList);
            }

            if (reuseScans) // Reuses scans
            {
                _goList = ScanLoadingMultiplication.InstantiateMultiplication(_goList);
            }
            
            var helper=gameObject.GetComponent<LevelOfDetailHelper>();
            helper.PopulateLodList();
            helper.SetInitialSwitchState();

            if (usePreGeneratedLod)
            {
                _allLodContainer = _addGameObjects(LevelOfDetailHelper.LevelOfDetailSwitches);
                _addbounds(_allLodContainer);
            }
        }
    }
}
