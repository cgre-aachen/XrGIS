using System;
using System.Collections.Generic;
using UnityEngine;

namespace LODCesium.Terranigma.Runtime.Camera_Control
{
    public class LODCameraSetup : MonoBehaviour
    {
        private Camera _mainCamera;
        public int loDs;
        public List<float> radius;

        private void CreateLODCollider()
        {
            if (loDs != radius.Count +1)
            {
                throw new Exception("Number of LODs and number of radii do not match.");
            }

            for (var lods = 0; lods < loDs-1; lods++)
            {
                // Instantiate a new game object
                var lodCollider = new GameObject
                {
                    // rename the game object
                    name = "lod" + lods
                };
                // Set the parent of the game object to the main camera
                lodCollider.transform.SetParent(_mainCamera.transform);
                // Set the position of the game object to the position of the main camera
                var transform1 = _mainCamera.transform;
                var position = transform1.position;
                lodCollider.transform.position = new Vector3(position.x,position.y,radius[lods]);

                // Add a sphere collider and a rigidbody to the game object
                lodCollider.AddComponent<Rigidbody>();
                lodCollider.GetComponent<Rigidbody>().angularDrag = 0;
                lodCollider.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                lodCollider.GetComponent<Rigidbody>().useGravity = false;
                // Set the radius of the sphere collider
                lodCollider.AddComponent<SphereCollider>();
                lodCollider.GetComponent<SphereCollider>().radius = radius[lods];
                lodCollider.GetComponent<SphereCollider>().isTrigger = true;
            }
        }

        private void Start()
        {
            // Set the main camera to the main camera
            _mainCamera = Camera.main;
            // Create the LOD colliders
            CreateLODCollider();
        }
    }
}