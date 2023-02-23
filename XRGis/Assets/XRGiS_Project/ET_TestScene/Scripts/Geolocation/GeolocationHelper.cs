using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.Geolocation
{
    public class GeolocationHelper : MonoBehaviour
    {
        #region Singleton component pattern
        private static GeolocationHelper _instance;
        public static GeolocationHelper Instance
        {
            get
            {
                _instance ??= FindInstance();
                return _instance;
            }
        }
        private static GeolocationHelper FindInstance()
        {
            var instances = FindObjectsOfType<GeolocationHelper>();
            if (instances.Length == 1)
            {
                return instances[0];
            }
            throw new Exception("There should only be once instance of a Singleton!");
        }
        #endregion

        //Longitude and latitude need to be in degrees. Currently no other units are supported.
        public List<float> longitudeScale;
        public List<float> latitudeScale;

        //The center of the scan based on an average value of the longitude and latitude.
        public List<double> longitudeCenter;
        public List<double> latitudeCenter;
        
    }
}
