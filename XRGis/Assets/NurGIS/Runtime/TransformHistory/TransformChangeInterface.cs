using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace NurGIS.Runtime.TransformHistory
{
 
    
    public class TransformChangeInterface : MonoBehaviour
    {
        #region GUI
        [CustomEditor(typeof(TransformChangeInterface))]
        public class CustomTransformUI : Editor
        {
            private SerializedProperty _transformGoList;
            private bool _showGoList = true;
            private TransformChangeInterface _transformChangeInterface;
            
            private void OnEnable()
            {
                _transformChangeInterface = (TransformChangeInterface)target;
                _transformGoList = serializedObject.FindProperty("transformGoList");
            }
            
            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                
                _showGoList = EditorGUILayout.Foldout(_showGoList, "Transforms");
                if (_showGoList)
                {
                    for (int i = 0; i < _transformGoList.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(_transformGoList.GetArrayElementAtIndex(i));
                    }
                }
                serializedObject.ApplyModifiedProperties();
                
                // Slider in Inspector
                _transformChangeInterface.sliderPosition = EditorGUILayout.IntSlider(_transformChangeInterface.sliderPosition,0, _transformChangeInterface.transformGoList.Count);
            }
        }
        #endregion


        //private bool _sliderPositionChanged;
        public int sliderPosition = 1;
        public List<GameObject> transformGoList;
        public bool globalUndoCallParameter;
        public bool useTransformTracker = true;
        

        private void IdentifyLastChangedGo()
        {
            GameObject go = transformGoList[^1];
            TransformChangeHelper helper = go.GetComponent<TransformChangeHelper>();
            helper.undoSingleTransform = true;
            globalUndoCallParameter = true;
            transformGoList.RemoveAt(transformGoList.Count - 1);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && transformGoList.Count > 0)
            {
                IdentifyLastChangedGo();
            }
            
            /*
            if (sliderPosition != transformGoList.Count) // Use Slider to change transform
            {
                _sliderPositionChanged = true;
                
            }
            else
            {
                _sliderPositionChanged = false;
            }
            */
        }
    }
}
