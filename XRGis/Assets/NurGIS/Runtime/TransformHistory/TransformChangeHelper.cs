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
        
        public class CustomTransform
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public TransformTypes transformType;
            public TransformSpecifier transformSpecifier;
            public bool IsActive;
            public string transformName;

            public CustomTransform Clone()
            {
                CustomTransform clone = new CustomTransform
                {
                    position = position,
                    rotation = rotation,
                    scale = scale,
                    transformType = transformType,
                    transformSpecifier = transformSpecifier,
                    IsActive = IsActive,
                    transformName = transformName
                };
                return clone;
            }
        }
        
        public class CustomTransformContainer
        {
            public List<CustomTransform> singleTransformList;
            public string transformListName;
        }
        
        public readonly List<CustomTransformContainer> transformListContainer = new();

        public bool noEntry;
        public bool applyToVertices;
        public int activeRadioButton;

        public Vector3 positionInput = Vector3.zero;
        public Vector3 scaleInput = Vector3.one;
        public Vector3 rotationInput = Vector3.zero;
        private Vector3 RotationInput // set to round to nearest 0.2 in GUI
        {
            set
            {
                rotationInput = value;
                rotationInput.x = Mathf.Round(rotationInput.x * 5)/5;
                rotationInput.y = Mathf.Round(rotationInput.y * 5)/5;
                rotationInput.z = Mathf.Round(rotationInput.z * 5)/5;
            }
        }
        
        #endregion
        
        #region Methods
        public void SaveRelativeTransformFromGui(Vector3 translation, Vector3 rotation, Vector3 scale, List<CustomTransform> activeTransformListInput)
        {
            if (translation == Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                noEntry = true;
                Debug.Log("Enter a value");
                return;
            }
            
            CustomTransform customTransform = new CustomTransform
            {
                transformType = TransformTypes.Relative,
                IsActive = true,
                position = translation,
                rotation = rotation,
                scale = scale,
            };
            
            if (translation != Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                customTransform.transformSpecifier = TransformSpecifier.Translation;
            }
            else if (rotation != Vector3.zero &&
                     translation == Vector3.zero &&
                     scale == Vector3.one)
            {
                customTransform.transformSpecifier = TransformSpecifier.Rotation;
            }
            else if (scale != Vector3.one &&
                     translation == Vector3.zero &&
                     rotation == Vector3.zero)
            {
                customTransform.transformSpecifier = TransformSpecifier.Scale;
            }
            else
            {
                customTransform.transformSpecifier = TransformSpecifier.MultipleTransforms;
            }

            string transformName = SetTransformName(customTransform);
            customTransform.transformName = transformName;
            noEntry = false;
            activeTransformListInput.Add(customTransform);
        }

        public void SaveAbsoluteTransform(Vector3 translation, Vector3 rotation, Vector3 scale, List<CustomTransform> activeTransformListInput)
        {
            CustomTransform absoluteTransform = new CustomTransform()
            {
                position = translation,
                rotation = rotation,
                scale = scale,
                transformType = TransformTypes.Absolute,
                transformSpecifier = TransformSpecifier.AbsoluteTransform,
                IsActive = true,
            };
            string transformName = SetTransformName(absoluteTransform);
            absoluteTransform.transformName = transformName;
            activeTransformListInput.Add(absoluteTransform);
        }

        public List<Vector3> CalculateTransform(int previousAbsolute, int position, List<CustomTransform> activeTransformListInput)
        {
            Vector3 positionVector = Vector3.zero;
            Vector3 rotationQuaternion = Vector3.zero;
            Vector3 scaleVector = Vector3.one;
            
            List<Vector3> list = new List<Vector3>();

            for (int i = previousAbsolute; i <= position; i++)
            {
                CustomTransform customTransform = activeTransformListInput[i];
                if (!customTransform.IsActive) continue;
                rotationQuaternion += customTransform.rotation;
                positionVector += customTransform.position;
                scaleVector = Vector3.Scale(scaleVector, customTransform.scale);
            }
            
            list.Add(positionVector);
            list.Add(rotationQuaternion);
            list.Add(scaleVector);

            return list;
        }
        
        public List<Vector3> CalculateSliderTransform(int previousAbsolute, int position, List<CustomTransform> activeTransformListInput)
        {
            Vector3 positionVector = Vector3.zero;
            Vector3 rotationVector = Vector3.zero;
            Vector3 scaleVector = Vector3.one;
            
            List<Vector3> list = new List<Vector3>();

            for (int i = previousAbsolute; i <= position; i++)
            {
                CustomTransform customTransform = activeTransformListInput[i];
                if (!customTransform.IsActive) continue;
                positionVector += customTransform.position;
                rotationVector += customTransform.rotation;
                scaleVector = Vector3.Scale(scaleVector, customTransform.scale);
            }
            
            list.Add(positionVector);
            list.Add(rotationVector);
            list.Add(scaleVector);

            return list;
        }

        public void ApplyTransformToGo(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            GameObject go = gameObject;
            go.transform.localPosition = translation;
            go.transform.localRotation = Quaternion.Euler(rotation);
            go.transform.localScale = scale;
            transform.hasChanged = false;
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
            transform.hasChanged = false;
        }

        public void UpdateGameObjectTranslation(Vector3 translation, Vector3 lastTranslation)
        {
            GameObject go = gameObject;
            lastTranslation += translation;
            go.transform.localPosition = lastTranslation;
            transform.hasChanged = false;
        }
        
        public void UpdateGameObjectRotation(Vector3 rotation, Vector3 lastRotation)
        {
            GameObject go = gameObject;
            lastRotation += rotation; //// this is the problem, check how we add quaternions
            go.transform.localRotation = Quaternion.Euler(lastRotation);
            transform.hasChanged = false;
        }
        
        public void UpdateGameObjectScale(Vector3 scale, Vector3 lastScale)
        {
            GameObject go = gameObject;
            lastScale = Vector3.Scale(lastScale, scale);
            go.transform.localScale = lastScale;
            transform.hasChanged = false;
        }

        private string SetTransformName(CustomTransform customTransform)
        {
            string transformName = "";
            
            switch (customTransform.transformSpecifier)
            {
                case TransformSpecifier.Translation:
                    transformName += "Translation Changed";
                    break;    
                case TransformSpecifier.Rotation:
                    transformName += "Rotation Changed";
                    break;
                case TransformSpecifier.Scale:
                    transformName += "Scale Changed";
                    break;
                case TransformSpecifier.MultipleTransforms:
                    transformName += "Transform Changed";
                    break;
                case TransformSpecifier.AbsoluteTransform:
                    transformName += "Transform Saved";
                    break;
                default:
                    transformName += "Unknown Transform Type";
                    break;
            }

            if (applyToVertices)
            {
                transformName += " // Vertices";
            }

            return transformName;
        }
        
        public int FindLastAbsoluteTransformIndex(int startIndex, bool onlyActiveTransforms, List<CustomTransform> activeTransformListInput)
        {
            int lastIndex = 0;
            for (int i = startIndex - 1; i >= 0; i--)
            {
                if (onlyActiveTransforms)
                {
                    if (activeTransformListInput[i].transformType == TransformTypes.Absolute && activeTransformListInput[i].IsActive)
                    {
                        lastIndex = i;
                        break;
                    }
                }
                else
                {
                    if (activeTransformListInput[i].transformType == TransformTypes.Absolute)
                    {
                        lastIndex = i;
                        break;
                    }
                }
            }
            
            return lastIndex;
        }
        
        public int FindNextAbsoluteTransformIndex(int startIndex, bool onlyActiveTransforms, List<CustomTransform> activeTransformListInput)
        {
            int nextIndex = activeTransformListInput.Count - 1;
            for (int i = startIndex + 1; i < activeTransformListInput.Count; i++)
            {
                if (onlyActiveTransforms)
                {
                    if (activeTransformListInput[i].transformType == TransformTypes.Absolute && activeTransformListInput[i].IsActive)
                    {
                        nextIndex = i;
                        break;
                    }
                }
                else
                {
                    if (activeTransformListInput[i].transformType == TransformTypes.Absolute)
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }
            
            if (nextIndex == 0)
            {
                nextIndex = activeTransformListInput.Count - 1;
            }
            
            return nextIndex;
        }

        public void DeactivateTransforms(int startIndex, List<CustomTransform> activeTransformListInput)
        {
            var index = activeTransformListInput.Count - 1;
            
            for (int i = startIndex; i < index; i++)
            {
                activeTransformListInput[i].IsActive = false;
            }
        }

        public void SetActiveNotActiveOfTransforms(int position, int previousAbsolute, int nextAbsolute, List<CustomTransform> activeTransformListInput)
        {
            for (int i = nextAbsolute; i > position; i--)
            {
                activeTransformListInput[i].IsActive = false;
            }
            for (int i = position; i >= previousAbsolute; i--)
            {
                activeTransformListInput[i].IsActive = true;
            }
            for (int i = previousAbsolute - 1; i >= 0; i--)
            {
                activeTransformListInput[i].IsActive = false;
            }

            if (activeTransformListInput[position].transformType == TransformTypes.Absolute)
            {
                for (int i = 0; i < activeTransformListInput.Count - 1; i++)
                {
                    activeTransformListInput[i].IsActive = false;
                }
                activeTransformListInput[position].IsActive = true;
            }
        }

        public void ToggleAbsoluteTransform(int position, int previousAbsolute, CustomTransform customTransform, List<CustomTransform> activeTransformListInput)
        {
            if (activeTransformListInput.Count == 1)
            {
                return;
            }

            if (customTransform.IsActive) //Scenario 1; Absolute toggle gets activated
            {
                for (int i = position - 1; i >= 0; i--)
                {
                    if (position == 0) // If the absolute transform is the first transform in the list
                    {
                        break;
                    }
                    
                    activeTransformListInput[i].IsActive = false;
                }
                
                for (int i = position + 1; i < activeTransformListInput.Count; i++) 
                {
                    if (activeTransformListInput[i].transformType == TransformTypes.Absolute)
                    {
                        activeTransformListInput[i].IsActive = false;
                    }
                    else
                    {
                        activeTransformListInput[i].IsActive = true;
                    }
                }
            }
            
            else // Scenario 2; Absolute toggle gets deactivated
            {
                for (int i = position - 1; i >= previousAbsolute; i--)
                {
                    if (activeTransformListInput[i].transformType != TransformTypes.Absolute)
                    {
                        activeTransformListInput[i].IsActive = true;
                    }
                    else
                    {
                        activeTransformListInput[i].IsActive = true;
                        break;
                    }
                }
            }
        }
        
        public void CreateNewTransformListEntry(bool calledAtStart)
        {
            var newTransform = new CustomTransform();
            if (calledAtStart)
            {
                var o = gameObject;

                newTransform.position = o.transform.position;   
                newTransform.rotation = o.transform.rotation.eulerAngles;
                newTransform.scale = o.transform.localScale;
                newTransform.transformType = TransformTypes.Absolute;
                newTransform.transformSpecifier = TransformSpecifier.NoTransform;
                newTransform.IsActive = true;
                newTransform.transformName = "Start Transform";
            }
            else
            {
                newTransform.position = Vector3.zero;
                newTransform.rotation = Vector3.zero;
                newTransform.scale = Vector3.one;
                newTransform.transformType = TransformTypes.Absolute;
                newTransform.transformSpecifier = TransformSpecifier.NoTransform;
                newTransform.IsActive = true;
                newTransform.transformName = "Start Transform";
            }


            
            var emptyTransformList = new List<CustomTransform> { newTransform };
            
            CustomTransformContainer transformListEntry = new CustomTransformContainer
            {
                singleTransformList = emptyTransformList,
                transformListName = "Transform List " + transformListContainer.Count
            };
                    
            transformListContainer.Add(transformListEntry);
        }
        
        public void CopyTransformListEntry(int index)
        {
            CustomTransformContainer transformListEntry = new CustomTransformContainer
            {
                transformListName = transformListContainer[index].transformListName + " Copy"
            };

            List<CustomTransform> oldList = transformListContainer[index].singleTransformList;
            List<CustomTransform> newList = new List<CustomTransform>(oldList.Count);
            
            oldList.ForEach((item)=>
            {
                newList.Add(item.Clone());
            });
            
            transformListEntry.singleTransformList = newList;
            transformListContainer.Add(transformListEntry);
        }

        private void GetRelativeTransform(int selectedRadioButton) // Calculate the rel. transform, triggered by transform.hasChanged
        {
            if (selectedRadioButton == -1)
            {
                return;
            }
            
            GameObject go = gameObject;
            var transform1 = go.transform;
            Vector3 newPosition = transform1.localPosition;
            Vector3 newRotation = transform1.localRotation.eulerAngles;
            Vector3 newScale = transform1.localScale;
            
           List<CustomTransform> activeTransformList = transformListContainer[selectedRadioButton].singleTransformList;
            
            Vector3 lastPosition = activeTransformList[^1].position;
            Vector3 lastRotation = activeTransformList[^1].rotation;
            Vector3 lastScale = activeTransformList[^1].scale;
            
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
            RotationInput = relativeRotation;
            scaleInput = relativeScale;
        }
        #endregion

        private void Awake() // Initial state is in awake so that the GUI reflects it (Start is too late) 
        {
            CreateNewTransformListEntry(true);
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                GetRelativeTransform(activeRadioButton);
                transform.hasChanged = false;
            }
        }
    }
}
