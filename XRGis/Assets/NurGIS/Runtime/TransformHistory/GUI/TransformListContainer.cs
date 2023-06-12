using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory.GUI
{
    public class TransformListContainer : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<TransformListContainer, UxmlTraits>
        {
        }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }

        private readonly List<string> m_groups = new List<string>() {"Default"};
        private readonly RadioButtonGroup m_radioButtonGroup;

        public TransformListContainer()
        {
            AddToClassList("radio-group");

            // ScrollView of transforms
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
            
            // RadioButton Group
            m_radioButtonGroup = new RadioButtonGroup
            {
                choices = m_groups
            };
            CreateAndRegisterCallbackAddTransform(m_radioButtonGroup);
            
            // Header Container -- this is for entire transform history groups
            VisualElement headerContainer;
            {
                headerContainer = new VisualElement
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
                
                headerContainer.Add(label);
                
                // Add new group button
                var addButton = new Button()
                {
                    text = "+",
                    style =
                    {
                        right    = 0,
                        position = Position.Relative
                    },
                };
                
                addButton.clicked    += () => { AddGroup($"New Group  {m_groups.Count}"); };
                headerContainer.Add(addButton);
            }

            
            
            scrollView.Add(headerContainer);
            scrollView.Add(m_radioButtonGroup);
        }


        private void CreateAndRegisterCallbackAddTransform(RadioButtonGroup radioButtonGroup)
        {
            // Create a callback for when a new RadioButton is added, add all the necessary UI elements
            UQueryBuilder<RadioButton> allRadioButtons = radioButtonGroup.Query<RadioButton>();
            
            allRadioButtons.ForEach(radioButton =>
            {
                radioButton.Children().First().style.paddingBottom = 10;
                
                // Rename group button
                var renameButton = new Button()
                {
                    text = "Rename",
                    style =
                    { 
                        right    = 24,
                        position = Position.Absolute
                    },
                };
                
                // Add new transform button
                var addTransformButton = new Button()
                {
                    text = "+",
                    style =
                    {
                        right    = 0,
                        position = Position.Absolute
                    },
                };
                
                var foldout = radioButton.Q<Foldout>() ?? new Foldout
                {
                    text = "Transforms"
                };
                
                radioButton.Children().First().Add(renameButton);
                radioButton.Children().First().Add(addTransformButton);
                
                radioButton.Add(foldout);
                
                addTransformButton.clicked += () => {AddTransformGroup(foldout);};
                renameButton.clicked += RenameRadioButton;
            });
        }

        private void AddGroup(string groupName)
        {
            m_groups.Add(groupName);
            m_radioButtonGroup.choices = m_groups;
            CreateAndRegisterCallbackAddTransform(m_radioButtonGroup);
        }

        private void AddTransformGroup(Foldout radioButtonFoldout)
        {
            var transformName = "Transform:";
            radioButtonFoldout.Add(new SingleTransform(transformName));
        }

        private void RenameRadioButton()
        {
            throw new System.NotImplementedException();
        }
    }
}