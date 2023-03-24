// Script to indicate visually which LOD system is activated

using LODCesium.Terranigma.Runtime.LevelOfDetail;
using TMPro;
using UnityEngine;

namespace LODCesium.Terranigma.Runtime.GUI
{
    public class LevelOfDetailSystemDisplay : MonoBehaviour
    {
        public TextMeshProUGUI enabledSystemGUILabel;

        private void Update()
        {
            enabledSystemGUILabel.text = $"Collision System: {LevelOfDetailHelper.ActiveLevelOfDetailSystem}";
        }
    }
}
