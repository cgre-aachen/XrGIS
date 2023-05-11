using System.Collections.Generic;
using NurGIS.Runtime.Geolocation;
using NurGIS.Runtime.ScanLoading;
using UnityEngine;
using UnityEngine.Serialization;

namespace NurGIS.Runtime
{
    public class Main : MonoBehaviour
    {
        public static bool isInitialized;
        private List<GameObject> _goList;
        public bool instantiateScans;
        public bool reuseScans;
        public bool geoReference;


        private void Update()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                if (instantiateScans) // Instantiates scans
                {
                    _goList = InstantiateFromList.InstantiateAllFromList();
                }
            
                if (geoReference) // Spatial reference is set
                {
                    GeolocationInterface.GeoReference(_goList);
                }
            
                if (reuseScans) // Reuses scans
                {
                    _goList = ScanLoadingMultiplication.InstantiateMultiplication(_goList);
                }
            }
        }
    }
}
