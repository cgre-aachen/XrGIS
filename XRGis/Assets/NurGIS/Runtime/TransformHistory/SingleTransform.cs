using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public class SingleTransform : VisualElement
    {
        public SingleTransform(string name, int index, RadioButtonGroup radioButtonGroup, Foldout radioButtonFoldout)
        {
            ///////////////// Single Transform Foldout /////////////////
            var foldout = new Foldout
            {
                text = name
            };
            hierarchy.Add(foldout);
            ///////////////// Single Transform Foldout /////////////////

            ///////////////// Vector3Fields /////////////////
            var position = new Vector3Field
            {
                label = "Position",
                name  = "positionInput",
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
            ///////////////// Vector3Fields /////////////////
            
            ///////////////// Single Transform Buttons /////////////////
            var buttonRow = foldout.Q<Toggle>().Children().First();
            buttonRow.style.paddingBottom = 6;
            
            var renameTransformButton = new Button
            {
                text = "Rename",
                style =
                {
                    right    = 87,
                    position = Position.Absolute,
                    width    = 60,
                    paddingRight = 0
                }
            };

            var deleteTransformButton = new Button
            {
                text = "Delete",
                style =
                {
                    right    = 31,
                    position = Position.Absolute,
                    width    = 55,
                    paddingRight = 0
                }
            };
            
            var absTransformButton = new Button
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
                    paddingLeft = 10
                }
            };
            
            var appliedToVertexCheckbox = new Toggle
            {
                label = "Apply To Vertices",
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
                    paddingLeft = 10
                }
            };
            ///////////////// Single Transform Toggles /////////////////
            
            foldout.Add(toggleContainer);
            buttonRow.Add(renameTransformButton);
            buttonRow.Add(deleteTransformButton);
            buttonRow.Add(absTransformButton);
            
            toggleContainer.Add(isActiveCheckbox);
            toggleContainer.Add(appliedToVertexCheckbox);

            ////////// Callbacks //////////
            deleteTransformButton.clicked += () => {TransformGuiMethods.DeleteSingleTransform(index, radioButtonGroup);};
            deleteTransformButton.clicked += () => {TransformGuiMethods.CreateAllSingleTransforms(index, radioButtonGroup, radioButtonFoldout) ;};

            renameTransformButton.clicked += () => {TransformGuiMethods.RenameInputWindow(index, radioButtonGroup, this);};
            absTransformButton.clicked += () => {;};
            position.RegisterValueChangedCallback(evt => {;});
            rotation.RegisterValueChangedCallback(evt => {;});
            scale.RegisterValueChangedCallback(evt => {;});
            isActiveCheckbox.RegisterValueChangedCallback(evt => {;});
            appliedToVertexCheckbox.RegisterValueChangedCallback(evt => {;});
            ////////// Callbacks //////////
        }
    }
}