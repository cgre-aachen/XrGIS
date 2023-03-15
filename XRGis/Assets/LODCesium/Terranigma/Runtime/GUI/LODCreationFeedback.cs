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
        
        private int _lodLastCount;
        private int _lodDisplayCount;
        private float _totalTimeToLoadLOD;
        private int _totalReductionCount;

        private int _reduction;
        

        private void Start()
        {
            _lodLastCount = 1;
        }
        
        private void Update()
        {
            if (main.useOptimizedMeshSimplifier)
            {
                if (_lodLastCount != LODGenerator.count) //Used for LOD1
                {
                    _lodLastCount+=1;
                    _lodDisplayCount += 1;
                    _totalReductionCount += 1;

                }

                if (main.LODGenerationFinished) // Used for LOD2
                {
                    _lodDisplayCount += 1;
                    main.LODGenerationFinished = false;
                    _totalTimeToLoadLOD += LODGenerator.timeElapsed;
                    _totalReductionCount += 1;
                }
            
                if (_lodDisplayCount == 2) // Used after LOD2
                {
                    _lodDisplayCount = 0;
                    _totalTimeToLoadLOD += LODGenerator.timeElapsed;
                    _totalReductionCount += 1;
                }
            
                if (_lodDisplayCount == 1) // logic to display the reduction
                {
                    _reduction = 50;
                }
                else
                {
                    _reduction = 20;
                }

                if (_totalReductionCount < 30) // Used to exit
                {
                    loadedLODText.text = $"LOD {_lodDisplayCount+1} finished loading";
                    timeToLoadLODText.text = $"Time needed (s): {Mathf.Abs(Mathf.Round(LODGenerator.timeElapsed * 100f) / 100f)}";
                    reductionText.text = $"Reduction: { _reduction}%";
                }
                else
                {
                    loadedLODText.text = "All LODs finished loading";
                    timeToLoadLODText.text = $"Time needed (s): {Mathf.Abs(Mathf.Round(LODGenerator.timeElapsed * 100f) / 100f)}";
                    reductionText.text = "All meshes reduced by: LOD1: 20%, LOD2: 50%";
                }
                totalReductionTimeText.text = $"Total reduction time (s): {Mathf.Abs(Mathf.Round(_totalTimeToLoadLOD * 100f) / 100f)}";
            }

            else
            {
                loadedLODText.text = "All LODs finished loading";
                reductionText.text = "All meshes reduced by: LOD1: 20%, LOD2: 50%";
                totalReductionTimeText.text = $"Total reduction time (s): {Mathf.Abs(Mathf.Round(main.timeToLoadLOD * 100f) / 100f)}";
            }

            
        }
    }
}