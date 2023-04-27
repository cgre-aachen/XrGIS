using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeHelper : MonoBehaviour
    {
        private Main _mainComponent;
        private TransformChangeInterface _transformChangeInterface;
        private bool _initialState;
        private readonly int _translationThreshold = 1; 
        
        public bool privateUndoTransform;
        
        public List<Vector3> positionList;
        public List<Quaternion> rotationList;
        public List<Vector3> scaleList;
        
        
        private void SaveTransformChange()
        {
            _transformChangeInterface.transformGoList.Add(gameObject);
            positionList.Add(gameObject.transform.localPosition);
            scaleList.Add(gameObject.transform.localScale);
            rotationList.Add(gameObject.transform.localRotation);
        }
        
        private void CheckRedundantState()
        {
            if (positionList.Count > 1)
            {
                if (positionList[^1] == positionList[^2] && scaleList[^1] == scaleList[^2] && rotationList[^1] == rotationList[^2]) // Nothing happened, delete entry
                {
                    positionList.RemoveAt(positionList.Count - 1);
                    scaleList.RemoveAt(scaleList.Count - 1);
                    rotationList.RemoveAt(rotationList.Count - 1);
                    _transformChangeInterface.transformGoList.RemoveAt(_transformChangeInterface.transformGoList.Count - 1);
                }
                else if (positionList[^1].x - positionList[^2].x < _translationThreshold)
                {
                    Debug.Log("Continuous translation detected");
                }
            }
        }
        
        private void UndoTransform()
        {
            positionList.RemoveAt(positionList.Count - 1);
            scaleList.RemoveAt(scaleList.Count - 1);
            rotationList.RemoveAt(rotationList.Count - 1);

            var go = gameObject;
            go.transform.localPosition = positionList[^1];
            go.transform.localScale = scaleList[^1];
            go.transform.localRotation = rotationList[^1];
        }

    

        private void Start()
        {
            _mainComponent = GameObject.Find("Main").GetComponent<Main>();
            _transformChangeInterface = GameObject.Find("Main").GetComponent<TransformChangeInterface>();
            transform.hasChanged = false;
        }
        
        private void Update()
        {
            if (_mainComponent.isInitialized)
            {
                if (_initialState == false) // Add the initial state to the lists
                {
                    _initialState = true;
                    positionList.Add(gameObject.transform.position);
                    scaleList.Add(gameObject.transform.localScale);
                    rotationList.Add(gameObject.transform.rotation);
                }

                if (_initialState)
                {
                    if (transform.hasChanged && _transformChangeInterface.globalUndoCallParameter == false) // Track transform change state
                    {
                        // Start here to yield frames
                        SaveTransformChange();
                        CheckRedundantState();
                        transform.hasChanged = false;
                    }

                    if (privateUndoTransform) // Undo transform
                    {
                        UndoTransform();
                        privateUndoTransform = false;
                        _transformChangeInterface.globalUndoCallParameter = false;
                        transform.hasChanged = false;
                    }
                    
                }


            }
        }
    }
}
