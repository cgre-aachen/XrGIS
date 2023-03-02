using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// Instantiates scans for LOD test from List  
namespace XRGiS_Project.ET_TestScene.Scripts.ScanLoading
{
    public static class InstantiateFromList
    {
        // Parent game object for all scans
        private static readonly GameObject ParentOfAllGameObjectsWithLevelOfDetail = new("LODParent");
        private static ScanLoadingHelper Helper => ScanLoadingHelper.Instance;
        
        private static float _scale;
        
        internal static List<GameObject> InstantiateAllFromList()
        {
            var goList = new List<GameObject>();
            // Instantiates all scans from the scan list
            for (var scanItem = 0; scanItem < Helper.scanPrefabs.Count; scanItem++)
            {
                ParentOfAllGameObjectsWithLevelOfDetail.transform.SetParent(Helper.cesiumParent.transform);
                // Instantiate each scan
                GameObject scanGameObject = Object.Instantiate(Helper.scanPrefabs[scanItem]);
                
                // Check if the data containing game object is set up as a child of an empty Go, this is necessary for unity mesh reducer
                if (scanGameObject.transform.childCount != 1)
                {
                    var goContainer = new GameObject()
                    {
                        name = scanGameObject.name + "parent"
                    };

                    scanGameObject.name = "default";
                    scanGameObject.transform.parent = goContainer.transform;
                    scanGameObject = goContainer;
                }
                
                // Set an overall parent to all scans
                scanGameObject.transform.SetParent(ParentOfAllGameObjectsWithLevelOfDetail.transform);

                goList.Add(scanGameObject);
            }
            return goList;
        }
    }
} 




