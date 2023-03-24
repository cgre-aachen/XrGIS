using UnityEngine;

namespace LODCesium.Terranigma.Runtime.GUI
{
    public class BillboardScript : MonoBehaviour
    {
        private Camera _mainCamera;
        void Start()
        {
            _mainCamera = Camera.main;
        }
        void LateUpdate()
        {
            transform.LookAt(_mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}
