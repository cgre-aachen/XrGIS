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

            // * Set up scrollView
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

            // * Header
            VisualElement header;
            {
                header = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row
                    }
                };

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

                addButton.clicked    += () => { AddGroup($"New group  {_groups.Count}"); };
                renameButton.clicked += RenameRadioButton;

                header.Add(addButton);
            }

            // * RadialGroup
            {
                _radialGroup = new RadioButtonGroup
                {
                    choices = _groups
                };
                CreateAndRegisterCallbackAddTransform(_radialGroup);
            } 
            
            scrollView.Add(_radialGroup);
            scrollView.Add(header);
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

                Foldout foldout = radioButton.Q<Foldout>("transforms foldout") ?? new Foldout
                {
                    name = "transforms foldout",
                    text = "Foldout"
                };

                addTransformButton.clicked += () => { AddTransform(foldout); };

                radioButton.Add(foldout);
                radioButton.Children().First().Add(addTransformButton);
            });
        }

        private void AddGroup(string groupName)
        {
            _groups.Add(groupName);
            _radialGroup.choices = _groups;
            CreateAndRegisterCallbackAddTransform(_radialGroup);
        }

        private void AddTransform(Foldout radioButtonFoldout)
        {
            // TODO: These transforms should be stored somewhere? 
            var transformName = $"Transform:"; //{radioButton.label} - {radioButton.label}"; 
            radioButtonFoldout.Add(new TransformControl(transformName));
        }

        private void RenameRadioButton()
        {
            throw new System.NotImplementedException();
        }
    }
}