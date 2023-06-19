using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public static class TransformGoOperations
    {
        public static void ApplyTransformToGo(GameObject go, Vector3 translation, Vector3 rotation, Vector3 scale) // combine to a single method
        {
            go.transform.localPosition = translation;
            go.transform.localRotation = Quaternion.Euler(rotation);
            go.transform.localScale = scale;
            go.transform.hasChanged = false;
        }
        
        public static void UpdateGameObjectTranslation(GameObject go, Vector3 translation, Vector3 lastTranslation)
        {
            lastTranslation += translation;
            go.transform.localPosition = lastTranslation;
            go.transform.hasChanged = false;
        } 
        
        public static void UpdateGameObjectRotation(GameObject go, Vector3 rotation, Vector3 lastRotation)
        {
            lastRotation += rotation; 
            go.transform.localRotation = Quaternion.Euler(lastRotation);
            go.transform.hasChanged = false;
        }
        
        public static void UpdateGameObjectScale(GameObject go, Vector3 scale, Vector3 lastScale)
        {
            lastScale = Vector3.Scale(lastScale, scale);
            go.transform.localScale = lastScale;
            go.transform.hasChanged = false;
        }
    }
}