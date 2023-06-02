#if false
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeHelper : MonoBehaviour
    {
        #region GUI
        [CustomEditor(typeof(TransformChangeHelper))]
        public class CustomTransformUI : Editor
        {
            #region GUIProperties
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
            
            private Vector3 _translationVector;
            private Vector3 _scaleVector;
            private Vector3 _eulerAngles;
            #endregion
            
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
                // Transform Lists
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
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                // Slider in Inspector
                EditorGUILayout.LabelField("Transform Slider", EditorStyles.boldLabel);
                _transformChangeHelper.sliderPosition = EditorGUILayout.IntSlider(_transformChangeHelper.sliderPosition,1, _transformChangeHelper.relativePositionList.Count);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                // Input Fields and Buttons
                EditorGUILayout.LabelField("Transform Input", EditorStyles.boldLabel);
                // Translation
                _translationVector = EditorGUILayout.Vector3Field("Position:", _translationVector);
                bool applyTranslationButton = GUILayout.Button("Apply Translation");
                if (applyTranslationButton)
                {
                    _transformChangeHelper.transformType = TransformType.Translation;
                    _transformChangeHelper.relativePositionList.Add(_translationVector);

                    _translationVector = Vector3.zero;
                }
                // Rotation
                _eulerAngles = EditorGUILayout.Vector3Field("Rotation:", _eulerAngles);
                bool applyRotationButton = GUILayout.Button("Apply Rotation");
                if (applyRotationButton)
                {
                    _transformChangeHelper.transformType = TransformType.Rotation;
                    Quaternion quaternion = Quaternion.Euler(_eulerAngles);
                    _transformChangeHelper.relativeRotationList.Add(quaternion);

                    _eulerAngles = Vector3.zero;
                }
                // Scale 
                _scaleVector = EditorGUILayout.Vector3Field("Scale:", _scaleVector);
                bool applyScaleButton = GUILayout.Button("Apply Scaling");
                if (applyScaleButton)
                {
                    _transformChangeHelper.transformType = TransformType.Scale;
                    _transformChangeHelper.relativeScaleList.Add(_scaleVector);

                    _scaleVector = Vector3.zero;
                }
                
                // Reset Button
                bool resetTransformButton = GUILayout.Button("Reset Transform");
                if (resetTransformButton)
                {
                    _transformChangeHelper.ResetObject();
                    _transformChangeHelper.transformType = TransformType.NoTransform;
                }
            }
        }
        #endregion
        
        #region Structs
        enum TransformTypes
        {
            Absolute,
            Relative
        }
        struct MyTransform
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public TransformTypes transformType;
            public bool IsActive;
            public void AbsTransform(Transform transform)
            {
                position = transform.localPosition;
                rotation = transform.localRotation;
                scale = transform.localScale;
                transformType = TransformTypes.Absolute;
            }
        
            public void RelTransform(MyTransform parentTransform, Transform currentTransform)
            {
                position = currentTransform.localPosition - parentTransform.position;
                rotation = Quaternion.Inverse(parentTransform.rotation) * currentTransform.localRotation;
                scale = currentTransform.localScale - parentTransform.scale;
                transformType = TransformTypes.Relative;
            }
        }
        #endregion

        
        #region Properties
        private Main _mainComponent;
        private TransformChangeInterface _transformChangeInterface;
        private bool _initialStateLoaded;
        
        private int _positionOfContinuousTransformStart;
        private int _positionOfContinuousTransformGoListStart;
        private bool _continuousTransform;
        private bool _continuousTransformEnded;
        private bool _sliderPositionChanged;

        public enum TransformType
        {
            Translation,
            Rotation,
            Scale,
            NoTransform
        }
        
        public TransformType transformType;
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
        {   // Baustelle
            _transformChangeInterface.transformGoList.Add(go);
            _transformChangeInterface.sliderPosition = _transformChangeInterface.transformGoList.Count;
            sliderPosition = relativePositionList.Count;
            
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
        
        private void SetAbsTransform(int position = 1)
        {
            var go = gameObject;
            go.transform.localPosition = positionList[^position];
            go.transform.localScale = scaleList[^position];
            go.transform.localRotation = rotationList[^position];
        }
        
        
        private void RemoveRelTransformInList(int count = 1, int position = 1, bool reset = false)
        {
            if (reset)
            {
                relativePositionList.RemoveRange(1, relativePositionList.Count - 1);
                relativeScaleList.RemoveRange(1, relativeScaleList.Count - 1);
                relativeRotationList.RemoveRange(1, relativeRotationList.Count - 1);
            }
            else
            {
                relativePositionList.RemoveRange(relativePositionList.Count - position, count);
                relativeScaleList.RemoveRange(relativeScaleList.Count - position, count);
                relativeRotationList.RemoveRange(relativeRotationList.Count - position, count);
            }
        }
        
        private void RemoveAbsTransformInList(int count = 1, int position = 1, bool reset = false)
        {
            if (reset)
            {
                positionList.RemoveRange(1, positionList.Count -1);
                scaleList.RemoveRange(1, scaleList.Count - 1);
                rotationList.RemoveRange(1, rotationList.Count - 1);
            }
            else
            {
                positionList.RemoveRange(positionList.Count - position, count);
                scaleList.RemoveRange(scaleList.Count - position, count);
                rotationList.RemoveRange(rotationList.Count - position, count);
            }


        }

        private void ResetObject()
        {
            RemoveRelTransformInList(reset:true);
            RemoveAbsTransformInList(reset:true);
            SetAbsTransform();
        }
        
        private void SetRelPosition()
        {
            Vector3 positionVector = Vector3.zero;
            foreach (Vector3 relPosition in relativePositionList)
            {
                positionVector += relPosition;
            }
            
            var position = positionVector;
            gameObject.transform.localPosition = position;
        }

        private void SetRelScale()
        {
            Vector3 scaleVector = Vector3.zero;
            foreach (Vector3 relScale in relativeScaleList)
            {
                scaleVector += relScale;
            }
            
            var scale = scaleVector;
            gameObject.transform.localScale = scale;
        }

        private void SetRelRotation()
        {
            Quaternion rotationQuaternion = Quaternion.identity;
            foreach (Quaternion relRotation in relativeRotationList)
            {
                rotationQuaternion *= relRotation;
            }
            
            var rotation = rotationQuaternion;
            gameObject.transform.localRotation = rotation;
        }

        private void SetTransform()
        {
            switch (transformType)
            {
                case TransformType.Translation:
                    SetRelPosition();
                    transformType = TransformType.NoTransform;
                    transform.hasChanged = false;
                    break;
                        
                case TransformType.Rotation:
                    SetRelRotation();
                    transformType = TransformType.NoTransform;
                    transform.hasChanged = false;
                    break;
                        
                case TransformType.Scale:
                    SetRelScale();
                    transformType = TransformType.NoTransform;
                    transform.hasChanged = false;
                    break;
                        
                case TransformType.NoTransform:
                    transform.hasChanged = false;
                    break;
                        
                default:
                    transform.hasChanged = false;
                    break;
            }
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
                if (_initialStateLoaded == false) // Add the initial state to the lists
                {
                    _initialStateLoaded = true;
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
                    
                    transformType = TransformType.NoTransform;
                }
                else
                {
                    SetTransform();
                    
                    if (transform.hasChanged && 
                        _transformChangeInterface.globalUndoCallParameter == false &&
                        _transformChangeInterface.useTransformTracker &&
                        _sliderPositionChanged == false) // Track transform change state
                    {
                        SaveTransform(gameObject);
                        
                        sliderPosition = positionList.Count;
                        
                        if (_continuousTransform == false)
                        {
                            transform.hasChanged = false;
                        }
                    }

                    if (undoSingleTransform) // Undo transforms
                    {
                        RemoveRelTransformInList();
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
                    else if (sliderPosition == positionList.Count && _sliderPositionChanged) // Set state after slider change
                    {
                        _sliderPositionChanged = false;
                        transform.hasChanged = false;
                    }
                }
            }
        }
    }
}
#endif



