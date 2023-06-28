using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory.Editor
{
    #if true
    [CustomEditor(typeof(TransformMonobehaviour))]
    public class TransformUI : UnityEditor.Editor
    {
        public VisualTreeAsset mUxml;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            mUxml.CloneTree(root);
            
            var selectedGameObject = target.GameObject();
                
            var transformListContainer = new TransformListContainer(selectedGameObject);
            root.Add(transformListContainer);
            return root;
        }
    }
    #endif
}
