using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformChange : MonoBehaviour
    {
        //Matrix4x4 localMatrix=Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        //Matrix4x4 savedMatrix = transform.worldToLocalMatrix;
        public bool undoTransform;
        public List<Vector3> translationList;

        private Transform _gameObjectTransform;
        private Matrix4x4 _lastTranslation;

        private void TrackTransformChangeState()
        {
            Debug.Log("Transform has changed!");
            transform.hasChanged = false;
            translationList.Add(gameObject.transform.position);
        }

        private Matrix4x4 SaveTranslation(Transform gameObjectTransform)
        {
            Matrix4x4 translationMatrix = Matrix4x4.Translate(gameObjectTransform.position);
            return translationMatrix;
        }
        
        private void UndoTranslation(Matrix4x4 lastTranslation)
        {
            Matrix4x4 inv = lastTranslation.inverse;
            Debug.Log("Inverse Translation Matrix: " + inv);
            gameObject.transform.position = inv.MultiplyPoint3x4(translationList[translationList.Count - 1]);
        }
        
        private void Start()
        {
            _gameObjectTransform = gameObject.transform;
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                TrackTransformChangeState();
                _lastTranslation = SaveTranslation(_gameObjectTransform);
            }
            if (undoTransform)
            {
                undoTransform = false;
                UndoTranslation(_lastTranslation);
            }
        }
    }
}
