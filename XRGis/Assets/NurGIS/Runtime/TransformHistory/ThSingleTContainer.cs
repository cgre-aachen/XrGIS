using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public class ThSingleTContainer : VisualElement
    {
        public ThSingleTContainer(string name, int singleTransformIndex, int transformGroupIndex, RadioButtonGroup radioButtonGroup, Foldout radioButtonFoldout, GameObject go)
        {
            ///////////////// Single Transform Foldout /////////////////
            #region foldoutStyle
            var foldout = new Foldout
            {
                text = name
            };
            hierarchy.Add(foldout);
            #endregion
            ///////////////// Single Transform Foldout /////////////////

            ///////////////// Vector3Fields /////////////////
            #region Vector3FieldsStyle
            var activeTransformList = ThMono.TransformListContainer[transformGroupIndex].singleTransformList;
            
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
            #endregion
            ///////////////// Vector3Fields /////////////////
            
            ///////////////// Single Transform Buttons /////////////////
            #region buttonStyle
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
            #endregion
            ///////////////// Single Transform Buttons /////////////////
        
            ///////////////// Single Transform Toggles /////////////////
            #region toggleStyle
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
            #endregion
            ///////////////// Single Transform Toggles /////////////////
            
            foldout.Add(toggleContainer);
            buttonRow.Add(renameTransformButton);
            buttonRow.Add(deleteTransformButton);
            buttonRow.Add(absTransformButton);
            
            toggleContainer.Add(isActiveCheckbox);
            toggleContainer.Add(appliedToVertexCheckbox);

            ////////// Callbacks //////////
            #region Callbacks
            deleteTransformButton.clicked += () => {ThMethods.DeleteSingleTransform(singleTransformIndex, radioButtonGroup);};
            deleteTransformButton.clicked += () => {ThMethods.CreateAllSingleTransforms(radioButtonGroup.value, radioButtonGroup, radioButtonFoldout, go) ;};

            renameTransformButton.clicked += () => {ThMethods.RenameSingleTransformInputWindow(singleTransformIndex, radioButtonGroup, this);};
            
            absTransformButton.clicked += () => {ThMethods.MakeTransformAbsolute(singleTransformIndex, ThMethods.GetTransformGroupNames(ThMono.TransformListContainer), radioButtonGroup);};
            absTransformButton.clicked += () => {ThMethods.DrawGUI(go, radioButtonGroup);};
            
            position.RegisterValueChangedCallback(evt => {ThMethods.UpdateTransform(evt.newValue, Vector3.zero, Vector3.one, radioButtonGroup, singleTransformIndex, go);});
            rotation.RegisterValueChangedCallback(evt => {ThMethods.UpdateTransform(Vector3.zero, evt.newValue, Vector3.one, radioButtonGroup, singleTransformIndex, go);});
            scale.RegisterValueChangedCallback(evt => {ThMethods.UpdateTransform(Vector3.zero,Vector3.zero , evt.newValue, radioButtonGroup, singleTransformIndex, go);});
            
            isActiveCheckbox.RegisterValueChangedCallback(evt => {activeTransformList[singleTransformIndex].isActive = evt.newValue; 
                ThMethods.UpdateTransform(Vector3.zero,Vector3.zero , Vector3.one, radioButtonGroup, singleTransformIndex, go); });
            
            appliedToVertexCheckbox.RegisterValueChangedCallback(evt => {activeTransformList[singleTransformIndex].appliedToVertices = evt.newValue;
                if (evt.newValue) ThMethods.ApplyTransformToVertices(singleTransformIndex, radioButtonGroup, go);
                else ThMethods.RemoveTransformFromVertices(singleTransformIndex, radioButtonGroup, go);
                ThMethods.UpdateTransform(Vector3.zero,Vector3.zero , Vector3.one, radioButtonGroup, singleTransformIndex, go);});
            #endregion
            ////////// Callbacks //////////
        }
    }
}
