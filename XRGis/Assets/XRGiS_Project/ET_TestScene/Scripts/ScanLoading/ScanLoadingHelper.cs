using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour to define the amount and number of scans which should be instantiated
namespace XRGiS_Project.ET_TestScene.Scripts.ScanLoading
{
    public class ScanLoadingHelper : MonoBehaviour
    {
        public static ScanLoadingHelper Instance { get; private set; }
        
        public List<GameObject> scanPrefabs;
        
        public int modelScale = 1;
        
        public GameObject cesiumParent;
        
        void Awake()
        {
            Instance = this;
        }
    }
}