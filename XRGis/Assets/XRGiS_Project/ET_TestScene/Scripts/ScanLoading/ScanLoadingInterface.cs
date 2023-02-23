using CesiumForUnity;
using UnityEngine;
using XRGiS_Project.ET_TestScene.Scripts.Geolocation;
using XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail;
using Object = UnityEngine.Object;

// Instantiates scans for LOD test from List  
namespace XRGiS_Project.ET_TestScene.Scripts.ScanLoading
{
    public static class InstantiateFromList
    {
        // Parent game object for all scans
        private static readonly GameObject ParentOfAllGameObjectsWithLevelOfDetail = new("LODParent");
        private static ScanLoadingHelper Helper => ScanLoadingHelper.Instance;
        
        private static GeolocationHelper GeoHelper => GeolocationHelper.Instance;
        
        private static float _scale;
        
        internal static void InstantiateAllFromList()
        {
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

                // Generate LODs
                LevelOfDetailInterface.GenerateLevelOfDetail(scanGameObject);
                // Attach various components to the scan
                var lodSystemSwitch = scanGameObject.AddComponent<LevelOfDetailSystemSwitch>();
                var lodBuiltinSystem = scanGameObject.AddComponent<LevelOfDetailUnityBuiltInSystem>();
                lodSystemSwitch.levelOfDetailBuiltInSystem = lodBuiltinSystem;
                var lodCustomSystem0 = scanGameObject.AddComponent<LevelOfDetailCustomSystem>();
                lodSystemSwitch.levelOfDetailCustomSystem0 = lodCustomSystem0;
                
                // Attach Geo reference objects
                if (Helper.geoReference)
                {
                    scanGameObject.AddComponent<CesiumGlobeAnchor>();
                    scanGameObject.AddComponent<CesiumOriginShift>();
                }
                
                // Set an overall parent to all scans
                scanGameObject.transform.SetParent(ParentOfAllGameObjectsWithLevelOfDetail.transform);
                
                
                // Format the different LOD levels to be in the same position and scale
                GameObject child0 = scanGameObject.transform.GetChild(1).gameObject; // _UMS_LODs_
                GameObject grandChild0 = child0.transform.GetChild(0).gameObject; // Level00
                GameObject greatGrandchild0 = grandChild0.transform.GetChild(0).gameObject; // 000_static_default
                MeshFilter meshFilterLOD0 = greatGrandchild0.GetComponent<MeshFilter>();
                
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
                var collider = scanGameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilterLOD2.mesh;
                
                // Game object is automatically created but not needed
                GameObject unwantedChild0 = scanGameObject.transform.GetChild(0).gameObject;
                unwantedChild0.SetActive(false);

                // Geo reference the scan if wanted
                if (Helper.geoReference)
                {
                    
                    //Adjust the location, location is set to be at the center of the scan
                    var globalAnchor = scanGameObject.GetComponent<CesiumGlobeAnchor>();
                    globalAnchor.longitude = GeoHelper.longitudeCenter[0];
                    globalAnchor.latitude = GeoHelper.latitudeCenter[0];
                    
                    // Adjust the scale, same scale for all objects
                    if (scanItem == 0)
                    { 
                        _scale = GeolocationInterface.GetScale(scanGameObject, meshFilterLOD0, GeoHelper.longitudeScale[0],GeoHelper.longitudeScale[1], GeoHelper.latitudeScale[0], GeoHelper.latitudeScale[1]);
                    }
                    
                    scanGameObject.transform.localScale = new Vector3(_scale, _scale, _scale);

                }
                
                else
                {
                    // Adjust the scale
                    scanGameObject.transform.localScale = new Vector3(Helper.modelScale, Helper.modelScale, Helper.modelScale);
                }
                
                // Recalculate the LOD bounds
                scanGameObject.GetComponent<LODGroup>().RecalculateBounds();
            }
        }
    }
} 




