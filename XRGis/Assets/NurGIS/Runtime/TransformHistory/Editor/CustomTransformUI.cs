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
            var deleteTransformButton = root.Q<Button>("deleteTransformButton");
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

            var listRoot = new VisualElement();

            if (helper.transformListContainer.Count > 0) // Sets values for the slider and the radiobutton group whenever the GUI is loaded
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
                activeTransformList[0].isActive = true;
                
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
                
                listRoot.Clear();
                uxmlFoldout.Add(listRoot);

                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                for (int i = 0; i < activeTransformList.Count; i++)
                {
                    var selectedTransform = activeTransformList[i];
                    var index = i;
                    
                    var listEntry = new VisualElement
                    {
                        style =
                        {
                            flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                            backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray)
                        }
                    }; // Create a new list entry
                    
                    listEntry.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        if (evt.button == 0) // Left click on list, highlights the entry and sets the active index
                        {
                            helper.activeTransformListIndex.Add(index);
                            helper.HighlightListEntry(helper.activeTransformListIndex, listRoot);
                        }

                        if (evt.button == 0 && Input.GetKeyDown(KeyCode.LeftShift)) // Left click + shift, highlights multiple entries and adds entries
                        {
                            
                        }
                    });
                    
                    var textField = new TextField // Add a label for the transform name
                    {
                        value = selectedTransform.transformName
                    };    

                    textField.RegisterValueChangedCallback(evt => // Callback logic when a label is changed
                    {
                        selectedTransform.transformName = evt.newValue;
                    });
                    
                    var indexLabel = new Label("#" + i); // Add a readonly integer next to the label
                    var toggle = new Toggle(); // Add a toggle next to the label
                    {
                        toggle.value = selectedTransform.isActive;
                        toggle.RegisterValueChangedCallback(evt => // Callback logic when a toggle is selected
                        {
                            selectedTransform.isActive = evt.newValue;
                            var nextAbsoluteTransform = helper.FindNextAbsoluteTransformIndex(index, onlyActiveTransforms:true, activeTransformList);
                            var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms:true, activeTransformList);
                            
                            if (lastAbsoluteTransform == 0  &&
                                index < nextAbsoluteTransform && 
                                selectedTransform.transformType != TransformChangeHelper.TransformTypes.Absolute &&
                                activeTransformList[nextAbsoluteTransform].transformType == TransformChangeHelper.TransformTypes.Absolute &&
                                activeTransformList[nextAbsoluteTransform].isActive)
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
                        toggle.SetValueWithoutNotify(selectedTransform.isActive);
                    }).Every(100);
                    
                    toggle.style.paddingRight = 5;
                    
                    listEntry.Add(indexLabel);
                    listEntry.Add(toggle);
                    listEntry.Add(textField);
                    listRoot.Add(listEntry);
                }
            };
            
            Action deleteTransformAction = () =>
            {
                if (transformListRadioButtonGroup.value == -1 || 
                    helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList.Count == 0)
                {
                    return;
                }
                
                if (helper.activeTransformListIndex.Count < 1)
                {
                    Debug.Log("No transform selected");
                    return;
                }
                
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                activeTransformList.RemoveAt(helper.activeTransformListIndex);
                
                var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = helper.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                
                slider.highValue -= 1;
                helper.activeTransformListIndex = -1;
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
            };
            #endregion

            #region Events
            slider.RegisterValueChangedCallback(evt => // Changes in Slider position
            {
                var activeTransformList = helper.transformListContainer[transformListRadioButtonGroup.value].singleTransformList;
                
                if(activeTransformList[evt.newValue].transformType != TransformChangeHelper.TransformTypes.Absolute)
                {
                    var lastAbsoluteTransform = helper.FindLastAbsoluteTransformIndex(evt.newValue, onlyActiveTransforms:true, activeTransformList);
                    var transformList = helper.CalculateSliderTransform(lastAbsoluteTransform, evt.newValue, activeTransformList);
                    helper.ApplyTransformToGo(transformList[0], transformList[1], transformList[2]);
                }

                if (activeTransformList[evt.newValue].transformType == TransformChangeHelper.TransformTypes.Absolute && activeTransformList[evt.newValue].isActive) // In case we are setting the slider to an absolute transform
                {
                    helper.ApplyTransformToGo(activeTransformList[evt.newValue].position, activeTransformList[evt.newValue].rotation, activeTransformList[evt.newValue].scale);
                }
            });
            
            transformListRadioButtonGroup.RegisterValueChangedCallback(evt => // Changes of the RadioButtonGroup for selecting different transform LoLs
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
                helper.activeTransformListIndex = -1;
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
            
            deleteTransformButton.clicked += deleteTransformAction;
            deleteTransformButton.clicked += updateTransformListAction;

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
