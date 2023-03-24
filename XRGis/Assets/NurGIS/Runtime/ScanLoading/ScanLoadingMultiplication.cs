using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NurGIS.Runtime.ScanLoading
{
    public static class ScanLoadingMultiplication // Instantiate additional scans if a scan should be reused
    {
        private static ScanLoadingHelper Helper => ScanLoadingHelper.Instance;

        internal static List<GameObject> InstantiateMultiplication(List<GameObject> goList)
        {
            var newGoList = new List<GameObject>();
            
            var x = Helper.xStart + Helper.xDistance;
            var z = Helper.zStart + Helper.zDistance;

            if (Helper.scanCount.Length != Helper.scanPrefabs.Count)
            {
                throw new Exception("Select a scan count for each scan!");
            }

            for (var scanItem = 0; scanItem < goList.Count; scanItem++)
            {
                // Generate scanCount amount of copies of each scan
                for (var i = 1; i < Helper.scanCount[scanItem]; i++)
                {
                    // Instantiates a copy of the scan prefab
                    var go = Object.Instantiate(goList[scanItem], new Vector3(x, 0, z), Quaternion.identity);
                    go.transform.SetParent(Helper.parentGameObject.transform);
                    go.name = $"{goList[scanItem].name}_" + i;

                    x += Helper.xDistance;
                    z += Helper.zDistance;

                    newGoList.Add(goList[scanItem]);
                }
            }
            return newGoList;
        }
    }
}

