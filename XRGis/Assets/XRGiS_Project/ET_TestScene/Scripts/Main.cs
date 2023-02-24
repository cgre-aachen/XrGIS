using System.Collections.Generic;
using UnityEngine;
using XRGiS_Project.ET_TestScene.Scripts.Geolocation;
using XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail;
using XRGiS_Project.ET_TestScene.Scripts.ScanLoading;

namespace XRGiS_Project.ET_TestScene.Scripts
{
    public class Main : MonoBehaviour
    {
        private List<GameObject> _goList;
        
        public bool instantiateScans = true;
        public bool generateLevelOfDetail = true;
        public bool geoReference = true;
        
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
            
            if (geoReference)
            {
                _goList = GeolocationInterface.GeoReference(_goList);
            }
        }
    }
}
