using System.Collections.Generic;
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public static class TransformCalculation
    {
        public static List<Vector3> CalculateTransform(int previousAbsolute, int position, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            var positionVector = Vector3.zero;
            var rotationEulerAngle = Vector3.zero;
            var scaleVector = Vector3.one;
            
            var list = new List<Vector3>();

            for (var i = previousAbsolute; i <= position; i++)
            {
                TransformMonobehaviour.CustomTransform customTransform = activeTransformListInput[i];
                if (!customTransform.isActive) continue;
                rotationEulerAngle += customTransform.rotation;
                positionVector += customTransform.position;
                scaleVector = Vector3.Scale(scaleVector, customTransform.scale);
            }
            
            list.Add(positionVector);
            list.Add(rotationEulerAngle);
            list.Add(scaleVector);

            return list;
        }
        
        public static List<Vector3> CalculateSliderTransform(int previousAbsolute, int position, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            var positionVector = Vector3.zero;
            var rotationVector = Vector3.zero;
            var scaleVector = Vector3.one;
            
            var list = new List<Vector3>();

            for (var i = previousAbsolute; i <= position; i++)
            {
                TransformMonobehaviour.CustomTransform customTransform = activeTransformListInput[i];
                if (!customTransform.isActive) continue;
                positionVector += customTransform.position;
                rotationVector += customTransform.rotation;
                scaleVector = Vector3.Scale(scaleVector, customTransform.scale);
            }
            
            list.Add(positionVector);
            list.Add(rotationVector);
            list.Add(scaleVector);

            return list;
        }
        
        public static void ApplyTransformToVertices(TransformMonobehaviour mono, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            var mesh = mono.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Matrix4x4 matrix = Matrix4x4.TRS(translation, Quaternion.Euler(rotation), scale);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
            }
            mono.transform.hasChanged = false;
        }
        
                public static int FindLastAbsoluteTransformIndex(int startIndex, bool onlyActiveTransforms, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            var lastIndex = 0;
            for (var i = startIndex - 1; i >= 0; i--)
            {
                if (onlyActiveTransforms)
                {
                    if (activeTransformListInput[i].transformType != TransformMonobehaviour.TransformTypes.Absolute ||
                        !activeTransformListInput[i].isActive) continue;
                    lastIndex = i;
                    break;
                }
                
                if (activeTransformListInput[i].transformType !=
                    TransformMonobehaviour.TransformTypes.Absolute) continue;
                lastIndex = i;
                break;
            }
            
            return lastIndex;
        }
        
        public static int FindNextAbsoluteTransformIndex(int startIndex, bool onlyActiveTransforms, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            var nextIndex = activeTransformListInput.Count - 1;
            for (var i = startIndex + 1; i < activeTransformListInput.Count; i++)
            {
                if (onlyActiveTransforms)
                {
                    if (activeTransformListInput[i].transformType != TransformMonobehaviour.TransformTypes.Absolute ||
                        !activeTransformListInput[i].isActive) continue;
                    nextIndex = i;
                    break;
                }

                if (activeTransformListInput[i].transformType !=
                    TransformMonobehaviour.TransformTypes.Absolute) continue;
                nextIndex = i;
                break;
            }
            
            if (nextIndex == 0)
            {
                nextIndex = activeTransformListInput.Count - 1;
            }
            
            return nextIndex;
        }
        
        public static void GetRelativeTransform(TransformMonobehaviour mono, int selectedRadioButton) // Calculate the rel. transform, triggered by transform.hasChanged
        {
            if (selectedRadioButton == -1)
            {
                return;
            }
            
            var go = mono.gameObject;
            var transform1 = go.transform;
            var newPositionVector3 = transform1.localPosition;
            var newRotationVector3 = transform1.localRotation.eulerAngles;
            var newScaleVector3 = transform1.localScale;
            
            List<TransformMonobehaviour.CustomTransform> activeTransformList = mono.transformListContainer[selectedRadioButton].singleTransformList;
            
            var lastPositionVector3 = activeTransformList[^1].position;
            var lastRotationVector3 = activeTransformList[^1].rotation;
            var lastScaleVector3 = activeTransformList[^1].scale;
            
            var relativePositionVector3 = newPositionVector3 - lastPositionVector3;
            var relativeRotationVector3 = newRotationVector3 - lastRotationVector3;
            
            var relativeScaleVector3 = new Vector3
            {
                x = newScaleVector3.x / lastScaleVector3.x,
                y = newScaleVector3.y / lastScaleVector3.y,
                z = newScaleVector3.z / lastScaleVector3.z
            };

            if (relativeScaleVector3 == Vector3.zero)
            {
                relativeScaleVector3 = Vector3.one;
            }
            
            mono.positionInput = relativePositionVector3;
            mono.rotationInput = relativeRotationVector3;
            mono.scaleInput = relativeScaleVector3;
        }
    }
}