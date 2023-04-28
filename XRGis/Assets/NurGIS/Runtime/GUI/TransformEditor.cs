/*
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.GUI
{
    [CustomPropertyDrawer(typeof(TransformHistory.TransformChangeInterface))]
    public class TransformEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var transformGoList = new PropertyField(property.FindPropertyRelative("transformGoList"));
            root.Add(transformGoList);

            var spawnInspector = new Box();
            root.Add(spawnInspector);
            
            transformGoList.RegisterCallback<ChangeEvent<Object>, VisualElement>(
                NewValueAdded, spawnInspector);
            
            return root;
        }
        
        private static void NewValueAdded(ChangeEvent<Object> evt, VisualElement spawnInspector)
        {
            var go = evt.newValue as GameObject;
            if (go != null)
            {
                var helper = go.GetComponent<TransformHistory.TransformChangeInterface>();
                if (helper != null)
                {
                    var inspectorElement = new PropertyField(new SerializedObject(helper).FindProperty("transformGoList"));
                    spawnInspector.Add(inspectorElement);
                }
            }
        }
    }
}

*/