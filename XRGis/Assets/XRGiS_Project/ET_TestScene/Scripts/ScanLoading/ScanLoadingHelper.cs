using System;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour to define the amount and number of scans which should be instantiated
namespace XRGiS_Project.ET_TestScene.Scripts.ScanLoading
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

        public GameObject cesiumParent;
    }
}