using System;
using TMPro;
using UnityEngine;
using XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail;

namespace XRGiS_Project.ET_TestScene.Scripts.GUI
{
    public class LevelOfDetailCounter : MonoBehaviour
    {
        private int _lod0Count;
        private int _lod1Count;
        private int _lod2Count;
        public TextMeshProUGUI lod0Text;
        public TextMeshProUGUI lod1Text;
        public TextMeshProUGUI lod2Text;
        
        private GameObject _child;
        private GameObject _grandChild1;
        private GameObject _grandChild2;
        private MeshRenderer _mr;

        private void UpdateLODCounts()
        {
            // LOD0 count
            _lod0Count = 0;
            _lod1Count = 0;
            _lod2Count = 0;

            // Find the gameObjects that are scanPrefabs
            foreach (var lodSwitch in LevelOfDetailHelper.LevelOfDetailSwitches)
            {
                // ActiveLOD system helper can be either UnityBuiltIn or Custom
                var level = LevelOfDetailHelper.ActiveLevelOfDetailSystemHelper switch
                {
                    // lodSwitch is all prefabs in the scene, checks which system is in use and returns the level
                    LevelOfDetailSystem.UnityBuiltIn => lodSwitch.levelOfDetailBuiltInSystem.ActiveLevel,
                    LevelOfDetailSystem.CustomSystem0 => lodSwitch.levelOfDetailCustomSystem0.ActiveLevel,
                    _ => throw new NotImplementedException("Hey you made a new system, eh?")
                };

                switch (level)
                {
                    case 0:
                        _lod0Count++;
                        break;
                    case 1:
                        _lod1Count++;
                        break;
                    case 2:
                        _lod2Count++;
                        break;
                    default:
                        throw new NotImplementedException("Hey you made an additional level of detail, eh?");
                }
            }
        }
        
        private void Update()
        {
            // Get the number of LOD0 meshes and update text
            UpdateLODCounts();
            lod0Text.text = "LOD0: " + _lod0Count;
            lod1Text.text = "LOD1: " + _lod1Count;
            lod2Text.text = "LOD2: " + _lod2Count;
        }
    }
}
