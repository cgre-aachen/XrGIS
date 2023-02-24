// Collects inputs from LevelOfDetailHelper, class generates LODs

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityMeshSimplifier;

namespace XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail
{
    public static class LevelOfDetailInterface
    {
        private static LevelOfDetailHelper Helper => LevelOfDetailHelper.Instance;
        
        
        public static List<GameObject> GenerateLevelOfDetail(List<GameObject> gameObject)
        {
            var goList = new List<GameObject>();
            if (goList == null) throw new ArgumentNullException(nameof(goList));

            foreach (var go in gameObject)
            {
                LODGenerator.GenerateLODs(go, Helper.levels, Helper.autoCollectRenderers, Helper.simplificationOptions, Helper.saveAssetPath);
                
                // Attach various components to the scan
                var lodSystemSwitch = go.AddComponent<LevelOfDetailSystemSwitch>();
                var lodBuiltinSystem = go.AddComponent<LevelOfDetailUnityBuiltInSystem>();
                lodSystemSwitch.levelOfDetailBuiltInSystem = lodBuiltinSystem;
                var lodCustomSystem0 = go.AddComponent<LevelOfDetailCustomSystem>();
                lodSystemSwitch.levelOfDetailCustomSystem0 = lodCustomSystem0;
                
                // Format the different LOD levels to be in the same position and scale
                GameObject child0 = go.transform.GetChild(1).gameObject; // _UMS_LODs_
                GameObject grandChild0 = child0.transform.GetChild(0).gameObject; // Level00
                GameObject greatGrandchild0 = grandChild0.transform.GetChild(0).gameObject; // 000_static_default

                greatGrandchild0.transform.localPosition = Vector3.zero;
                greatGrandchild0.transform.localScale = Vector3.one;
                
                GameObject grandChild1 = child0.transform.GetChild(1).gameObject; // Level01
                GameObject greatGrandchild1 = grandChild1.transform.GetChild(0).gameObject; // 000_static_default
                
                greatGrandchild1.transform.localPosition = Vector3.zero;
                greatGrandchild1.transform.localScale = Vector3.one;
                
                GameObject grandChild2 = child0.transform.GetChild(2).gameObject; // Level02
                GameObject greatGrandchild2 = grandChild2.transform.GetChild(0).gameObject; // 000_static_default
                MeshFilter meshFilterLOD2 = greatGrandchild2.GetComponent<MeshFilter>();

                greatGrandchild2.transform.localPosition = Vector3.zero;
                greatGrandchild2.transform.localScale = Vector3.one;
                
                // Add Collider System to GameObject and attach the lowest level LOD mesh to it
                var collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilterLOD2.mesh;
                
                // Game object is automatically created but not needed
                GameObject unwantedChild0 = go.transform.GetChild(0).gameObject;
                unwantedChild0.SetActive(false);
                
                // Recalculate the LOD bounds
                go.GetComponent<LODGroup>().RecalculateBounds();
                
                goList.Add(go);
                
            }  return goList;
        }
    } 
}




