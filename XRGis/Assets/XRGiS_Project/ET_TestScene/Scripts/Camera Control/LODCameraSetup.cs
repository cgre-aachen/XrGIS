using System.Collections.Generic;
using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.Camera_Control
{
    public class LODCameraSetup : MonoBehaviour
    {
        public Camera mainCamera;
        public int loDs;
        public List<float> radius;

        private void CreateLODCollider()
        {
            for (var lods = 0; lods < loDs; lods++)
            {
                // Instantiate a new game object
                var lodCollider = new GameObject
                {
                    // rename the game object
                    name = "lod" + lods
                };
                // Set the parent of the game object to the main camera
                lodCollider.transform.SetParent(mainCamera.transform);
                // Set the position of the game object to the position of the main camera
                lodCollider.transform.position = new Vector3(0,0,radius[lods]);

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
            mainCamera = Camera.main;
            // Create the LOD colliders
            CreateLODCollider();
        }
    }
}