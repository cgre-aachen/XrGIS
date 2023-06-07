using UnityEngine;
using UnityEngine.UIElements;


// ReSharper disable once CheckNamespace
namespace GemPlayControls.Advanced
{
    public class TransformControl : VisualElement
    {
        internal enum TranformGroup
        {
            Position,
            Rotation,
            Scale
        }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<TransformControl, UxmlTraits>
        {
        }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription iconAddress = new UxmlStringAttributeDescription()
                { name = "IconAddress", defaultValue = "" };


            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var item = ve as TransformControl;
            }
        }

        /*
        <ui:Vector3Field label="Translation" name="translationInput" binding-path="positionInput" style="background-color: rgba(56, 56, 56, 0);" />
        <ui:Vector3Field label="Rotation" name="rotationInput" binding-path="rotationInput" />
        <ui:Vector3Field label="Scale" name="scaleInput" x="1" y="1" z="1" binding-path="scaleInput" />
        */
        public TransformControl()
        {
            var foldout = new Foldout
            {
                text = "A transform"
            };
            Add(foldout);
            
            var position = new Vector3Field
            {
                label = "Position",
                name  = "positionInput",
                style =
                {
                    backgroundColor = new Color(56, 56, 56, 0)
                }
            };
            foldout.Add(position);
            
            var rotation = new Vector3Field
            {
                label = "Rotation",
                name  = "rotationInput"
            };
            foldout.Add(rotation);
            
            var scale = new Vector3Field
            {
                label = "Scale",
                name  = "scaleInput",
                value = new Vector3(1, 1, 1)
            };

            foldout.Add(scale);
            
        }
    }
}