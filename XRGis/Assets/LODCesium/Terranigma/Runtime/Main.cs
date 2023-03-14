using System.Collections.Generic;
using LODCesium.Terranigma.Runtime.Geolocation;
using LODCesium.Terranigma.Runtime.LevelOfDetail;
using LODCesium.Terranigma.Runtime.ScanLoading;
using UnityEngine;

namespace LODCesium.Terranigma.Runtime
{
    public class Main : MonoBehaviour
    {
        private List<GameObject> _goList;
        
        public bool instantiateScans;
        public bool generateLevelOfDetail;
        public bool reuseScans;
        public bool geoReference;
        

        private void Awake()
        {
            if (generateLevelOfDetail)
            {
                instantiateScans = true;
            }
            
            if (geoReference)
            {
                instantiateScans = true;
            }
            
            
            if (instantiateScans)
            {
                _goList = InstantiateFromList.InstantiateAllFromList();
            }
            
            if (generateLevelOfDetail)
            {
                _goList = LevelOfDetailInterface.GenerateLevelOfDetail(_goList);
            }

            if (reuseScans)
            {
                _goList = ScanLoadingMultiplication.InstantiateMultiplication(_goList);
            }

            if (geoReference)
            {
                GeolocationInterface.GeoReference(_goList);
            }
        }
    }
}
