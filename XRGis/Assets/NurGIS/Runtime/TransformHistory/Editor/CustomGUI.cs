using System;
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
            
            var saveTransformButton = root.Q<Button>("applyTransformButton");
            var resetButton = root.Q<Button>("resetInputButton");
            var setAbsoluteTransformButton = root.Q<Button>("saveAbsolutePositionButton");
            var resetAllTransformsButton = root.Q<Button>("resetTransformButton");
            var debugButton = root.Q<Button>("debug");
            
            var translationInput = root.Q<Vector3Field>("translationInput");
            var rotationInput = root.Q<Vector3Field>("rotationInput");
            var scaleInput = root.Q<Vector3Field>("scaleInput");
            
            translationInput.value = Vector3.zero;
            rotationInput.value = Vector3.zero;
            scaleInput.value = Vector3.one;

            var slider = root.Q<SliderInt>("slider");
            slider.lowValue = 0;
            
            if (helper.transformList.Count > 0)
            {
                slider.highValue = helper.transformList.Count - 1;
                slider.value = helper.transformList.Count - 1;
            }
            else
            {
                slider.highValue = 0;
                slider.value = 0;
            }

            var uxmlFoldout = root.Q<Foldout>("transformListFoldout");
            #endregion
            
            #region Actions
            Action saveTransformAction = () =>
            {
                helper.SaveRelativeTransformFromGui(translationInput.value, rotationInput.value, scaleInput.value);
                if (!helper.noEntry)
                {
                    var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                    var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);

                    if (helper.applyToVertices)
                    {
                        helper.ApplyTransformToVertices(transformList[0], transformList[1], transformList[2]);
                    }
                    else if (!helper.applyToVertices)
                    {
                        helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                    }
                    
                    helper.SetTransformName();
                    
                    slider.highValue = helper.transformList.Count - 1;
                    slider.value = helper.transformList.Count - 1;
                    
                    translationInput.value = Vector3.zero;
                    rotationInput.value = Vector3.zero;
                    scaleInput.value = Vector3.one;
                    
                    helper.positionInput = Vector3.zero;
                    helper.rotationInput = Vector3.zero;
                    helper.scaleInput = Vector3.one;
                }
            };
            
            Action resetInputAction = () =>
            {
                translationInput.value = Vector3.zero;
                rotationInput.value = Vector3.zero;
                scaleInput.value = Vector3.one;
            };
            
            Action saveAbsoluteTransformAction = () =>
            {
                int lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                var transformList = helper.CalculateTransform(lastAbsoluteTransform, helper.transformList.Count - 1);
                helper.SaveAbsoluteTransform(transformList[0], transformList[1], transformList[2]);
                helper.SetTransformName();
                helper.DeactivateTransforms(lastAbsoluteTransform);
                slider.highValue = helper.transformList.Count - 1;
                slider.value = helper.transformList.Count - 1;
            };
            
            Action resetAllTransformsAction = () =>
            {
                helper.ResetTransforms();
                slider.highValue = helper.transformList.Count -1;
                slider.value = helper.transformList.Count -1;
            };
            
            Action updateListEntryAction = () =>
            {
                if (helper.noEntry)
                {
                    return;
                }

                uxmlFoldout.Clear();
                var listRoot = new VisualElement();
                listRoot.AddToClassList("listEntryBackground");
                listRoot.style.paddingBottom = 5;
                listRoot.style.paddingTop = 5;
                listRoot.style.paddingLeft = 5;
                listRoot.style.paddingRight = 5;
                uxmlFoldout.Add(listRoot);
                
                for (int i = 0; i < helper.transformList.Count; i++)
                {
                    var transform = helper.transformList[i];
                    var index = i;
                    var lastAbsoluteTransformIndex = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                    
                    var listEntry = new VisualElement();
                    listEntry.AddToClassList("listEntry");
                    listEntry.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    listEntry.style.backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray);
                    
                    var label = new Label(helper.transformNameList[i]);
                    label.AddToClassList("listEntryLabel");
                    label.style.color = new StyleColor(Color.white);
                    
                    // Add a toggle next to the label
                    var toggle = new Toggle();
                    {
                        toggle.value = transform.IsActive;
                        toggle.RegisterValueChangedCallback(evt =>
                        {
                            if (index < lastAbsoluteTransformIndex && slider.value >= lastAbsoluteTransformIndex)
                            {
                                toggle.value = false;
                                return;
                            }

                            var oldValue = transform.IsActive;
                            if (oldValue != evt.newValue)
                            {
                                transform.IsActive = evt.newValue;
                                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                                var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);
                                helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                            }
                        });
                    }
                    toggle.style.paddingRight = 5;
                    toggle.AddToClassList("listEntryToggle");
                    toggle.value = helper.transformList[i].IsActive;
                    
                    listEntry.Add(toggle);
                    listEntry.Add(label);
                    listRoot.Add(listEntry);
                }
            };
            
            Action debugAction = () =>
            {
                //Debug.Log(helper.positionInput);
            };
            
            #endregion

            #region Events 
            
            slider.RegisterValueChangedCallback(evt =>
            {
                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(evt.newValue, onlyActiveTransforms:false);
                var nextActiveTransform = helper.FindNextAbsoluteTransformIndex(evt.newValue, onlyActiveTransforms:false);
                helper.SetActiveNotActiveOfTransforms(lastActiveTransform, nextActiveTransform, evt.newValue);
                var transformList = helper.CalculateTransform(lastActiveTransform, evt.newValue);
                helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                updateListEntryAction();
            });
            
            
            translationInput.RegisterValueChangedCallback(evt =>
            {
                if (helper.transformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);
                Vector3 translation = transformList[0];
                helper.UpdateGameObjectTranslation(evt.newValue, translation);
            });
            
            
            rotationInput.RegisterValueChangedCallback(evt =>
            {
                if (helper.transformList.Count == 0)
                {
                    return;
                }

                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);
                Vector3 rotation = transformList[1];
                helper.UpdateGameObjectRotation(evt.newValue, Quaternion.Euler(rotation));
            });
            
            scaleInput.RegisterValueChangedCallback(evt =>
            {
                if (helper.transformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(helper.transformList.Count - 1, onlyActiveTransforms:true);
                var transformList = helper.CalculateTransform(lastActiveTransform, helper.transformList.Count - 1);
                Vector3 scale = transformList[2];
                helper.UpdateGameObjectScale(evt.newValue, scale);
            });
            
            
            
            
            saveTransformButton.clicked += saveTransformAction;
            saveTransformButton.clicked += updateListEntryAction;
            
            resetButton.clicked += resetInputAction;
            
            setAbsoluteTransformButton.clicked += saveAbsoluteTransformAction;
            setAbsoluteTransformButton.clicked += updateListEntryAction;
            
            resetAllTransformsButton.clicked += resetAllTransformsAction;
            resetAllTransformsButton.clicked += updateListEntryAction;

            debugButton.clicked += debugAction;
            #endregion

            resetInputAction();
            updateListEntryAction();
            return root;
        }
    }
}
