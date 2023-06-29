using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory.Sandbox
{
    public class TransformChangeInterface : MonoBehaviour
    {
        public List<GameObject> transformGoList;
        
        private void IdentifyLastChangedGo()
        {
            GameObject go = transformGoList[^1];
            ThMono mono = go.GetComponent<ThMono>();
            transformGoList.RemoveAt(transformGoList.Count - 1);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && transformGoList.Count > 0)
            {
                IdentifyLastChangedGo();
            }
        }
    }
}
