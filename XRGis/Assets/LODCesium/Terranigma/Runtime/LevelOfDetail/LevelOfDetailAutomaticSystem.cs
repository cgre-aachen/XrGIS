using System.Collections.Generic;
using UnityEngine;

namespace LODCesium.Terranigma.Runtime.LevelOfDetail
{
    public class LevelOfDetailAutomaticSystem : MonoBehaviour
    {

        private Camera _camera;
        private Vector3 _cameraPosition;

        public static List<Bounds> bounds;
        public bool isInBounds;
        
        private void Start()
        {
            _camera = Camera.main;
        }
        
        private void Update()
        {
            _cameraPosition = _camera.transform.position;
            //loop over all bounds and update bool
            foreach (Bounds bound in bounds)
            {
                if (bound.Contains(_cameraPosition))
                {
                    isInBounds = true;
                }
                else
                {
                    isInBounds = false;
                }
            }
        }
    }
}