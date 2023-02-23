using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.Geolocation
{
    public class GeolocationInterface
    {
        private static float SphericalDistance(float lat1, float lat2, float lon1, float lon2)
        {
            //Convert degree to radians
            var radians = Math.PI / 180;
            
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

        public static float GetScale(GameObject go, MeshFilter meshFilter, float lat1, float lat2, float lon1, float lon2)
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
    }
}
