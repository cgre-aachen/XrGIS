// Collects inputs from LevelOfDetailHelper, class generates LODs
using UnityEngine;
using UnityMeshSimplifier;

namespace XRGiS_Project.ET_TestScene.Scripts.LevelOfDetail
{
    public static class LevelOfDetailInterface
    {
        private static LevelOfDetailHelper Helper => LevelOfDetailHelper.Instance;
        
        

        public static void GenerateLevelOfDetail(GameObject gameObject)
        {
            LODGenerator.GenerateLODs(gameObject, Helper.levels, Helper.autoCollectRenderers, Helper.simplificationOptions, Helper.saveAssetPath);
        }
    } 
}




