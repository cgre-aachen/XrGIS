using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory.GUI
{
    public class SingleTransform : VisualElement
    {
        private readonly Foldout m_foldout;
        
        public SingleTransform(string name) : this()
        {
            m_foldout.text = name;
        }

        private SingleTransform()
        {
            {
                m_foldout = new Foldout
                {
                    text = "Transform"
                };
                hierarchy.Add(m_foldout);
            }
            
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
                m_foldout.Add(position);

                var rotation = new Vector3Field
                {
                    label = "Rotation",
                    name  = "rotationInput"
                };
                m_foldout.Add(rotation);

                var scale = new Vector3Field
                {
                    label = "Scale",
                    name  = "scaleInput",
                    value = new Vector3(1, 1, 1)
                };
                m_foldout.Add(scale);
            }

            var appliedToVertexCheckbox = new Toggle
            {
                label = "Applied to vertex",
                name  = "appliedToVertexCheckbox"
            };
            m_foldout.Add(appliedToVertexCheckbox);
            
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
                
                var toggleRow = m_foldout.Q<Toggle>().Children().First();
                toggleRow.Add(addTransformButton);
                toggleRow.Add(isActiveCheckbox);
            }
        }
    }
}