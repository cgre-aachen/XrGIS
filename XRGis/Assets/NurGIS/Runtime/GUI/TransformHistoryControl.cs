using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.GUI
{
    public class TransformHistoryControl : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<TransformHistoryControl, UxmlTraits>
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
                var item = ve as TransformHistoryControl;
            }
        }

        private List<string>     _groups = new List<string>() { "Default" };
        private RadioButtonGroup _radialGroup;

        public TransformHistoryControl()
        {
            AddToClassList("radial-group");
            var scrollView = new ScrollView
            {
                style =
                {
                    height = 200
                }
            };
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            

            Add(scrollView);
            var header = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            scrollView.Add(header);

            // in petrol
            var label = new Label("<color=#00FFFF>Transform history</color>")
            {
                style =
                {
                    flexGrow = 1
                }
            };
            header.Add(label);

            var addButton = new Button()
            {
                text = "+",
                style =
                {
                    right    = 0,
                    position = Position.Absolute
                },
            };

            var renameButton = new Button()
            {
                text = "Rename",
                style =
                {
                    right    = 0,
                    position = Position.Absolute
                },
            };
            
            addButton.clicked    += () => { AddGroup($"New group  { _groups.Count }"); };
            renameButton.clicked += () => { RenameRadioButton(); };

            header.Add(addButton);
            // header.Add(renameButton);

            _radialGroup = new RadioButtonGroup();
            scrollView.Add(_radialGroup);
            _radialGroup.choices = _groups;
            CreateAndRegisterCallbackAddTransform(_radialGroup);
        }


        private void CreateAndRegisterCallbackAddTransform(RadioButtonGroup radialGroup)
        {
            UQueryBuilder<RadioButton> allRadioButton = radialGroup.Query<RadioButton>();
            allRadioButton.ForEach(radioButton =>
            {
                var addTransformButton = new Button()
                {
                    text = "+",
                    style =
                    {
                        right    = 0,
                        position = Position.Absolute
                    },
                };

                addTransformButton.clicked += () => { AddTransform(radioButton);};
                radioButton.Children().First().Add(addTransformButton);
            });
        }

        private void AddGroup(string groupName)
        {
            _groups.Add(groupName);

            _radialGroup.choices = _groups;
            CreateAndRegisterCallbackAddTransform(_radialGroup);
        }

        private void AddTransform(RadioButton radioButton)
        {
            // TODO: These transforms should be stored somewhere? 
            var transformName = $"Transform: {radioButton.label} - {radioButton.label}"; 
            radioButton.Add(new TransformControl(radioButton.label));
        }
        
        private void RenameRadioButton()
        {
           throw new System.NotImplementedException();
        }
    }
}