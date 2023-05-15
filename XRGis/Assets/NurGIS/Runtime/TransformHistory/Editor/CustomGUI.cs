using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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
                    var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1);
                    var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);
                    
                    if (helper.applyToVertices)
                    {
                        helper.ApplyTransformToVertices(transformList);
                    }
                    else if (!helper.applyToVertices)
                    {
                        helper.ApplyTransformToGo(transformList);
                    }
                    
                    helper.SetTransformName();
                    uxmlSlider.highValue = helper.transformList.Count - 1;
                    uxmlSlider.value = helper.transformList.Count - 1;
                }
            };
            
            Action resetTransformAction = () =>
            {
                helper.ResetTransforms();
                uxmlSlider.highValue = helper.transformList.Count -1;
                uxmlSlider.value = helper.transformList.Count -1;
            };
            
            Action savePositionAction = () =>
            {
                int lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1);
                var transformList = helper.CalculateTransform(lastAbsoluteTransform, helper.transformList.Count - 1);
                helper.SaveAbsoluteTransform(transformList);
                helper.SetTransformName();
                helper.DeactivateTransforms(lastAbsoluteTransform);
                uxmlSlider.highValue = helper.transformList.Count - 1;
                uxmlSlider.value = helper.transformList.Count - 1;
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
                
                for (int i = 0; i < helper.transformList.Count; i++)
                {
                    var transform = helper.transformList[i];
                    var index = i;
                    var lastAbsoluteTransformIndex = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1);
                    
                    var listEntry = new VisualElement();
                    listEntry.AddToClassList("listEntry");
                    listEntry.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    listEntry.style.backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray);
                    
                    var label = new Label(helper.transformNameList[i]);
                    label.AddToClassList("listEntryLabel");
                    label.style.color = new StyleColor(Color.white);
                    
                    // Add a toggle next to the label
                    var toggle = new Toggle();
                    { // Binding
                        toggle.value = transform.IsActive;
                        toggle.RegisterValueChangedCallback(evt =>
                        {
                            if (index < lastAbsoluteTransformIndex && uxmlSlider.value >= lastAbsoluteTransformIndex)
                            {
                                toggle.value = false;
                                return;
                            }

                            var oldValue = transform.IsActive;
                            if (oldValue != evt.newValue)
                            {
                                transform.IsActive = evt.newValue;
                                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1);
                                var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);
                                helper.ApplyTransformToGo(transformList);
                            }
                        });
                    }
                    toggle.style.paddingRight = 5;
                    toggle.AddToClassList("listEntryToggle");
                    toggle.value = helper.transformList[i].IsActive;
                    
                    listEntry.Add(toggle);
                    listEntry.Add(label);
                    uxmlListEntryBackground.Add(listEntry);
                }
            };
            #endregion

            #region Events 
            uxmlSlider.RegisterValueChangedCallback(evt =>
            {
                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(evt.newValue);
                var nextActiveTransform = helper.FindNextAbsoluteTransformIndex(evt.newValue);
                helper.SetActiveNotActiveOfTransforms(lastActiveTransform, nextActiveTransform, evt.newValue);
                var updatedTransform = helper.CalculateTransform(lastActiveTransform, evt.newValue);
                helper.ApplyTransformToGo(updatedTransform);
                updateListEntry();
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
