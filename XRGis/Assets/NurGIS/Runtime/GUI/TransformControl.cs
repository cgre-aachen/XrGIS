using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


// ReSharper disable once CheckNamespace
namespace NurGIS.Runtime.GUI
{
    public class TransformControl : VisualElement
    {
        private Foldout _foldout;

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
        public override VisualElement contentContainer { get; }

        public TransformControl(string name): this()
        {
           _foldout.name = name; 
        }
        public TransformControl()
        
        {
            // style.flexDirection = FlexDirection.Row;
            // var rowContainer = new VisualElement
            // {
            //     name = "rowContainer"
            // };
            //
            // hierarchy.Add(rowContainer);
            _foldout = new Foldout
            {
                text = "Transform"
            };
            hierarchy.Add(_foldout);

            // var buttonsContainer = new VisualElement
            // {
            //     name = "buttonsContainer"
            // };
            //
            // rowContainer.Add(buttonsContainer);

            var position = new Vector3Field
            {
                label = "Position",
                name  = "positionInput",
                style =
                {
                    backgroundColor = new Color(56, 56, 56, 0)
                }
            };
            _foldout.Add(position);

            var rotation = new Vector3Field
            {
                label = "Rotation",
                name  = "rotationInput"
            };
            _foldout.Add(rotation);

            var scale = new Vector3Field
            {
                label = "Scale",
                name  = "scaleInput",
                value = new Vector3(1, 1, 1)
            };

            _foldout.Add(scale);

            var appliedToVertexCheckbox = new Toggle
            {
                label = "Applied to vertex",
                name  = "appliedToVertexCheckbox"
            };
            _foldout.Add(appliedToVertexCheckbox);

            var addTransformButton = new Button
            {
                text = "Abs",
                style =
                {
                    right = 0,
                    position = Position.Absolute
                }
            };
            var toggleRow = _foldout.Q<Toggle>().Children().First();
            toggleRow.Add(addTransformButton);
        }
    }
}