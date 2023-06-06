using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformMonobehaviour : MonoBehaviour
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
            public bool isActive;
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
                    isActive = isActive,
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
        public List<int> selectedTransformListIndex = new List<int>();

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
        
        private void Awake() // Initial state is in awake so that the GUI reflects it (Start is too late) 
        {
            TransformGuiMethods.CreateNewTransformListEntry(this, true);
        }
    
        private void Update()
        {
            if (!transform.hasChanged) return;
            TransformCalculation.GetRelativeTransform(this, activeRadioButton);
            transform.hasChanged = false;
        }
    }
}