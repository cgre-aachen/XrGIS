using TMPro;
using UnityEngine;
using UnityMeshSimplifier.Plugins.leHighPerformanceMeshSimplifier.MeshSimplifierSingle;

namespace LODCesium.Terranigma.Runtime.GUI
{
    public class LODCreationFeedback : MonoBehaviour
    {
        public TextMeshProUGUI loadedLODText;
        public TextMeshProUGUI timeToLoadLODText;
        
        private void Update()
        {
            loadedLODText.text = $"Currently loading LOD: {LODGenerator.count}";
            //timeToLoadLODText.text = $"Time to load LOD{}: {}";
        }
    }
}