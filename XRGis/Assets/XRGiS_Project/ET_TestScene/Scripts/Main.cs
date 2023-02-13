using UnityEngine;
using XRGiS_Project.ET_TestScene.Scripts.ScanLoading;

namespace XRGiS_Project.ET_TestScene.Scripts
{
    public class Main : MonoBehaviour
    {
        // Instantiates all scans
        private void Awake()
        {
            InstantiateFromList.InstantiateAllFromList();
        }
    }
}
