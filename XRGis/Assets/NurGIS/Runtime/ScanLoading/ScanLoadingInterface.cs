using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NurGIS.Runtime.ScanLoading // Instantiates scans for LOD test from List  
{
    public static class InstantiateFromList
    {
        // Parent game object for all scans
        private static ScanLoadingHelper Helper => ScanLoadingHelper.Instance;
        
        internal static List<GameObject> InstantiateAllFromList()
        {
            var goList = new List<GameObject>();
            // Instantiates all scans from the scan list
            for (var scanItem = 0; scanItem < Helper.scanPrefabs.Count; scanItem++)
            {
                GameObject scanGameObject = Object.Instantiate(Helper.scanPrefabs[scanItem], new Vector3(0,0, 0), Quaternion.identity);
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
                
                scanGameObject.transform.SetParent(Helper.parentGameObject.transform);
                goList.Add(scanGameObject);
            }
            return goList;
        }
    }
} 




