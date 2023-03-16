using TMPro;
using UnityEngine;
using UnityMeshSimplifier.Plugins.leHighPerformanceMeshSimplifier.MeshSimplifierSingle;

namespace LODCesium.Terranigma.Runtime.GUI
{
    public class LODCreationFeedback : MonoBehaviour
    {
        public TextMeshProUGUI loadedLODText;
        public TextMeshProUGUI timeToLoadLODText;
        public TextMeshProUGUI reductionText;
        public TextMeshProUGUI totalReductionTimeText;
        public Main main;

        private int _lodDisplayCount;
        private float _totalTimeToLoadAllLoDs;
        private int _totalReductionCount;
        private int _reduction;
        private int _meshCount;

        private bool _meshCountSwitchLOD1 = true;
        private bool _meshCountSwitchLOD2 = true;

        private void Update()
        {
            if (main.useOptimizedMeshSimplifier)
            {
                
                if (LODGenerator.count % 2 == 1 && LODGenerator.count < 20) // LOD1
                {
                    _lodDisplayCount = 1;
                    _reduction = 20;
                    _totalTimeToLoadAllLoDs += LODGenerator.timetoLoadSingleLod;
                    
                    
                    if (_meshCountSwitchLOD1)
                    {
                        _meshCount++;
                        _meshCountSwitchLOD1 = false;
                    }
                    
                    
                    loadedLODText.text = $"Loading: Mesh {_meshCount}, LOD{_lodDisplayCount}";
                    reductionText.text = $"Reduction: { _reduction}%";
                    timeToLoadLODText.text = $"Time needed (s): {Mathf.Abs(Mathf.Round(LODGenerator.timetoLoadSingleLod * 100f) / 100f)}";
                    totalReductionTimeText.text = $"Total reduction time (s): {Mathf.Abs(Mathf.Round(_totalTimeToLoadAllLoDs * 1f) / 100f)}";
                }
                
                if (LODGenerator.count % 2 == 0 && LODGenerator.count < 20) // LOD2
                {
                    _lodDisplayCount = 2;
                    _reduction = 50;
                    _totalTimeToLoadAllLoDs += LODGenerator.timetoLoadSingleLod;
                    _meshCountSwitchLOD1 = true;

                    loadedLODText.text = $"Loading: Mesh {_meshCount}, LOD{_lodDisplayCount}";
                    reductionText.text = $"Reduction: { _reduction}%";
                    timeToLoadLODText.text = $"Time needed (s): {Mathf.Abs(Mathf.Round(LODGenerator.timetoLoadSingleLod * 100f) / 100f)}";
                    totalReductionTimeText.text = $"Total reduction time (s): {Mathf.Abs(Mathf.Round(_totalTimeToLoadAllLoDs * 1f) / 100f)}";
                }
                
                if (LODGenerator.count == 20) // LOD loading finished
                {
                    loadedLODText.text = "All LODs finished loading";
                    Destroy(timeToLoadLODText);
                    reductionText.text = "All meshes reduced by: LOD1: 20%, LOD2: 50%";
                    totalReductionTimeText.text = $"Total reduction time (s): {Mathf.Abs(Mathf.Round(_totalTimeToLoadAllLoDs * 1f) / 100f)}";
                }
            }

            else // Used when we are not reducing in runtime
            {
                loadedLODText.text = "All LODs finished loading";
                Destroy(timeToLoadLODText);
                reductionText.text = "All meshes reduced by: LOD1: 20%, LOD2: 50%";
                totalReductionTimeText.text = $"Total reduction time (s): {Mathf.Abs(Mathf.Round(main.timeToLoadLOD * 100f) / 100f)}";
            }

            
        }
    }
}