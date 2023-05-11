using System;
using UnityEditor;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace NurGIS.Runtime.TransformHistory.Editor
{
    [CustomEditor(typeof(TransformChangeHelper))]
    public class CustomTransformUI : UnityEditor.Editor
    {
        public VisualTreeAsset mUxml;
        
        public override VisualElement CreateInspectorGUI()
        {
            #region Properties
            var root = new VisualElement();
            TransformChangeHelper helper = (TransformChangeHelper)target;
            mUxml.CloneTree(root);
            
            var uxmlApplyTransform = root.Q<Button>("ApplyTransform");
            var uxmlSavePosition = root.Q<Button>("SavePosition");
            var uxmlResetTransform = root.Q<Button>("ResetButton");
            
            var uxmlSlider = root.Q<SliderInt>("Slider");
            uxmlSlider.lowValue = 0;
            uxmlSlider.highValue = helper.transformNameList.Count;
            uxmlSlider.value = helper.transformNameList.Count;
            
            var uxmlFoldout = root.Q<Foldout>("transformListFoldout");
            #endregion
            
            #region Actions
            Action applyTransformAction = () =>
            {
                helper.SaveRelativeTransformFromGui();
                if (!helper.noEntry)
                {
                    var lastActiveTransform = helper.FindLastActiveTransformIndex(helper.transformNameList.Count - 1);
                    var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformNameList.Count - 1);
                    helper.ApplyTransformToGo(transformList);
                    helper.SetTransformName();
                    uxmlSlider.highValue = helper.transformNameList.Count - 1;
                    uxmlSlider.value = helper.transformNameList.Count - 1;
                }
            };
            
            Action resetTransformAction = () =>
            {
                helper.ResetTransforms();
                uxmlSlider.highValue = helper.transformNameList.Count -1;
                uxmlSlider.value = helper.transformNameList.Count -1;
            };
            
            Action savePositionAction = () =>
            {
                int lastActiveTransform = helper.FindLastActiveTransformIndex(helper.transformNameList.Count - 1);
                var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformNameList.Count - 1);
                helper.SaveAbsoluteTransform(transformList);
                helper.SetTransformName();
                uxmlSlider.highValue = helper.transformNameList.Count - 1;
                uxmlSlider.value = helper.transformNameList.Count - 1;
            };
            
            Action updateListEntry = () =>
            {
                uxmlFoldout.Clear();
                // create a background box
                var uxmlListEntryBackground = new VisualElement();
                uxmlListEntryBackground.AddToClassList("listEntryBackground");
                uxmlListEntryBackground.style.paddingBottom = 5;
                uxmlListEntryBackground.style.paddingTop = 5;
                uxmlListEntryBackground.style.paddingLeft = 5;
                uxmlListEntryBackground.style.paddingRight = 5;
                uxmlFoldout.Add(uxmlListEntryBackground);
                
                for (int i = 0; i < helper.transformNameList.Count; i++)
                {
                    var uxmlListEntry = new VisualElement();
                    uxmlListEntry.AddToClassList("listEntry");
                    uxmlListEntry.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    uxmlListEntry.style.backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray);
                    
                    var uxmlListEntryLabel = new Label(helper.transformNameList[i]);
                    uxmlListEntryLabel.AddToClassList("listEntryLabel");
                    uxmlListEntryLabel.style.color = new StyleColor(Color.white);
                    
                    // Add a toggle next to the label
                    var uxmlListEntryToggle = new Toggle();
                    uxmlListEntryToggle.style.paddingRight = 5;
                    uxmlListEntryToggle.AddToClassList("listEntryToggle");
                    uxmlListEntryToggle.value = helper.transformList[i].IsActive;
                    
                    uxmlListEntry.Add(uxmlListEntryToggle);
                    uxmlListEntry.Add(uxmlListEntryLabel);
                    uxmlListEntryBackground.Add(uxmlListEntry);
                }
            };
            #endregion
            
            #region Events 
            uxmlSlider.RegisterValueChangedCallback(evt =>
            {
                var lastActiveTransform = helper.FindLastActiveTransformIndex(evt.newValue);
                var transformList = helper.CalculateTransform(lastActiveTransform, evt.newValue);
                helper.ApplyTransformToGo(transformList);
            });
            
            uxmlApplyTransform.clicked += applyTransformAction;
            uxmlApplyTransform.clicked += updateListEntry;
            
            uxmlResetTransform.clicked += resetTransformAction;
            uxmlResetTransform.clicked += updateListEntry;
            
            uxmlSavePosition.clicked += savePositionAction;
            uxmlSavePosition.clicked += updateListEntry;
            #endregion
            
            return root;
        }
    }
}
