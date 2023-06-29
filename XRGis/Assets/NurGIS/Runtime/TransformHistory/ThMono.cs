using System;
using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public class ThMono : MonoBehaviour
    {
        [Serializable]
        public class CustomTransform
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public bool isActive;
            public bool appliedToVertices;
            public string transformName;

            public CustomTransform Clone()
            {
                CustomTransform clone = new CustomTransform
                {
                    position = position,
                    rotation = rotation,
                    scale = scale,
                    isActive = isActive,
                    appliedToVertices = appliedToVertices,
                    transformName = transformName
                };
                return clone;
            }
        }
        
        [Serializable]
        public class CustomTransformContainer
        {
            public List<CustomTransform> singleTransformList;
            public string transformListName;
        }
        
        public static readonly List<CustomTransformContainer> TransformListContainer = new();
        
        private void Awake() // Initial state is in awake so that the GUI reflects it (Start is too late) 
        {
            ThMethods.SaveStartPosition(gameObject);
        }
        
        private void OnDestroy()
        {
            TransformListContainer.Clear();
        }
    }
}