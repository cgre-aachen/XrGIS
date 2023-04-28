using System;
using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeHelper : MonoBehaviour
    {
        private Main _mainComponent;
        private TransformChangeInterface _transformChangeInterface;
        private bool _initialState;
        
        private readonly double _translationThreshold = 1;

        private int _positionOfContinuousTransformStart;
        private int _positionOfContinuousTransformGoListStart;
        private bool _continuousTransform;
        private bool _continuousTransformEnded;

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
        
        private void RemoveEmptyTransform()
        {
            if (positionList.Count > 1)
            {
                if (positionList[^1] == positionList[^2] && scaleList[^1] == scaleList[^2] && rotationList[^1] == rotationList[^2] && _continuousTransform == false) // Nothing happened, delete entry
                {
                    RemoveLastTransformInList();
                    _transformChangeInterface.transformGoList.RemoveAt(_transformChangeInterface.transformGoList.Count - 1);
                }
                
                else if (positionList[^1] == positionList[^2] && scaleList[^1] == scaleList[^2] &&
                         rotationList[^1] == rotationList[^2] && _continuousTransform)
                {
                    positionList.RemoveRange(_positionOfContinuousTransformStart, positionList.Count - _positionOfContinuousTransformStart -1);
                    scaleList.RemoveRange(_positionOfContinuousTransformStart, scaleList.Count - _positionOfContinuousTransformStart -1);
                    rotationList.RemoveRange(_positionOfContinuousTransformStart, rotationList.Count - _positionOfContinuousTransformStart -1);
                    _transformChangeInterface.transformGoList.RemoveRange(_positionOfContinuousTransformGoListStart, _transformChangeInterface.transformGoList.Count - _positionOfContinuousTransformGoListStart -1);
                
                    _continuousTransform = false;
                    transform.hasChanged = false;
                    _continuousTransformEnded = true;
                }
            }
        }

        private void DetectContinuousTransform()
        {
            if (positionList.Count > 1 && _continuousTransform == false && _continuousTransformEnded == false)
            {
                if (Math.Abs(positionList[^1].x - positionList[^2].x) < _translationThreshold && Math.Abs(positionList[^1].x - positionList[^2].x) > 0 ||
                Math.Abs(positionList[^1].y - positionList[^2].y) < _translationThreshold && Math.Abs(positionList[^1].y - positionList[^2].y) > 0 ||
                Math.Abs(positionList[^1].z - positionList[^2].z) < _translationThreshold && Math.Abs(positionList[^1].z - positionList[^2].z) > 0)
                {
                    _positionOfContinuousTransformStart = positionList.Count - 1;
                    _positionOfContinuousTransformGoListStart = _transformChangeInterface.transformGoList.Count - 1;
                    _continuousTransform = true;
                }
            }
            
            else
            {
                _continuousTransformEnded = false;
            }
        }
        
        private void RemoveLastTransformInList()
        {
            positionList.RemoveAt(positionList.Count - 1);
            scaleList.RemoveAt(scaleList.Count - 1);
            rotationList.RemoveAt(rotationList.Count - 1);
        }
        
        private void UndoTransform()
        {
            RemoveLastTransformInList();
            var go = gameObject; // Set the transform of the go to the new last entry
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
            if (_mainComponent.isInitialized) // Look if the initial state was set in main()
            {
                if (_initialState == false) // Add the initial state to the lists
                {
                    _initialState = true;
                    positionList.Add(gameObject.transform.position);
                    scaleList.Add(gameObject.transform.localScale);
                    rotationList.Add(gameObject.transform.rotation);
                }

                else
                {
                    if (transform.hasChanged && _transformChangeInterface.globalUndoCallParameter == false) // Track transform change state
                    {
                        // Start here to yield frames
                        SaveTransformChange();
                        RemoveEmptyTransform();
                        DetectContinuousTransform();
                        
                        if (_continuousTransform == false)
                        {
                            transform.hasChanged = false;
                        }
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
