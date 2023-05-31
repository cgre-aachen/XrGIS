using System;
using System.Collections.Generic;
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
            
            var transformListRadioButtonGroup = root.Q<RadioButtonGroup>("transformListSelection");
            transformListRadioButtonGroup.value = 0;
            
            var copyTransformListButton = root.Q<Button>("copyTransformList");
            var createNewEmptyListButton = root.Q<Button>("createNewEmptyList");
            var deleteListButton = root.Q<Button>("deleteListButton");
            
            translationInput.value = Vector3.zero;
            rotationInput.value = Vector3.zero;
            scaleInput.value = Vector3.one;

            var slider = root.Q<SliderInt>("slider");
            slider.lowValue = 0;
            
            
            if (helper.transformListContainer.Count > 0)
            {
                var activeDefaultTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
            
                if (activeDefaultTransformList.Count > 0)
                {
                    slider.highValue = activeDefaultTransformList.Count - 1;
                    slider.value = activeDefaultTransformList.Count - 1;
                }
                else
                {
                    slider.highValue = 0;
                    slider.value = 0;
                }
            }
            
            var uxmlFoldout = root.Q<Foldout>("transformListFoldout");
            #endregion
            
            #region Actions
            Action saveTransformAction = () =>
            {
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                
                helper.SaveRelativeTransformFromGui(translationInput.value, rotationInput.value, scaleInput.value, activeTransformList);
                if (!helper.noEntry)
                {
                    var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                    var transformList = helper.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);

                    if (helper.applyToVertices)
                    {
                        helper.ApplyTransformToVertices(transformList[0], transformList[1], transformList[2]);
                    }
                    else if (!helper.applyToVertices)
                    {
                        helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                    }
                    
                    slider.highValue = activeTransformList.Count - 1;
                    slider.value = activeTransformList.Count - 1;
                    
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
            
            Action copyTransformListButtonAction = () =>
            {
                int selectedTransformListIndex = transformListRadioButtonGroup.value;
                helper.CopyTransformListEntry(selectedTransformListIndex);
            };
            
            Action createNewEmptyListButtonAction = () =>
            {
                helper.CreateNewTransformListEntry(false);
            };
            
            Action saveAbsoluteTransformAction = () =>
            {
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                int lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                helper.SaveAbsoluteTransform(transformList[0], transformList[1], transformList[2], activeTransformList);
                helper.DeactivateTransforms(lastAbsoluteTransform, activeTransformList);
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
            };
            
            Action resetAllTransformsAction = () =>
            {
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                activeTransformList.RemoveRange(1, activeTransformList.Count - 1);
                activeTransformList[0].IsActive = true;
                
                var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                
                slider.highValue = activeTransformList.Count -1;
                slider.value = activeTransformList.Count -1;
            };

            Action updateRadioButtonDisplay = () =>
            {
                var transformListNames = new List<string>(); // Iterate over helper all entries in the transform list container and create a new radio button for each entry
                
                foreach (TransformChangeHelper.CustomTransformContainer transformList in helper.transformListContainer)
                {
                    transformListNames.Add(transformList.transformListName); 
                }
                transformListRadioButtonGroup.choices = transformListNames;
            };

            Action updateTransformListAction = () =>
            {
                if (helper.noEntry || transformListRadioButtonGroup.value == -1 || helper.transformListContainer.Count == 0)
                {
                    return;
                }

                uxmlFoldout.Clear();
                var listRoot = new VisualElement();
                listRoot.style.paddingBottom = 5;
                listRoot.style.paddingTop = 5;
                listRoot.style.paddingLeft = 5;
                listRoot.style.paddingRight = 5;
                uxmlFoldout.Add(listRoot);

                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                for (int i = 0; i < activeTransformList.Count; i++)
                {
                    var selectedTransform = activeTransformList[i];
                    var index = i;
                    
                    var listEntry = new VisualElement(); // Create a new list entry
                    listEntry.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    listEntry.style.backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray);

                    var label = new Label(selectedTransform.transformName); // Add a label for the transform name   
                    label.style.color = selectedTransform.IsActive == false ? new StyleColor(Color.gray) : new StyleColor(Color.white);
                    
                    if (selectedTransform.transformType == TransformChangeHelper.TransformTypes.Absolute)
                    {
                        label.style.color = new StyleColor(Color.red);
                    }
                    
                    var indexLabel = new Label("#" + i); // Add a readonly integer next to the label
                    indexLabel.style.color = selectedTransform.IsActive == false ? new StyleColor(Color.gray) : new StyleColor(Color.white);
                    
                    var toggle = new Toggle(); // Add a toggle next to the label
                    {
                        toggle.value = selectedTransform.IsActive;
                        toggle.RegisterValueChangedCallback(evt => // Callback logic when a toggle is selected
                        {
                            selectedTransform.IsActive = evt.newValue;

                            label.style.color = selectedTransform.IsActive == false ? new StyleColor(Color.gray) : new StyleColor(Color.white);
                            if (selectedTransform.transformType == TransformChangeHelper.TransformTypes.Absolute)
                            {
                                label.style.color = new StyleColor(Color.red);
                            }
                            indexLabel.style.color = selectedTransform.IsActive == false ? new StyleColor(Color.gray) : new StyleColor(Color.white);

                            var nextAbsoluteTransform = helper.FindNextAbsoluteTransformIndex(index, onlyActiveTransforms:true, activeTransformList);
                            var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms:true, activeTransformList);
                            
                            if (lastAbsoluteTransform == 0  &&
                                index < nextAbsoluteTransform && 
                                selectedTransform.transformType != TransformChangeHelper.TransformTypes.Absolute &&
                                activeTransformList[nextAbsoluteTransform].transformType == TransformChangeHelper.TransformTypes.Absolute &&
                                activeTransformList[nextAbsoluteTransform].IsActive)
                            {
                                toggle.value = false;
                                return;
                            }
                            
                            if (selectedTransform.transformType == TransformChangeHelper.TransformTypes.Absolute)
                            {
                                lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms:false, activeTransformList);
                                helper.ToggleAbsoluteTransform(index, lastAbsoluteTransform, selectedTransform, activeTransformList);
                            }
                            
                            var transformList = helper.CalculateTransform(lastAbsoluteTransform, nextAbsoluteTransform, activeTransformList);
                            helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                        });
                    }
                    
                    toggle.schedule.Execute(() => // Update the toggle value every 100ms
                    {
                        toggle.SetValueWithoutNotify(selectedTransform.IsActive);
                    }).Every(100);
                    
                    toggle.style.paddingRight = 5;

                    listEntry.Add(indexLabel);
                    listEntry.Add(toggle);
                    listEntry.Add(label);
                    listRoot.Add(listEntry);
                }
            };
            
            Action deleteListAction = () =>
            {
                if (transformListRadioButtonGroup.value == -1 || 
                    helper.transformListContainer.Count is 0 or 1)
                {
                    return;
                }
                
                helper.transformListContainer.RemoveAt(transformListRadioButtonGroup.value);
                transformListRadioButtonGroup.value -= 1;
                updateRadioButtonDisplay();
                updateTransformListAction();
            };
            
            Action debugAction = () =>
            {
                #if true
                
                #endif
            };
            #endregion

            #region Events
            slider.RegisterValueChangedCallback(evt =>
            {
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                
                if(activeTransformList[evt.newValue].transformType != TransformChangeHelper.TransformTypes.Absolute)
                {
                    var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(evt.newValue, onlyActiveTransforms:true, activeTransformList);
                    var transformList = helper.CalculateSliderTransform(lastAbsoluteTransform, evt.newValue, activeTransformList);
                    helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                }

                if (activeTransformList[evt.newValue].transformType == TransformChangeHelper.TransformTypes.Absolute && activeTransformList[evt.newValue].IsActive) // In case we are setting the slider to an absolute transform
                {
                    helper.ApplyTransformToGo(activeTransformList[evt.newValue].position, activeTransformList[evt.newValue].rotation, activeTransformList[evt.newValue].scale);
                }
            });
            
            transformListRadioButtonGroup.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == -1)
                {
                    transformListRadioButtonGroup.value = evt.previousValue;    
                }
                
                updateTransformListAction();
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
                
                helper.activeRadioButton = evt.newValue;
            });

            translationInput.RegisterValueChangedCallback(evt =>
            {
                if (helper.transformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                Vector3 translation = transformList[0];
                helper.UpdateGameObjectTranslation(evt.newValue, translation);
            });
            
            rotationInput.RegisterValueChangedCallback(evt =>
            {
                if (helper.transformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }

                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                helper.UpdateGameObjectRotation(evt.newValue, transformList[1]);
            });
            
            scaleInput.RegisterValueChangedCallback(evt =>
            {
                if (helper.transformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                Vector3 scale = transformList[2];
                helper.UpdateGameObjectScale(evt.newValue, scale);
            });
            
            saveTransformButton.clicked += saveTransformAction;
            saveTransformButton.clicked += updateTransformListAction;
            
            resetButton.clicked += resetInputAction;

            copyTransformListButton.clicked += copyTransformListButtonAction;
            copyTransformListButton.clicked += updateRadioButtonDisplay;
            
            createNewEmptyListButton.clicked += createNewEmptyListButtonAction;
            createNewEmptyListButton.clicked += updateRadioButtonDisplay;
            
            setAbsoluteTransformButton.clicked += saveAbsoluteTransformAction;
            setAbsoluteTransformButton.clicked += updateTransformListAction;
            
            resetAllTransformsButton.clicked += resetAllTransformsAction;
            resetAllTransformsButton.clicked += updateTransformListAction;
            
            deleteListButton.clicked += deleteListAction;

            debugButton.clicked += debugAction;
            #endregion
            
            resetInputAction();
            updateTransformListAction();
            
            updateRadioButtonDisplay();
            transformListRadioButtonGroup.value = 0;
            return root;
        }
    }
}
