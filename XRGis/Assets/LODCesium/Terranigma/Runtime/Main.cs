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
        public bool reuseScans;
        public bool geoReference;

        public bool useOptimizedMeshSimplifier;
        

        private async void Awake()
        {
            LevelOfDetailAutomaticSystem.bounds = new List<Bounds>();
            
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
                if (useOptimizedMeshSimplifier) // Check if we use the optimized mesh simplifier
                {
                    LODGenerator.useOptimizedMeshSimplifier = true;
                }
                else
                {
                    LODGenerator.useOptimizedMeshSimplifier = false;
                }
                _goList = await LevelOfDetailInterface.GenerateLevelOfDetail(_goList);
            }

            if (reuseScans) // Reuses scans
            {
                _goList = ScanLoadingMultiplication.InstantiateMultiplication(_goList);
            }
            
            if (geoReference) // Spatial reference is set
            {
                GeolocationInterface.GeoReference(_goList);
            }
            
            if (generateLevelOfDetail) // Populates the LOD list and sets the initial switch state
            {
                var helper=gameObject.GetComponent<LevelOfDetailHelper>();
                helper.PopulateLodList();
                helper.SetInitialSwitchState();
            }
        }
    }
}
