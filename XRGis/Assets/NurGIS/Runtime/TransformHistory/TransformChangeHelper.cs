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
        
        public bool noEntry;
        public bool applyToVertices;
        private bool _initialStateLoaded;

        public Vector3 positionInput = Vector3.zero;
        public Vector3 rotationInput = Vector3.zero;
        public Vector3 scaleInput = Vector3.one;
        
        
        #endregion
        
        #region Methods
        public void SaveRelativeTransformFromGui(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            if (translation == Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                noEntry = true;
                Debug.Log("Enter a value");
                return;
            }
            
            MyTransform myTransform = new MyTransform
            {
                transformType = TransformTypes.Relative,
                IsActive = true,
                position = translation,
                rotation = Quaternion.Euler(rotation),
                scale = scale
            };
            
            if (translation != Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                myTransform.transformSpecifier = TransformSpecifier.Translation;
            }
            else if (rotation != Vector3.zero &&
                     translation == Vector3.zero &&
                     scale == Vector3.one)
            {
                myTransform.transformSpecifier = TransformSpecifier.Rotation;
            }
            else if (scale != Vector3.one &&
                     translation == Vector3.zero &&
                     rotation == Vector3.zero)
            {
                myTransform.transformSpecifier = TransformSpecifier.Scale;
            }
            else
            {
                myTransform.transformSpecifier = TransformSpecifier.MultipleTransforms;
            }

            noEntry = false;
            transformList.Add(myTransform);
        }

        public void SaveAbsoluteTransform(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            MyTransform absoluteTransform = new MyTransform()
            {
                position = translation,
                rotation = Quaternion.Euler(rotation),
                scale = scale,
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

        public void ApplyTransformToGo(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            GameObject go = gameObject;
            go.transform.localPosition = translation;
            go.transform.localRotation = Quaternion.Euler(rotation);
            go.transform.localScale = scale;
        }

        public void UpdateGameObjectTranslation(Vector3 translation, Vector3 lastTranslation)
        {
            GameObject go = gameObject;
            var localPosition = lastTranslation;
            localPosition += translation;
            go.transform.localPosition = localPosition;
        }
        
        public void UpdateGameObjectRotation(Vector3 rotation, Quaternion lastRotation)
        {
            GameObject go = gameObject;
            var localRotation = lastRotation;
            localRotation *= Quaternion.Euler(rotation);
            go.transform.localRotation = localRotation;
        }
        
        public void UpdateGameObjectScale(Vector3 scale, Vector3 lastScale)
        {
            GameObject go = gameObject;
            var localScale = lastScale;
            localScale = Vector3.Scale(localScale, scale);
            go.transform.localScale = localScale;
        }
        
        public void ApplyTransformToVertices(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Matrix4x4 matrix = Matrix4x4.TRS(translation, Quaternion.Euler(rotation), scale);
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
                    listValue += "Transform changed";
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
        
        public int FindLastAbsoluteTransformIndex(int startIndex, bool onlyActiveTransforms)
        {
            int lastIndex = 0;
            for (int i = startIndex; i >= 0; i--)
            {
                if (onlyActiveTransforms)
                {
                    if (transformList[i].transformType == TransformTypes.Absolute && transformList[i].IsActive)
                    {
                        lastIndex = i;
                        break;
                    }
                }
                else
                {
                    if (transformList[i].transformType == TransformTypes.Absolute)
                    {
                        lastIndex = i;
                        break;
                    }
                }
            }
            return lastIndex;
        }
        
        public int FindNextAbsoluteTransformIndex(int startIndex, bool onlyActiveTransforms)
        {
            int nextIndex = 0;
            for (int i = startIndex; i < transformList.Count; i++)
            {
                if (onlyActiveTransforms)
                {
                    if (transformList[i].transformType == TransformTypes.Absolute && transformList[i].IsActive)
                    {
                        nextIndex = i;
                        break;
                    }
                }
                else
                {
                    if (transformList[i].transformType == TransformTypes.Absolute)
                    {
                        nextIndex = i;
                        break;
                    }
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

            if (transformList[position].transformType == TransformTypes.Absolute)
            {
                for (int i = 0; i < transformList.Count - 1; i++)
                {
                    transformList[i].IsActive = false;
                }
                transformList[position].IsActive = true;
            }
        }

        public void ResetTransforms()
        {
            transformList.RemoveRange(1, transformList.Count - 1);
            transformNameList.RemoveRange(1, transformNameList.Count - 1);
            CalculateTransform(0,0);
        }

        public void GetRelativeTransform()
        {
            GameObject go = gameObject;
            var transform1 = go.transform;
            Vector3 newPosition = transform1.localPosition;
            Vector3 newRotation = transform1.localRotation.eulerAngles;
            Vector3 newScale = transform1.localScale;
            
            Vector3 lastPosition = transformList[^1].position;
            Vector3 lastRotation = transformList[^1].rotation.eulerAngles;
            Vector3 lastScale = transformList[^1].scale;
            
            Vector3 relativePosition = newPosition - lastPosition;
            Vector3 relativeRotation = newRotation - lastRotation;
            
            Vector3 relativeScale = new Vector3
            {
                x = newScale.x / lastScale.x,
                y = newScale.y / lastScale.y,
                z = newScale.z / lastScale.z
            };

            if (relativeScale == Vector3.zero)
            {
                relativeScale = Vector3.one;
            }
            
            positionInput = relativePosition;
            rotationInput = relativeRotation;
            scaleInput = relativeScale;
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

            if (transform.hasChanged)
            {
                GetRelativeTransform();
                transform.hasChanged = false;
            }
        }
    }
}
