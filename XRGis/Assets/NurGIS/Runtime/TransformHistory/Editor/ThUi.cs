using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory.Editor
{
    [CustomEditor(typeof(ThMono))]
    public class ThUi : UnityEditor.Editor
    {
        public VisualTreeAsset mUxml;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            mUxml.CloneTree(root);
            
            var selectedGameObject = target.GameObject();
                
            var transformListContainer = new ThListContainer(selectedGameObject);
            root.Add(transformListContainer);
            return root;
        }
    }
}
