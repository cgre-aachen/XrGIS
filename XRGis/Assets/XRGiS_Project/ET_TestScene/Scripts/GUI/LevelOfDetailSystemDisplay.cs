// Script to indicate visually which LOD system is activated

using TMPro;
using UnityEngine;
using XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail;

namespace XRGiS_Project.ET_TestScene.Scripts.GUI
{
    public class LevelOfDetailSystemDisplay : MonoBehaviour
    {
        public TextMeshProUGUI enabledSystemGUILabel;

        private void Update()
        {
            enabledSystemGUILabel.text = $"Collision System: {LevelOfDetailHelper.ActiveLevelOfDetailSystemHelper}";
        }
    }
}
