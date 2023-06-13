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
            ///////////////// Single Transform Foldout /////////////////
            m_foldout = new Foldout
                {
                    text = "Transform"
                };
            hierarchy.Add(m_foldout);
            ///////////////// Single Transform Foldout /////////////////

            ///////////////// Vector3Fields /////////////////
            var position = new Vector3Field
            {
                label = "Position",
                name  = "positionInput",
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
            ///////////////// Vector3Fields /////////////////
            
            ///////////////// Single Transform Buttons /////////////////
            var toggleRow = m_foldout.Q<Toggle>().Children().First();
            toggleRow.style.paddingBottom = 3;
            
            var addTransformButton = new Button
            {
                text = "Abs",
                style =
                {
                    right    = 0,
                    position = Position.Absolute,
                    width    = 30,
                    paddingRight = 0
                }
            };
            ///////////////// Single Transform Buttons /////////////////
        
            ///////////////// Single Transform Toggles /////////////////
            var toggleContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.FlexEnd,
                    alignItems = Align.FlexEnd,
                }
            };
                
            var isActiveCheckbox = new Toggle()
            {
                label = "Active",
                labelElement =
                {
                    style =
                    {
                        minWidth = 0,
                    }   
                },
                
                name  = "isActiveCheckbox",
                style =
                {
                    paddingLeft = 5
                }
            };
            
            var appliedToVertexCheckbox = new Toggle
            {
                label = "Applied To Vertices",
                labelElement =
                {
                    style =
                    {
                        minWidth = 0,
                    }   
                },
                
                name  = "appliedToVerticesCheckbox",
                style =
                {
                    paddingLeft = 5
                }
            };
            ///////////////// Single Transform Toggles /////////////////
            
            m_foldout.Add(toggleContainer);
            toggleRow.Add(addTransformButton);
            
            toggleContainer.Add(isActiveCheckbox);
            toggleContainer.Add(appliedToVertexCheckbox);
            
            
            

        }
    }
}