using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.Geolocation
{
    public class GeolocationHelper : MonoBehaviour
    {
        public static GeolocationHelper Instance { get; private set; }

        //Longitude and latitude need to be in degrees. Currently no other units are supported.
        public List<float> longitudeScale;
        public List<float> latitudeScale;

        //The center of the scan based on an average value of the longitude and latitude.
        public double longitudeCenter;
        public double latitudeCenter;

        private void Awake()
        {
            Instance = this;
        }
    }
}
