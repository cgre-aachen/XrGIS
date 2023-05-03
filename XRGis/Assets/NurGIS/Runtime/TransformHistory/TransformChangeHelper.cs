using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    
    [CustomEditor(typeof(TransformChangeHelper))]
    public class CustomTransformUI : Editor
    {
        private SerializedProperty _translationList;
        private SerializedProperty _rotationList;
        private SerializedProperty _scaleList;

        private bool _showTranslation = true;
        private bool _showRotation = true;
        private bool _showScale = true;
        private TransformChangeHelper _transformChangeHelper;
        
        private void OnEnable()
        {
            _transformChangeHelper = (TransformChangeHelper)target;
            _translationList = serializedObject.FindProperty("positionList");
            _rotationList = serializedObject.FindProperty("rotationList");
            _scaleList = serializedObject.FindProperty("scaleList");
        }
        
        public override void OnInspectorGUI()
        {
            #region rotationscalepositionGUI
            serializedObject.Update();
            
            _showTranslation = EditorGUILayout.Foldout(_showTranslation, "Translation");
            if (_showTranslation)
            {
                for (int i = 0; i < _translationList.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_translationList.GetArrayElementAtIndex(i));
                }
            }
            
            _showRotation = EditorGUILayout.Foldout(_showRotation, "Rotation");
            
            if (_showRotation)
            {
                for (int i = 0; i < _rotationList.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_rotationList.GetArrayElementAtIndex(i));
                }
            }
            
            _showScale = EditorGUILayout.Foldout(_showScale, "Scale");
            
            if (_showScale)
            {
                for (int i = 0; i < _scaleList.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_scaleList.GetArrayElementAtIndex(i));
                }
            }
            
            serializedObject.ApplyModifiedProperties();
            #endregion
            
            // Slider in Inspector
            _transformChangeHelper.sliderPosition = EditorGUILayout.IntSlider(_transformChangeHelper.sliderPosition,0, _transformChangeHelper.positionList.Count);
        }
    }
    
    
    public class TransformChangeHelper : MonoBehaviour
    {
        
        #region Properties
        private Main _mainComponent;
        private TransformChangeInterface _transformChangeInterface;
        private bool _initialState;
        
        private readonly double _translationThreshold = 1;
        private readonly double _rotationThreshold = 1;
        private readonly double _scaleThreshold = 0.1;

        private int _positionOfContinuousTransformStart;
        private int _positionOfContinuousTransformGoListStart;
        private bool _continuousTransform;
        private bool _continuousTransformEnded;
        private bool _sliderPositionChanged;

        public bool privateUndoTransform;
        public List<Vector3> positionList;
        public List<Quaternion> rotationList;
        public List<Vector3> scaleList;
        public int sliderPosition = 1;
        #endregion
        
        #region Methods
        private void SaveTransform()
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
                if (positionList[^1] == positionList[^2] && scaleList[^1] == scaleList[^2] && rotationList[^1] == rotationList[^2] && _continuousTransform == false)
                {
                    RemoveTransformInList();
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
                
                else if (Math.Abs(scaleList[^1].x - scaleList[^2].x) < _scaleThreshold && Math.Abs(scaleList[^1].x - scaleList[^2].x) > 0 ||
                         Math.Abs(scaleList[^1].y - scaleList[^2].y) < _scaleThreshold && Math.Abs(scaleList[^1].y - scaleList[^2].y) > 0 ||
                         Math.Abs(scaleList[^1].z - scaleList[^2].z) < _scaleThreshold && Math.Abs(scaleList[^1].z - scaleList[^2].z) > 0)
                {
                    _positionOfContinuousTransformStart = scaleList.Count - 1;
                    _positionOfContinuousTransformGoListStart = _transformChangeInterface.transformGoList.Count - 1;
                    _continuousTransform = true;
                }
                
                else if (Math.Abs(rotationList[^1].x - rotationList[^2].x) < _rotationThreshold && Math.Abs(rotationList[^1].x - rotationList[^2].x) > 0 ||
                         Math.Abs(rotationList[^1].y - rotationList[^2].y) < _rotationThreshold && Math.Abs(rotationList[^1].y - rotationList[^2].y) > 0 ||
                         Math.Abs(rotationList[^1].z - rotationList[^2].z) < _rotationThreshold && Math.Abs(rotationList[^1].z - rotationList[^2].z) > 0)
                {
                    _positionOfContinuousTransformStart = rotationList.Count - 1;
                    _positionOfContinuousTransformGoListStart = _transformChangeInterface.transformGoList.Count - 1;
                    _continuousTransform = true;
                }
            }
            
            else
            {
                _continuousTransformEnded = false;
            }
        }
        
        private void RemoveTransformInList(int count = 1)
        {
            positionList.RemoveAt(positionList.Count - count);
            scaleList.RemoveAt(scaleList.Count - count);
            rotationList.RemoveAt(rotationList.Count - count);
        }

        private void UndoTransform()
        {
            RemoveTransformInList();
            var go = gameObject;
            go.transform.localPosition = positionList[^1];
            go.transform.localScale = scaleList[^1];
            go.transform.localRotation = rotationList[^1];
        }
        #endregion

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
                    sliderPosition = positionList.Count;
                }

                else
                {
                    if (transform.hasChanged && 
                        _transformChangeInterface.globalUndoCallParameter == false &&
                        _transformChangeInterface.useTransformTracker &&
                        _sliderPositionChanged == false) // Track transform change state
                    {
                        SaveTransform();
                        RemoveEmptyTransform();
                        DetectContinuousTransform();
                        sliderPosition = positionList.Count;
                        
                        if (_continuousTransform == false)
                        {
                            transform.hasChanged = false;
                        }
                    }

                    if (privateUndoTransform) // Undo transforms
                    {
                        UndoTransform();
                        privateUndoTransform = false;
                        _transformChangeInterface.globalUndoCallParameter = false;
                        transform.hasChanged = false;
                        sliderPosition = positionList.Count;
                    }
                    
                    if (sliderPosition != positionList.Count) // Use Slider to change transform
                    {
                        _sliderPositionChanged = true;
                        var go = gameObject;
                        go.transform.localPosition = positionList[sliderPosition];
                        go.transform.localScale = scaleList[sliderPosition];
                        go.transform.localRotation = rotationList[sliderPosition];
                    }
                    else
                    {
                        _sliderPositionChanged = false;
                    }
                }
            }
        }
    }
}
