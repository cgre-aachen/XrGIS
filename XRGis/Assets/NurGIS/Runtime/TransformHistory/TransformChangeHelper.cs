using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeHelper : MonoBehaviour
    {
        #region GUI
        [CustomEditor(typeof(TransformChangeHelper))]
        public class CustomTransformUI : Editor
        {
            private SerializedProperty _absTranslationList;
            private SerializedProperty _absRotationList;
            private SerializedProperty _absScaleList;
            
            private SerializedProperty _relTranslationList;
            private SerializedProperty _relRotationList;
            private SerializedProperty _relScaleList;

            private bool _showTranslation = true;
            private bool _showRotation;
            private bool _showScale;
            private TransformChangeHelper _transformChangeHelper;
            
            private void OnEnable()
            {
                _transformChangeHelper = (TransformChangeHelper)target;
                
                _absTranslationList = serializedObject.FindProperty("positionList");
                _absRotationList = serializedObject.FindProperty("rotationList");
                _absScaleList = serializedObject.FindProperty("scaleList");
                
                _relTranslationList = serializedObject.FindProperty("relativePositionList");
                _relRotationList = serializedObject.FindProperty("relativeRotationList");
                _relScaleList = serializedObject.FindProperty("relativeScaleList");
            }
            
            public override void OnInspectorGUI()
            {
                #region rotationscalepositionGUI
                serializedObject.Update();
                
                _showTranslation = EditorGUILayout.Foldout(_showTranslation, "Translation");
                if (_showTranslation)
                {
                    EditorGUILayout.LabelField("Absolute Transforms", EditorStyles.boldLabel);
                    for (int i = 0; i < _absTranslationList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_absTranslationList.GetArrayElementAtIndex(i));
                    }
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Relative Transforms", EditorStyles.boldLabel);
                    for (int i = 0; i < _relTranslationList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_relTranslationList.GetArrayElementAtIndex(i));
                    }
                }
                
                _showRotation = EditorGUILayout.Foldout(_showRotation, "Rotation");
                
                if (_showRotation)
                {
                    EditorGUILayout.LabelField("Absolute Transforms", EditorStyles.boldLabel);
                    for (int i = 0; i < _absRotationList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_absRotationList.GetArrayElementAtIndex(i));
                    }
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Relative Transforms", EditorStyles.boldLabel);
                    for (int i = 0; i < _relRotationList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_relRotationList.GetArrayElementAtIndex(i));
                    }
                }
                
                _showScale = EditorGUILayout.Foldout(_showScale, "Scale");
                
                if (_showScale)
                {
                    EditorGUILayout.LabelField("Absolute Transforms", EditorStyles.boldLabel);
                    for (int i = 0; i < _absScaleList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_absScaleList.GetArrayElementAtIndex(i));
                    }
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Relative Transforms", EditorStyles.boldLabel);
                    for (int i = 0; i < _relScaleList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_relScaleList.GetArrayElementAtIndex(i));
                    }
                }
                
                serializedObject.ApplyModifiedProperties();
                #endregion
                
                // Slider in Inspector
                _transformChangeHelper.sliderPosition = EditorGUILayout.IntSlider(_transformChangeHelper.sliderPosition,1, _transformChangeHelper.positionList.Count);
            }
        }
        #endregion
        
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

        public bool undoSingleTransform;
        public int sliderPosition;
        
        public List<Vector3> positionList;
        public List<Vector3> relativePositionList;
        private Vector3 _relativePositionChange;
        
        public List<Quaternion> rotationList;
        public List<Quaternion> relativeRotationList;
        private Quaternion _relativeRotationChange;
        
        public List<Vector3> scaleList;
        public List<Vector3> relativeScaleList;
        private Vector3 _relativeScaleChange;
        #endregion
        
        #region Methods
        private void SaveTransform(GameObject go)
        {
            _transformChangeInterface.transformGoList.Add(go);
            _transformChangeInterface.sliderPosition = _transformChangeInterface.transformGoList.Count;
            var localPosition = go.transform.localPosition;
            var localScale = go.transform.localScale;
            var localRotation = go.transform.localRotation;
            
            positionList.Add(localPosition);
            _relativePositionChange = localPosition - positionList[^2];
            relativePositionList.Add(_relativePositionChange);

            
            scaleList.Add(localScale);
            _relativeScaleChange = localScale - scaleList[^2];
            relativeScaleList.Add(_relativeScaleChange);
            
            rotationList.Add(localRotation);
            _relativeRotationChange = localRotation * Quaternion.Inverse(rotationList[^2]);
            relativeRotationList.Add(_relativeRotationChange);
        }
        
        private void DetectContinuousAbsTransform()
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
        
        
        private void SetAbsTransform(int position = 1)
        {
            var go = gameObject;
            go.transform.localPosition = positionList[^position];
            go.transform.localScale = scaleList[^position];
            go.transform.localRotation = rotationList[^position];
        }
        
        
        private void RemoveRelTransformInList(int count = 1, int position = 1)
        {
            relativePositionList.RemoveRange(relativePositionList.Count - position, count);
            relativeScaleList.RemoveRange(relativeScaleList.Count - position, count);
            relativeRotationList.RemoveRange(relativeRotationList.Count - position, count);
            
            positionList.RemoveRange(positionList.Count - position, count);
            scaleList.RemoveRange(scaleList.Count - position, count);
            rotationList.RemoveRange(rotationList.Count - position, count);
        }
        
        private void RemoveEmptyRelTransform()
        {
            if (relativePositionList.Count > 1)
            {
                if (relativePositionList[^1].x == 0 && relativePositionList[^1].y == 0 && relativePositionList[^1].z == 0 &&
                    relativeScaleList[^1].x == 0 && relativeScaleList[^1].y == 0 && relativeScaleList[^1].z == 0 &&
                    relativeRotationList[^1].x == 0 && relativeRotationList[^1].y == 0 && relativeRotationList[^1].z == 0)
                {
                    if (_continuousTransform == false)
                    {
                        RemoveRelTransformInList();
                        _transformChangeInterface.transformGoList.RemoveAt(_transformChangeInterface.transformGoList.Count - 1);
                        _transformChangeInterface.sliderPosition = _transformChangeInterface.transformGoList.Count;
                    }
                    else if (_continuousTransform)
                    {
                        positionList.RemoveRange(_positionOfContinuousTransformStart, positionList.Count - _positionOfContinuousTransformStart -1);
                        scaleList.RemoveRange(_positionOfContinuousTransformStart, scaleList.Count - _positionOfContinuousTransformStart -1);
                        rotationList.RemoveRange(_positionOfContinuousTransformStart, rotationList.Count - _positionOfContinuousTransformStart -1);
                    
                        relativePositionList.RemoveRange(_positionOfContinuousTransformStart, relativePositionList.Count - _positionOfContinuousTransformStart -1);
                        relativeScaleList.RemoveRange(_positionOfContinuousTransformStart, relativeScaleList.Count - _positionOfContinuousTransformStart -1);
                        relativeRotationList.RemoveRange(_positionOfContinuousTransformStart, relativeRotationList.Count - _positionOfContinuousTransformStart -1);
                    
                        _transformChangeInterface.transformGoList.RemoveRange(_positionOfContinuousTransformGoListStart, _transformChangeInterface.transformGoList.Count - _positionOfContinuousTransformGoListStart -1);
                        _transformChangeInterface.sliderPosition = _transformChangeInterface.transformGoList.Count;
                
                        _continuousTransform = false;
                        transform.hasChanged = false;
                        _continuousTransformEnded = true;
                    }
                }
            }
        }
        
        private void DetectContinuousRelTransform()
        {
            if (relativePositionList.Count > 1 && _continuousTransform == false && _continuousTransformEnded == false)
            {
                if (Math.Abs(relativePositionList[^1].x) < _translationThreshold && Math.Abs(relativePositionList[^1].x) > 0 ||
                    Math.Abs(relativePositionList[^1].y) < _translationThreshold && Math.Abs(relativePositionList[^1].y) > 0 ||
                    Math.Abs(relativePositionList[^1].z) < _translationThreshold && Math.Abs(relativePositionList[^1].z) > 0)
                {
                    _positionOfContinuousTransformStart = relativePositionList.Count - 1;
                    _positionOfContinuousTransformGoListStart = _transformChangeInterface.transformGoList.Count - 1;
                    _continuousTransform = true;
                }
                
                else if (Math.Abs(relativeScaleList[^1].x) < _scaleThreshold && Math.Abs(relativeScaleList[^1].x) > 0 ||
                         Math.Abs(relativeScaleList[^1].y) < _scaleThreshold && Math.Abs(relativeScaleList[^1].y) > 0 ||
                         Math.Abs(relativeScaleList[^1].z) < _scaleThreshold && Math.Abs(relativeScaleList[^1].z) > 0)
                {
                    _positionOfContinuousTransformStart = relativeScaleList.Count - 1;
                    _positionOfContinuousTransformGoListStart = _transformChangeInterface.transformGoList.Count - 1;
                    _continuousTransform = true;
                }
                
                else if (Math.Abs(relativeRotationList[^1].x) < _rotationThreshold && Math.Abs(relativeRotationList[^1].x) > 0 ||
                         Math.Abs(relativeRotationList[^1].y) < _rotationThreshold && Math.Abs(relativeRotationList[^1].y) > 0 ||
                         Math.Abs(relativeRotationList[^1].z) < _rotationThreshold && Math.Abs(relativeRotationList[^1].z) > 0 )
                {
                    _positionOfContinuousTransformStart = relativeRotationList.Count - 1;
                    _positionOfContinuousTransformGoListStart = _transformChangeInterface.transformGoList.Count - 1;
                    _continuousTransform = true;
                }
            }
            
            else
            {
                _continuousTransformEnded = false;
            }
        }
        
        private void SetRelTransform()
        {
            var go = gameObject;
            Vector3 positionVector = Vector3.zero;
            foreach (Vector3 relPosition in relativePositionList)
            {
                positionVector += relPosition;
            }
            
            Vector3 scaleVector = Vector3.zero;
            foreach (Vector3 relScale in relativeScaleList)
            {
                scaleVector += relScale;
            }
            
            Quaternion rotationQuaternion = Quaternion.identity;
            foreach (Quaternion relRotation in relativeRotationList)
            {
                rotationQuaternion *= relRotation;
            }
            
            go.transform.localPosition = positionVector;
            go.transform.localScale = scaleVector;
            go.transform.localRotation = rotationQuaternion;
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
                    var go = gameObject;
                    
                    var localScale = go.transform.localScale;
                    var localPosition = go.transform.localPosition;
                    var localRotation = go.transform.localRotation;
                    
                    positionList.Add(localPosition);
                    scaleList.Add(localScale);
                    rotationList.Add(localRotation);
                    
                    relativePositionList.Add(localPosition);
                    relativeScaleList.Add(localScale);
                    relativeRotationList.Add(localRotation);
                    sliderPosition = positionList.Count;
                }
                else
                {
                    if (transform.hasChanged && 
                        _transformChangeInterface.globalUndoCallParameter == false &&
                        _transformChangeInterface.useTransformTracker &&
                        _sliderPositionChanged == false) // Track transform change state
                    {
                        SaveTransform(gameObject);
                        RemoveEmptyRelTransform(); 
                        DetectContinuousRelTransform(); 
                        SetRelTransform();

                        sliderPosition = positionList.Count;
                        
                        if (_continuousTransform == false)
                        {
                            transform.hasChanged = false;
                        }
                    }

                    if (undoSingleTransform) // Undo transforms
                    {
                        RemoveRelTransformInList(1,1);
                        SetRelTransform();
                        undoSingleTransform = false;
                        _transformChangeInterface.globalUndoCallParameter = false;
                        transform.hasChanged = false;
                        sliderPosition = positionList.Count;
                    }
                    
                    if (sliderPosition != positionList.Count) // Use Slider to change transform
                    {
                        _sliderPositionChanged = true;
                        var go = gameObject;
                        go.transform.localPosition = positionList[sliderPosition-1];
                        go.transform.localScale = scaleList[sliderPosition-1];
                        go.transform.localRotation = rotationList[sliderPosition-1];
                    }
                    else if (sliderPosition == positionList.Count && _sliderPositionChanged)
                    {
                        SetRelTransform();
                        _sliderPositionChanged = false;
                        transform.hasChanged = false;
                    }
                }
            }
        }
    }
}
