# if false
using UnityEngine;

namespace NurGIS.Runtime.TransformHistory
{
    public static class TransformGoOperations
    {
        public static void ApplyTransformToGo(TransformMonobehaviour mono, Vector3 translation, Vector3 rotation, Vector3 scale) // combine to a single method
        {
            var gameObject = mono.gameObject;
            gameObject.transform.localPosition = translation;
            gameObject.transform.localRotation = Quaternion.Euler(rotation);
            gameObject.transform.localScale = scale;
            gameObject.transform.hasChanged = false;
        }
        
        public static void UpdateGameObjectTranslation(TransformMonobehaviour mono, Vector3 translation, Vector3 lastTranslation)
        {
            var gameObject = mono.gameObject;
            lastTranslation += translation;
            gameObject.transform.localPosition = lastTranslation;
            mono.transform.hasChanged = false;
        } 
        
        public static void UpdateGameObjectRotation(TransformMonobehaviour mono, Vector3 rotation, Vector3 lastRotation)
        {
            var gameObject = mono.gameObject;
            lastRotation += rotation; 
            gameObject.transform.localRotation = Quaternion.Euler(lastRotation);
            mono.transform.hasChanged = false;
        }
        
        public static void UpdateGameObjectScale(TransformMonobehaviour mono, Vector3 scale, Vector3 lastScale)
        {
            var gameObject = mono.gameObject;
            lastScale = Vector3.Scale(lastScale, scale);
            gameObject.transform.localScale = lastScale;
            mono.transform.hasChanged = false;
        }
    }
}
#endif