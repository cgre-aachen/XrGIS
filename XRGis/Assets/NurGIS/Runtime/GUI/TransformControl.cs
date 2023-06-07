using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


// ReSharper disable once CheckNamespace
namespace NurGIS.Runtime.GUI
{
    public class TransformControl : VisualElement
    {
        private Foldout _foldout;

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
        public override VisualElement contentContainer { get; }

        public TransformControl(string name) : this()
        {
            _foldout.text = name;
        }

        public TransformControl()
        {
            // * Foldout
            {
                _foldout = new Foldout
                {
                    text = "Transform"
                };
                hierarchy.Add(_foldout);
            }

            // * Vector3Fields
            {
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
            }

            var appliedToVertexCheckbox = new Toggle
            {
                label = "Applied to vertex",
                name  = "appliedToVertexCheckbox"
            };
            _foldout.Add(appliedToVertexCheckbox);

            // * Transform buttons
            {
                var addTransformButton = new Button
                {
                    text = "Abs",
                    style =
                    {
                        right    = 0,
                        position = Position.Absolute
                    }
                };
            
                var isActiveCheckbox = new Toggle()
                {
                    text = "Active",
                    style =
                    {
                        right    = 20,
                        position = Position.Absolute
                    },
                };
                isActiveCheckbox.RegisterCallback((ChangeEvent<bool> evt) => { throw new NotImplementedException(); });

            
                var toggleRow = _foldout.Q<Toggle>().Children().First();
                toggleRow.Add(addTransformButton);
                toggleRow.Add(isActiveCheckbox);
            }
        }
    }
}