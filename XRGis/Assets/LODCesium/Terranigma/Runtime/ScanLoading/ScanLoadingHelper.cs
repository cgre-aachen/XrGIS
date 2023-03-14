using System;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour to define the amount and number of scans which should be instantiated
namespace LODCesium.Terranigma.Runtime.ScanLoading
{
    public class ScanLoadingHelper : MonoBehaviour
    {
        #region Singleton component pattern
        private static ScanLoadingHelper _instance;
        public static ScanLoadingHelper Instance
        {
            get
            {
                _instance ??= FindInstance();
                return _instance;
            }
        }
        private static ScanLoadingHelper FindInstance()
        {
            var instances = FindObjectsOfType<ScanLoadingHelper>();
            if (instances.Length == 1)
            {
                return instances[0];
            }
            throw new Exception("There should only be once instance of a Singleton!");
        }
        #endregion

        public List<GameObject> scanPrefabs;

        public GameObject parentGameObject;
        
        [Tooltip("Defines how often a scan should be reused")]
        public int[] scanCount;
        [Tooltip("The x-coordinate of the starting position")]
        public int xStart = 0;
        [Tooltip("The z-coordinate of the starting position")]
        public int zStart = 0;
        [Tooltip("The distance (x-coordinate) between each new scan")]
        public int xDistance = -10;
        [Tooltip("The distance (z-coordinate) between each new scan")]
        public int zDistance = -10;

    }
}