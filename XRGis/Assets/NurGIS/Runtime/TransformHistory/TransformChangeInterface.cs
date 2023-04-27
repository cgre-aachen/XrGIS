using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeInterface : MonoBehaviour
    {
        public List<GameObject> transformGoList;
        public bool globalUndoCallParameter;

        private void RevertlastState()
        {
            GameObject go = transformGoList[^1];
            TransformChangeHelper helper = go.GetComponent<TransformChangeHelper>();
            helper.privateUndoTransform = true;
            globalUndoCallParameter = true;
            transformGoList.RemoveAt(transformGoList.Count - 1);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                RevertlastState();
            }  
        }
    }
    
}
