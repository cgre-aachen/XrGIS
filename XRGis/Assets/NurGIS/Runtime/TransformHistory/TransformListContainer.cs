using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public class TransformListContainer : VisualElement
    {
        private readonly List<string> _mGroups = TransformGuiMethods.GetTransformGroupNames(TransformMonobehaviour.TransformListContainer);
        public TransformListContainer(GameObject go)
        {
            AddToClassList("radio-group");
            
            ///////////////// Scroll View /////////////////
            ScrollView scrollView;
            {
                scrollView = new ScrollView
                {
                    style =
                    {
                        height = 200
                    },
                    
                    horizontalScrollerVisibility = ScrollerVisibility.Hidden
                };

                Add(scrollView);
            }
            ///////////////// Scroll View /////////////////
            
            ///////////////// Radio Button Group /////////////////
            var radioButtonGroup = new RadioButtonGroup
            {
                choices = _mGroups
            };
            TransformGuiMethods.DrawGUI(go, radioButtonGroup);
            ///////////////// Radio Button Group /////////////////
            
            ///////////////// Header Container /////////////////
            var headerContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            // Header Label
            var label = new Label
            {
                text = "Groups",
                style =
                {
                    flexGrow = 1,
                    fontSize = 20,

                }
            };
            
            // Add new group button
            var addButton = new Button()
            {
                text = "+",
                style =
                {
                    right    = 22,
                    position = Position.Absolute
                },
            };
            
            // Delete group button
            var deleteButton = new Button()
            {
                text = "-",
                style =
                {
                    right    = 0,
                    position = Position.Absolute
                },
            };
            ///////////////// Header Container /////////////////
            
            headerContainer.Add(label);
            headerContainer.Add(addButton);
            headerContainer.Add(deleteButton);

            scrollView.Add(headerContainer);
            scrollView.Add(radioButtonGroup);
            
            ///////////////// Callbacks /////////////////
            radioButtonGroup.RegisterValueChangedCallback(_ => { TransformGuiMethods.ApplyTransform(radioButtonGroup, go); });
            addButton.clicked    += () => { TransformGuiMethods.AddTransformGroup($"New Group  {_mGroups.Count}",_mGroups, radioButtonGroup); };
            addButton.clicked    += () => { TransformGuiMethods.DrawGUI(go, radioButtonGroup); };
            
            deleteButton.clicked += () => { TransformGuiMethods.DeleteTransformGroup(_mGroups, radioButtonGroup); };
            deleteButton.clicked += () => { TransformGuiMethods.DrawGUI(go, radioButtonGroup); };
            ///////////////// Callbacks /////////////////
        }
    }
}