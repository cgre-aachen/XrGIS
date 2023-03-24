using System.Collections.Generic;
using LODCesium.Terranigma.Runtime.Geolocation;
using LODCesium.Terranigma.Runtime.ScanLoading;
using UnityEngine;

namespace LODCesium.Terranigma.Runtime
{
    public class Main : MonoBehaviour
    {
        private List<GameObject> _goList;
        public bool instantiateScans;
        public bool reuseScans;
        public bool geoReference;


        private async void Awake()
        {
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
        
        private void Update()
        {
        }
    }
}
