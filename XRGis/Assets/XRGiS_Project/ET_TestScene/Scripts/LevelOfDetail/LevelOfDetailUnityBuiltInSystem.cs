// Monobehaviour that tracks the active LOD in the built in unity system

using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail
{
    public class LevelOfDetailUnityBuiltInSystem : MonoBehaviour
    {
        private GameObject _child;
        private GameObject _grandChild1;
        private GameObject _greatGrandChild1;
        private MeshRenderer _mr1;
        
        private GameObject _grandChild2;
        private GameObject _greatGrandChild2;
        private MeshRenderer _mr2;

        public int ActiveLevel
        {
            get
            {
                _child ??= transform.GetChild(0).gameObject;
                _grandChild1 ??= _child.transform.GetChild(0).gameObject;
                _greatGrandChild1 ??= _grandChild1.transform.GetChild(0).gameObject;
                _mr1 ??= _greatGrandChild1.GetComponent<MeshRenderer>();
                
                _grandChild2 ??= _child.transform.GetChild(1).gameObject;
                _greatGrandChild2 ??= _grandChild2.transform.GetChild(0).gameObject;
                _mr2 ??= _greatGrandChild2.GetComponent<MeshRenderer>();

                if (_mr1.isVisible) return 0;
                if (_mr2.isVisible) return 1;
                return 2;
            }
        }
    }
}