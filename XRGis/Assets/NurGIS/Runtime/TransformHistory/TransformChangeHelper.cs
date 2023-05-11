using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeHelper : MonoBehaviour
    {
        #region GUI
        #if false
        
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

            private bool _showTranslation;
            private bool _showRotation;
            private bool _showScale;
            private bool _detailedInformation;
            private bool _showTransform = true;
            
            private TransformChangeHelper _transformChangeHelper;
            private Vector3 _translationVector;
            private Vector3 _scaleVector;
            private Vector3 _eulerAngles;
            private MyTransform _myTransform;
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
                
                _showTransform = EditorGUILayout.Foldout(_showTransform, "Transforms");
                if (_showTransform)
                {
                    foreach (var t in _transformChangeHelper.transformTypeList)
                    {
                        EditorGUILayout.LabelField(t);
                    }
                }
                
                _detailedInformation = EditorGUILayout.Foldout(_detailedInformation, "Detailed Information");
                if (_detailedInformation)
                { 
                    _showTranslation = EditorGUILayout.Foldout(_showTranslation, "Translation");
                    if (_showTranslation)
                    {
                        EditorGUILayout.LabelField("Absolute Transforms", EditorStyles.boldLabel);
                        foreach (Vector3 i in _transformChangeHelper.positionList)
                        {
                            EditorGUILayout.LabelField(i.ToString());
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
                }
                
                serializedObject.ApplyModifiedProperties();
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                #region Slider
                EditorGUILayout.LabelField("Transform Slider", EditorStyles.boldLabel);
                _transformChangeHelper.sliderPosition = EditorGUILayout.IntSlider(_transformChangeHelper.sliderPosition,1, _transformChangeHelper.relativePositionList.Count);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                #endregion
                
                #region InputFields and Buttons
                EditorGUILayout.LabelField("Transform Input", EditorStyles.boldLabel);
                // Translation
                _translationVector = EditorGUILayout.Vector3Field("Position:", _translationVector);
                bool applyTranslationButton = GUILayout.Button("Apply Translation");
                if (applyTranslationButton)
                {
                    _myTransform = new MyTransform
                    {
                        transformType = TransformTypes.Relative,
                        transformSpecifier = TransformSpecifier.Translation,
                        IsActive = true,
                        position = _translationVector,
                        rotation = Quaternion.identity,
                        scale = Vector3.zero
                    };

                    _transformChangeHelper.transformList.Add(_myTransform);
                    _transformChangeHelper.transformListChanged = true;
                    
                    _transformChangeHelper.relativePositionList.Add(_translationVector);
                    _translationVector = Vector3.zero;
                }
                
                // Rotation
                _eulerAngles = EditorGUILayout.Vector3Field("Rotation:", _eulerAngles);
                bool applyRotationButton = GUILayout.Button("Apply Rotation");
                if (applyRotationButton)
                {
                    _myTransform = new MyTransform
                    {
                        transformType = TransformTypes.Relative,
                        transformSpecifier = TransformSpecifier.Rotation,
                        IsActive = true,
                        position = Vector3.zero,
                        rotation = Quaternion.Euler(_eulerAngles),
                        scale = Vector3.zero
                    };
                    
                    _transformChangeHelper.transformList.Add(_myTransform);
                    _transformChangeHelper.transformListChanged = true;
                    
                    Quaternion quaternion = Quaternion.Euler(_eulerAngles);
                    _transformChangeHelper.relativeRotationList.Add(quaternion);
                    _eulerAngles = Vector3.zero;
                }
                // Scale 
                _scaleVector = EditorGUILayout.Vector3Field("Scale:", _scaleVector);
                bool applyScaleButton = GUILayout.Button("Apply Scaling");
                if (applyScaleButton)
                {
                    _myTransform = new MyTransform
                    {
                        transformType = TransformTypes.Relative,
                        transformSpecifier = TransformSpecifier.Scale,
                        IsActive = true,
                        position = Vector3.zero,
                        rotation = Quaternion.identity,
                        scale = _scaleVector
                    };
                    
                    _transformChangeHelper.transformList.Add(_myTransform);
                    _transformChangeHelper.transformListChanged = true;
                    

                    _transformChangeHelper.relativeScaleList.Add(_scaleVector);
                    _scaleVector = Vector3.zero;
                }
                
                // Reset Button
                bool resetTransformButton = GUILayout.Button("Reset Transform");
                if (resetTransformButton)
                {
                }
                #endregion
                
            }
        }
        
        #endif
        #endregion

        #region Properties
        public enum TransformTypes
        {
            Absolute,
            Relative
        }

        public enum TransformSpecifier
        {
            Translation,
            Rotation,
            Scale,
            AbsoluteTransform,
            NoTransform
        }

        public struct MyTransform
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public TransformTypes transformType;
            public TransformSpecifier transformSpecifier;
            public bool IsActive;
        }
        
        public List<MyTransform> transformList;
        public List<string> transformNameList;
        
        public Vector3 translationInput;
        public Vector3 rotationInput;
        public Vector3 scaleInput = Vector3.one;

        public bool noEntry;
        
        private bool _initialStateLoaded;
        #endregion
        
        #region Methods
        public void SaveRelativeTransformFromGui()
        {
            MyTransform myTransform = new MyTransform();
            if (translationInput != Vector3.zero)
            {
                myTransform.transformType = TransformTypes.Relative;
                myTransform.transformSpecifier = TransformSpecifier.Translation;
                myTransform.IsActive = true;
                myTransform.position = translationInput;
                myTransform.rotation = Quaternion.identity;
                myTransform.scale = Vector3.one;
                transformList.Add(myTransform);
                translationInput = Vector3.zero;
                noEntry = false;
            }
            else if (rotationInput != Vector3.zero)
            {
                myTransform.transformType = TransformTypes.Relative;
                myTransform.transformSpecifier = TransformSpecifier.Rotation;
                myTransform.IsActive = true;
                myTransform.position = Vector3.zero;
                myTransform.rotation = Quaternion.Euler(rotationInput);
                myTransform.scale = Vector3.one;
                transformList.Add(myTransform);
                rotationInput = Vector3.zero;
                noEntry = false;
            }
            else if (scaleInput != Vector3.one)
            {
                myTransform.transformType = TransformTypes.Relative;
                myTransform.transformSpecifier = TransformSpecifier.Scale;
                myTransform.IsActive = true;
                myTransform.position = Vector3.zero;
                myTransform.rotation = Quaternion.identity;
                myTransform.scale = scaleInput;
                transformList.Add(myTransform);
                scaleInput = Vector3.one;
                noEntry = false;
            }
            else
            {
                noEntry = true;
                Debug.Log("Enter a value");
            }
        }

        public void SaveAbsoluteTransform(List<Vector3> list)
        {
            MyTransform absoluteTransform = new MyTransform()
            {
                position = list[0],
                rotation = Quaternion.Euler(list[1]),
                scale = list[2],
                transformType = TransformTypes.Absolute,
                transformSpecifier = TransformSpecifier.AbsoluteTransform,
                IsActive = true
            };
            
            this.transformList.Add(absoluteTransform);
        }

        public List<Vector3> CalculateTransform(int savePointIndex, int lastIndex)
        {
            Quaternion rotationQuaternion = Quaternion.identity;
            Vector3 positionVector = Vector3.zero;
            Vector3 scaleVector = Vector3.one;
            
            List<Vector3> list = new List<Vector3>();

            for (int i = savePointIndex; i <= lastIndex; i++)
            {
                MyTransform myTransform = this.transformList[i];
                if (myTransform.IsActive == false)
                {
                    rotationQuaternion *= Quaternion.identity;
                    positionVector += Vector3.zero;
                    scaleVector = Vector3.Scale(scaleVector, Vector3.one);
                }
                else
                {
                    rotationQuaternion *= myTransform.rotation;
                    positionVector += myTransform.position;
                    scaleVector = Vector3.Scale(scaleVector, myTransform.scale);
                }
            }
            
            list.Add(positionVector);
            list.Add(rotationQuaternion.eulerAngles);
            list.Add(scaleVector);

            return list;
        }

        public void ApplyTransformToGo(List<Vector3> list)
        {
            GameObject go = gameObject;
            go.transform.localPosition = list[0];
            go.transform.localRotation = Quaternion.Euler(list[1]);
            go.transform.localScale = list[2];
        }

        public string SetTransformName()
        {
            string listValue = "";
            MyTransform myTransform = transformList[^1];

            switch (myTransform.transformSpecifier)
            {
                case TransformSpecifier.Translation:
                    listValue += "Position changed";
                    break;    
                case TransformSpecifier.Rotation:
                    listValue += "Rotation changed";
                    break;
                case TransformSpecifier.Scale:
                    listValue += "Scale changed";
                    break;
                case TransformSpecifier.AbsoluteTransform:
                    listValue += "Position Saved";
                    break;
                default:
                    listValue += "Unknown Transform Type";
                    break;
            }
            transformNameList.Add(listValue);
            return listValue;
        }
        
        public int FindLastActiveTransformIndex(int startIndex)
        {
            int lastIndex = 0;
            for (int i = startIndex; i >= 0; i--)
            {
                if (transformList[i].transformType == TransformTypes.Absolute)
                {
                    lastIndex = i;
                    break;
                }
            }
            return lastIndex;
        }

        public void ResetTransforms()
        {
            transformList.RemoveRange(1, transformList.Count - 1);
            transformNameList.RemoveRange(1, transformNameList.Count - 1);
            CalculateTransform(0,0);
        }
        #endregion
        
        private void Start()
        {
            transformList = new List<MyTransform>();
            transformNameList = new List<string>();
        }
        
        private void Update()
        {
            if (Main.isInitialized) // Look if the initial state was set in main()
            {
                if (_initialStateLoaded == false) // Add the initial state to the lists
                {
                    var go = gameObject;
                    var localScale = go.transform.localScale;
                    var localPosition = go.transform.localPosition;
                    var localRotation = go.transform.localRotation;

                    var initialTransform = new MyTransform
                    {
                        position = localPosition,
                        rotation = localRotation,
                        scale = localScale,
                        transformType = TransformTypes.Absolute,
                        transformSpecifier = TransformSpecifier.NoTransform,
                        IsActive = true
                    };
                    
                    transformList.Add(initialTransform);
                    transformNameList.Add("Start Position");
                    _initialStateLoaded = true;
                }
            }
        }
    }
}
