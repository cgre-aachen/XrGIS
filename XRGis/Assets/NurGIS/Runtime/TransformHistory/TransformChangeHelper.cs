using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChangeHelper : MonoBehaviour
    {
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
            NoTransform,
            MultipleTransforms
        }
        
        public class MyTransform
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public TransformTypes transformType;
            public TransformSpecifier transformSpecifier;
            public bool IsActive;
        }
        
        public List<MyTransform> transformList = new();
        public List<string> transformNameList = new();
        
        public Vector3 translationInput = Vector3.zero;
        public Vector3 rotationInput = Vector3.zero;
        public Vector3 scaleInput = Vector3.one;

        public bool noEntry;
        public bool applyToVertices;
        
        private bool _initialStateLoaded;
        #endregion
        
        #region Methods
        public void SaveRelativeTransformFromGui()
        {
            if (translationInput == Vector3.zero &&
                rotationInput == Vector3.zero &&
                scaleInput == Vector3.one)
            {
                noEntry = true;
                Debug.Log("Enter a value");
                return;
            }
            
            MyTransform myTransform = new MyTransform
            {
                transformType = TransformTypes.Relative,
                IsActive = true,
                position = translationInput,
                rotation = Quaternion.Euler(rotationInput),
                scale = scaleInput
            };
            
            if (translationInput != Vector3.zero &&
                rotationInput == Vector3.zero &&
                scaleInput == Vector3.one)
            {
                myTransform.transformSpecifier = TransformSpecifier.Translation;
            }
            else if (rotationInput != Vector3.zero &&
                     translationInput == Vector3.zero &&
                     scaleInput == Vector3.one)
            {
                myTransform.transformSpecifier = TransformSpecifier.Rotation;
            }
            else if (scaleInput != Vector3.one &&
                     translationInput == Vector3.zero &&
                     rotationInput == Vector3.zero)
            {
                myTransform.transformSpecifier = TransformSpecifier.Scale;
            }
            else
            {
                myTransform.transformSpecifier = TransformSpecifier.MultipleTransforms;
            }

            noEntry = false;
            transformList.Add(myTransform);
            translationInput = Vector3.zero;
            rotationInput = Vector3.zero;
            scaleInput = Vector3.one;
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
            
            transformList.Add(absoluteTransform);
        }

        public List<Vector3> CalculateTransform(int savePointIndex, int lastIndex)
        {
            Quaternion rotationQuaternion = Quaternion.identity;
            Vector3 positionVector = Vector3.zero;
            Vector3 scaleVector = Vector3.one;
            
            List<Vector3> list = new List<Vector3>();

            for (int i = savePointIndex; i <= lastIndex; i++)
            {
                MyTransform myTransform = transformList[i];
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

        public void ApplyTransformToVertices(List<Vector3> list)
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Matrix4x4 matrix = Matrix4x4.TRS(list[0], Quaternion.Euler(list[1]), list[2]);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
            }
        }

        public string SetTransformName()
        {
            string listValue = "";
            MyTransform myTransform = transformList[^1];

            switch (myTransform.transformSpecifier)
            {
                case TransformSpecifier.Translation:
                    listValue += "Translation changed";
                    break;    
                case TransformSpecifier.Rotation:
                    listValue += "Rotation changed";
                    break;
                case TransformSpecifier.Scale:
                    listValue += "Scale changed";
                    break;
                case TransformSpecifier.MultipleTransforms:
                    listValue += "Transforms";
                    break;
                case TransformSpecifier.AbsoluteTransform:
                    listValue += "Transform Saved";
                    break;
                default:
                    listValue += "Unknown Transform Type";
                    break;
            }

            if (applyToVertices)
            {
                listValue += " // Vertices";
            }

            transformNameList.Add(listValue);
            return listValue;
        }
        
        public int FindLastAbsoluteTransformIndex(int startIndex)
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
        
        public int FindNextAbsoluteTransformIndex(int startIndex)
        {
            int nextIndex = 0;
            for (int i = startIndex; i < transformList.Count; i++)
            {
                if (transformList[i].transformType == TransformTypes.Absolute && transformList[i].IsActive)
                {
                    nextIndex = i;
                    break;
                }
            }
            
            if (nextIndex == 0)
            {
                nextIndex = transformList.Count - 1;
            }
            
            return nextIndex;
        }

        public void DeactivateTransforms(int startIndex)
        {
            var index = transformList.Count - 1;
            
            for (int i = startIndex; i < index; i++)
            {
                transformList[i].IsActive = false;
            }
        }

        public void SetActiveNotActiveOfTransforms(int startIndex, int lastIndex, int position)
        {
            for (int i = lastIndex; i > position; i--)
            {
                transformList[i].IsActive = false;
            }
            for (int i = position; i >= startIndex; i--)
            {
                transformList[i].IsActive = true;
            }
            for (int i = startIndex - 1; i >= 0; i--)
            {
                transformList[i].IsActive = false;
            }
        }

        public void ResetTransforms()
        {
            transformList.RemoveRange(1, transformList.Count - 1);
            transformNameList.RemoveRange(1, transformNameList.Count - 1);
            CalculateTransform(0,0);
        }
        #endregion
        
        private void Update()
        {
            if (Main.isInitialized) // Look if the initial state was set in main()
            {
                if (_initialStateLoaded == false) // Add the initial state of the monobehaviour
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
