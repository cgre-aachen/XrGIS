using NurGIS.Runtime.TransformHistory;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(TransformChangeInterface))]
    public class NewBehaviourScript : UnityEditor.Editor
    {
        private SerializedProperty _transformGoList;
        
        private void OnEnable()
        {
            _transformGoList = serializedObject.FindProperty("transformGoList");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _transformGoList.arraySize = EditorGUILayout.IntField("# of Transforms", _transformGoList.arraySize);
            
            for (int i = 0; i < _transformGoList.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_transformGoList.GetArrayElementAtIndex(i));
            }
            
            TransformChangeInterface transformChangeInterface = (TransformChangeInterface)target;
            EditorGUILayout.Slider(0,0, transformChangeInterface.transformGoList.Count - 1);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
