using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LODCesium.Terranigma.Runtime.Camera_Control
{
    public class LODCameraSetup : MonoBehaviour
    {
        private Camera _mainCamera;
        public int loDs;
        public List<float> radii;
        private List<float> _previousRadii;

        private List<Collider> _colliderList;

        private List<Collider> CreateLODCollider()
        {
            List<Collider> colliderList = new List<Collider>();

            if (loDs != radii.Count +1)
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
                var position1 = transform1.position;

                lodCollider.transform.position = position1 + transform1.forward * radii[lods];
                
                // Add a sphere collider and a rigidbody to the game object
                lodCollider.AddComponent<Rigidbody>();
                lodCollider.GetComponent<Rigidbody>().angularDrag = 0;
                lodCollider.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                lodCollider.GetComponent<Rigidbody>().useGravity = false;
                // Set the radius of the sphere collider
                lodCollider.AddComponent<SphereCollider>();
                lodCollider.GetComponent<SphereCollider>().radius = radii[lods];
                lodCollider.GetComponent<SphereCollider>().isTrigger = true;
                
                colliderList.Add(lodCollider.GetComponent<SphereCollider>());
            }

            return colliderList;
        }
        

        private void Start()
        {
            _mainCamera = Camera.main;
            _colliderList = CreateLODCollider();
            _previousRadii = new List<float>(radii);
        }

        private void Update()
        {
            foreach (float singleRadius in radii)
            {
                float previousRadius = _previousRadii[radii.IndexOf(singleRadius)];
                if (Math.Abs(singleRadius - previousRadius) > 0.001)
                {
                    // Update the radius of the sphere collider
                    var transform1 = _mainCamera.transform;
                    Vector3 cameraPosition = transform1.position;
                    Vector3 cameraDirection = transform1.forward;
                    Collider singleCollider = _colliderList[radii.IndexOf(singleRadius)];
                    singleCollider.transform.position = cameraPosition + cameraDirection * singleRadius;
                    singleCollider.GetComponent<SphereCollider>().radius = singleRadius;
                    _previousRadii[radii.IndexOf(singleRadius)] = singleRadius;
                }
            }
        }
    }
}