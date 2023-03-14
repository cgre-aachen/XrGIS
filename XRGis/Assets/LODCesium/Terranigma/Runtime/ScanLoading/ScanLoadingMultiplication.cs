using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LODCesium.Terranigma.Runtime.ScanLoading
{
    public static class ScanLoadingMultiplication
    {
        // Instantiate additional scans if a scan should be reused
        private static ScanLoadingHelper Helper => ScanLoadingHelper.Instance;

        internal static List<GameObject> InstantiateMultiplication(List<GameObject> goList)
        {
            var newGoList = new List<GameObject>();
            
            var x = Helper.xStart;
            var z = Helper.zStart;

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
                    var go = Object.Instantiate(goList[scanItem], new Vector3(x+Helper.xDistance, 0, z), Quaternion.identity);
                    go.transform.SetParent(Helper.parentGameObject.transform);
                    go.name = $"{goList[scanItem].name}_" + i;
                    
                    // Find mesh of LOD0 in nested game objects of scan prefab
                    GameObject child0 = goList[scanItem].transform.GetChild(1).gameObject; // _UMS_LODs_
                    GameObject grandChild0 = child0.transform.GetChild(0).gameObject; // Level00
                    GameObject greatGrandchild0 = grandChild0.transform.GetChild(0).gameObject; // 000_static_default
                    Mesh meshLod0 = greatGrandchild0.GetComponent<MeshFilter>().sharedMesh;
                    

                    // Find mesh of LOD1 in nested game objects of scan prefab
                    GameObject grandChild1 = child0.transform.GetChild(1).gameObject; // Level01
                    GameObject greatGrandchild1 = grandChild1.transform.GetChild(0).gameObject; // 000_static_default
                    Mesh meshLod1 = greatGrandchild1.GetComponent<MeshFilter>().sharedMesh;

                    // Find mesh of LOD2 in nested game objects of scan prefab
                    GameObject grandChild2 = child0.transform.GetChild(2).gameObject; // Level02
                    GameObject greatGrandchild2 = grandChild2.transform.GetChild(0).gameObject; // 000_static_default
                    Mesh meshLod2 = greatGrandchild2.GetComponent<MeshFilter>().sharedMesh;
                    
                    // Clone LOD0 mesh and rename it
                    Mesh meshLod0Clone = Object.Instantiate(meshLod0);
                    meshLod0Clone.name = $"{go.name}_lod0";
                    
                    // Clone LOD1 mesh and rename it
                    Mesh meshLod1Clone = Object.Instantiate(meshLod1);
                    meshLod1Clone.name = $"{go.name}_lod1";
                    
                    // Clone LOD2 mesh and rename it
                    Mesh meshLod2Clone = Object.Instantiate(meshLod2);
                    meshLod2Clone.name = $"{go.name}_lod2";
                    
                    // Find LOD0 child and assign mesh
                    GameObject child0Copy = go.transform.GetChild(1).gameObject;
                    GameObject grandChild0Copy = child0Copy.transform.GetChild(0).gameObject;
                    GameObject greatGrandchild0Copy = grandChild0Copy.transform.GetChild(0).gameObject;
                    MeshFilter meshFilter0 = greatGrandchild0Copy.GetComponent<MeshFilter>();
                    meshFilter0.mesh = meshLod0Clone;
                    
                    // Find LOD1 child and assign mesh
                    GameObject grandChild1Copy = child0Copy.transform.GetChild(1).gameObject;
                    GameObject greatGrandchild1Copy = grandChild1Copy.transform.GetChild(0).gameObject;
                    MeshFilter meshFilter1 = greatGrandchild1Copy.GetComponent<MeshFilter>();
                    meshFilter1.mesh = meshLod1Clone;
                    
                    // Find LOD2 child and assign mesh
                    GameObject grandChild2Copy = child0Copy.transform.GetChild(2).gameObject;
                    GameObject greatGrandchild2Copy = grandChild2Copy.transform.GetChild(0).gameObject;
                    MeshFilter meshFilter2 = greatGrandchild2Copy.GetComponent<MeshFilter>();
                    meshFilter2.mesh = meshLod2Clone;
                    
                    // Add Collider System to GameObject
                    var collider = go.GetComponent<MeshCollider>();
                    collider.sharedMesh = meshLod2Clone;
                    
                    
                    // Update the position of the next scan
                    x += Helper.xDistance;
                    if (x == -50)
                    {
                        x = 0;
                        z += Helper.zDistance;
                    }

                    newGoList.Add(goList[scanItem]);
                }
            }
            return newGoList;
        }
    }
}

