// MonoBehaviour for the custom collision system 

using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail
{
    public class LevelOfDetailCustomSystem : MonoBehaviour
    {
        private MeshFilter _mf;
        private Mesh _lod0;
        private Mesh _lod1;
        private Mesh _lod2;

        private GameObject _child0;
        private GameObject _grandChild0;
        private GameObject _grandChild1;
        private GameObject _grandChild2;

        // Active level is used in several scripts to determine which LOD is used
        [SerializeField]
        private int activeLevel = 2;
        public int ActiveLevel
        {
            get => activeLevel;
            internal set => activeLevel = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Find go of LOD0 and LOD1 in nested game objects of scan prefab
            _child0 ??= transform.GetChild(1).gameObject;
            _grandChild0 ??= _child0.transform.GetChild(0).gameObject;
            _grandChild1 ??= _child0.transform.GetChild(1).gameObject;
            _grandChild2 ??= _child0.transform.GetChild(2).gameObject;
            
            var colliderGameObject = other.gameObject;
            var colliderName = colliderGameObject.name;

            switch (colliderName)
            {
                case "lod0":
                    _grandChild0.SetActive(true);
                    _grandChild1.SetActive(false);
                    _grandChild2.SetActive(false);
                    ActiveLevel = 0;
                    break;
                case "lod1":
                    _grandChild0.SetActive(false);
                    _grandChild1.SetActive(true);
                    _grandChild2.SetActive(false);
                    ActiveLevel = 1;
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
            // Find go of LOD0 and LOD1 in nested game objects of scan prefab
            _child0 ??= transform.GetChild(1).gameObject;
            _grandChild0 ??= _child0.transform.GetChild(0).gameObject;
            _grandChild1 ??= _child0.transform.GetChild(1).gameObject;
            _grandChild2 ??= _child0.transform.GetChild(2).gameObject;
        
            var colliderGameObject = other.gameObject;
            var colliderName = colliderGameObject.name;

            switch (colliderName)
            {
                case "lod0":
                    _grandChild0.SetActive(false);
                    _grandChild1.SetActive(true);
                    _grandChild2.SetActive(false);
                    ActiveLevel = 1;
                    break;
                case "lod1":
                    _grandChild0.SetActive(false);
                    _grandChild1.SetActive(false);
                    _grandChild2.SetActive(true);
                    ActiveLevel = 2;
                    break;
            }
        }
    }
}

