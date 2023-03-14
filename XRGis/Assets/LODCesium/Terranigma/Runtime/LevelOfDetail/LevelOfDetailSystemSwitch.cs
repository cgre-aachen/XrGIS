// MonoBehaviour allowing for a switch between two level of detail systems

using UnityEngine;

namespace LODCesium.Terranigma.Runtime.LevelOfDetail
{
    public enum LevelOfDetailSystem
    {
        UnityBuiltIn,
        CustomSystem0,
        NoLod,
    }
    
    public class LevelOfDetailSystemSwitch : MonoBehaviour
    {
        // Instances of the two level of detail systems
        public LevelOfDetailCustomSystem levelOfDetailCustomSystem0;
        public LevelOfDetailUnityBuiltInSystem levelOfDetailBuiltInSystem;

        private MeshCollider _meshCollider;
        private LODGroup _lodGroup;
        private Camera _camera;
        private SphereCollider _cameraCollider0;
        private SphereCollider _cameraCollider1;
        
        private GameObject _child0;
        private GameObject _grandChild0;
        private GameObject _grandChild1;
        private GameObject _grandChild2;
        
        
        public LevelOfDetailSystem ActiveLevelOfDetailSystemPerGameObject
        {
            set
            {
                switch (value)
                {
                    case LevelOfDetailSystem.UnityBuiltIn:
                        // Disable custom system and enable Unity System
                        _meshCollider ??= GetComponent<MeshCollider>();
                        _meshCollider.enabled = false;
                        
                        _lodGroup ??= GetComponent<LODGroup>();
                        _lodGroup.enabled = true;

                        _child0 ??= transform.GetChild(1).gameObject;
                        _grandChild0 ??= _child0.transform.GetChild(0).gameObject;
                        _grandChild1 ??= _child0.transform.GetChild(1).gameObject;
                        _grandChild2 ??= _child0.transform.GetChild(2).gameObject;
                        
                        _grandChild0.SetActive(true);
                        _grandChild1.SetActive(true);
                        _grandChild2.SetActive(true);

                        break;
                    
                    case LevelOfDetailSystem.CustomSystem0:
                        // Disable Unity system and enable custom system
                        _meshCollider ??= GetComponent<MeshCollider>();
                        _meshCollider.enabled = true;
                        
                        _lodGroup ??= GetComponent<LODGroup>();
                        _lodGroup.enabled = false;

                        _child0 ??= transform.GetChild(1).gameObject;
                        _grandChild0 ??= _child0.transform.GetChild(0).gameObject;
                        _grandChild1 ??= _child0.transform.GetChild(1).gameObject;
                        _grandChild2 ??= _child0.transform.GetChild(2).gameObject;
                        
                        // get the camera collider
                        _camera ??= Camera.main;
                        if (_camera != null) _cameraCollider0 ??= _camera.transform.Find("lod0").GetComponentInChildren<SphereCollider>();
                        if (_camera != null) _cameraCollider1 ??= _camera.transform.Find("lod1").GetComponentInChildren<SphereCollider>();

                        //Set the default by checking if there is an intersection between sphere collider and mesh collider
                        if (_cameraCollider0.bounds.Intersects(_meshCollider.bounds))
                        {
                            _grandChild0.SetActive(true);
                            _grandChild1.SetActive(false);
                            _grandChild2.SetActive(false);
                            levelOfDetailCustomSystem0.ActiveLevel = 0;
                        }
                        
                        else if (_cameraCollider1.bounds.Intersects(_meshCollider.bounds))
                        {
                            _grandChild0.SetActive(false);
                            _grandChild1.SetActive(true);
                            _grandChild2.SetActive(false);
                            levelOfDetailCustomSystem0.ActiveLevel = 1;
                        }
                            
                        else
                        {
                            _grandChild0.SetActive(false);
                            _grandChild1.SetActive(false);
                            _grandChild2.SetActive(true);
                            levelOfDetailCustomSystem0.ActiveLevel = 2;
                        }
                        break;
                    
                    case LevelOfDetailSystem.NoLod:
                        _meshCollider ??= GetComponent<MeshCollider>();
                        _meshCollider.enabled = false;
                        
                        _lodGroup ??= GetComponent<LODGroup>();
                        _lodGroup.enabled = false;

                        _child0 ??= transform.GetChild(1).gameObject;
                        _grandChild0 ??= _child0.transform.GetChild(0).gameObject;
                        _grandChild1 ??= _child0.transform.GetChild(1).gameObject;
                        _grandChild2 ??= _child0.transform.GetChild(2).gameObject;
                        
                        _grandChild0.SetActive(true);
                        _grandChild1.SetActive(false);
                        _grandChild2.SetActive(false);
                        break;
                }
            }
        }
        
        private void Start()
        {
            // Set Default LOD System and LOD Level
            _lodGroup = GetComponent<LODGroup>();
            _lodGroup.enabled = false;
        }
    }
}