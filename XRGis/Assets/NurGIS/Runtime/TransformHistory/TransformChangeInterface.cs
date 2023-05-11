using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeInterface : MonoBehaviour
    {
        public List<GameObject> transformGoList;
        
        private void IdentifyLastChangedGogo()
        {
            GameObject go = transformGoList[^1];
            TransformChangeHelper helper = go.GetComponent<TransformChangeHelper>();
            transformGoList.RemoveAt(transformGoList.Count - 1);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && transformGoList.Count > 0)
            {
                IdentifyLastChangedGogo();
            }
        }
    }
}
