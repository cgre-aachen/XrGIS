using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public class SingleTransform : VisualElement
    {
        public SingleTransform(string name, int singleTransformIndex, int transformGroupIndex, RadioButtonGroup radioButtonGroup, Foldout radioButtonFoldout, GameObject go)
        {
            ///////////////// Single Transform Foldout /////////////////
            var foldout = new Foldout
            {
                text = name
            };
            hierarchy.Add(foldout);
            ///////////////// Single Transform Foldout /////////////////

            ///////////////// Vector3Fields /////////////////
            var activeTransformList = TransformMonobehaviour.TransformListContainer[transformGroupIndex].singleTransformList;
            
            var position = new Vector3Field
            {
                label = "Position",
                name  = "positionInput",
                value = activeTransformList[singleTransformIndex].position  
            };
            foldout.Add(position);

            var rotation = new Vector3Field
            {
                label = "Rotation",
                name  = "rotationInput",
                value = activeTransformList[singleTransformIndex].rotation
            };
            foldout.Add(rotation);

            var scale = new Vector3Field
            {
                label = "Scale",
                name  = "scaleInput",
                value = activeTransformList[singleTransformIndex].scale
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
                },
                
                value = activeTransformList[singleTransformIndex].isActive
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
            deleteTransformButton.clicked += () => {TransformGuiMethods.DeleteSingleTransform(singleTransformIndex, radioButtonGroup);};
            deleteTransformButton.clicked += () => {TransformGuiMethods.CreateAllSingleTransforms(radioButtonGroup.value, radioButtonGroup, radioButtonFoldout, go) ;};

            renameTransformButton.clicked += () => {TransformGuiMethods.RenameSingleTransformInputWindow(singleTransformIndex, radioButtonGroup, this);};
            
            absTransformButton.clicked += () => {TransformGuiMethods.MakeTransformAbsolute(singleTransformIndex, TransformGuiMethods.GetTransformGroupNames(TransformMonobehaviour.TransformListContainer), radioButtonGroup);};
            absTransformButton.clicked += () => { TransformGuiMethods.DrawGUI(go, radioButtonGroup); };
            
            position.RegisterValueChangedCallback(evt => {TransformGuiMethods.UpdateTransform(evt.newValue, Vector3.zero, Vector3.one, radioButtonGroup, singleTransformIndex, go);});
            rotation.RegisterValueChangedCallback(evt => {TransformGuiMethods.UpdateTransform(Vector3.zero, evt.newValue, Vector3.one, radioButtonGroup, singleTransformIndex, go);});
            scale.RegisterValueChangedCallback(evt => {TransformGuiMethods.UpdateTransform(Vector3.zero,Vector3.zero , evt.newValue, radioButtonGroup, singleTransformIndex, go);});
            
            isActiveCheckbox.RegisterValueChangedCallback(evt => {activeTransformList[singleTransformIndex].isActive = evt.newValue; TransformGuiMethods.UpdateTransform(Vector3.zero,Vector3.zero , Vector3.one, radioButtonGroup, singleTransformIndex, go); });
            appliedToVertexCheckbox.RegisterValueChangedCallback(evt => {;});
            ////////// Callbacks //////////
        }
    }
}