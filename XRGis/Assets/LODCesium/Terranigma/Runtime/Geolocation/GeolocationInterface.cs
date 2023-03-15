using System;
using System.Collections.Generic;
using System.Linq;
using CesiumForUnity;
using UnityEngine;
using XRGiS_Project.ET_TestScene.Scripts.Geolocation;

namespace LODCesium.Terranigma.Runtime.Geolocation
{
    public static class GeolocationInterface
    {
        
        private static GeolocationHelper Helper => GeolocationHelper.Instance;
        
        
        private static float SphericalDistance(float lat1, float lat2, float lon1, float lon2)
        {
            //Convert degree to radians
            const double radians = Math.PI / 180;
            
            var lat1Rad = lat1 * radians;
            var lat2Rad = lat2 * radians;
            var lon1Rad = lon1 * radians;
            var lon2Rad = lon2 * radians;
            
            // Spherical distance between two points on the earth
            var phi1 = (float)(0.5*Math.PI - lat1Rad);
            var phi2 = (float)(0.5*Math.PI - lat2Rad);
            const float r = (float)(0.5 * (6378137 + 6356752)); // mean radius of the earth in meters
            
            var t = Math.Sin(phi1) * Math.Sin(phi2) * Math.Cos(lon1Rad - lon2Rad) + Math.Cos(phi1) * Math.Cos(phi2);
            var d = r * (float) Math.Acos(t);

            return d;
        }

        private static float GetScale(MeshFilter meshFilter, float lat1, float lat2, float lon1, float lon2)
        {
            // gets the scale of an gameObject go assuming that the scale is the same for all axis
            
            // Unity values
            var bounds = meshFilter.sharedMesh.bounds;
            var xMin = bounds.min.x;
            var xMax = bounds.max.x;
            var unityDistance = xMax - xMin;
            
            // real world values
            var realDistance = SphericalDistance(lat1, lat2, lon1, lon2);
            
            // scale
            var scalex =  realDistance / (unityDistance);

                return scalex;
        }
        
        public static void GeoReference(List<GameObject> gameObjects)
        {
            var scale = new float();
            // Looping over all scans
            foreach (GameObject go in gameObjects)
            {
                go.AddComponent<CesiumOriginShift>();
                go.AddComponent<CesiumGlobeAnchor>();
                
                //Getting the scale of the first scan, apply the same scale to all
                if (gameObjects.First() == go)
                {
                    
                    /*
                    GameObject child0 = go.transform.GetChild(1).gameObject; // _UMS_LODs_
                    GameObject grandChild0 = child0.transform.GetChild(0).gameObject; // Level00
                    GameObject greatGrandchild0 = grandChild0.transform.GetChild(0).gameObject; // 000_static_default
                    MeshFilter meshFilterLOD0 = greatGrandchild0.GetComponent<MeshFilter>();
                    */
                    
                    GameObject child0 = go.transform.GetChild(0).gameObject;
                    MeshFilter meshFilterLOD0 = child0.GetComponent<MeshFilter>();
                    scale = GetScale(meshFilterLOD0, Helper.longitudeScale[0],Helper.longitudeScale[1], Helper.latitudeScale[0], Helper.latitudeScale[1]);
                }
                
                
                // Set the lon lat according to a single center
                var globalAnchor = go.GetComponent<CesiumGlobeAnchor>();
                globalAnchor.longitude = Helper.longitudeCenter[0];
                globalAnchor.latitude = Helper.latitudeCenter[0];
                
                go.transform.localScale = new Vector3(scale, scale, scale);
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
