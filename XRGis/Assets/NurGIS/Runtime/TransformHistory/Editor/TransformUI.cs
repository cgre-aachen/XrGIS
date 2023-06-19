﻿using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace NurGIS.Runtime.TransformHistory.Editor
{
    #if true
    [CustomEditor(typeof(TransformMonobehaviour))]
    public class TransformUI : UnityEditor.Editor
    {
        public VisualTreeAsset mUxml;
        
        public override VisualElement CreateInspectorGUI()
        {
            #region Properties
            var root = new VisualElement();
            mUxml.CloneTree(root);
            
            // reference the game object of the selected object
            var selectedGameObject = target.GameObject();
            var radioButtonGroup = root.Q<RadioButtonGroup>("transformListSelection"); // The radiobutton group that is used to select the transform list
            radioButtonGroup.value = 0;

            var createNewEmptyListButton = root.Q<Button>("createNewEmptyList");
            
            var uxmlFoldout = root.Q<Foldout>("transformListFoldout");
            var dropdownField = root.Q<DropdownField>("dropdownField"); // The dropdown field that is used to select functionality for the transform lists
            
            var slider = root.Q<SliderInt>("slider");
            slider.lowValue = 0;
            
            var translationInput = root.Q<Vector3Field>("translationInput");
            var rotationInput = root.Q<Vector3Field>("rotationInput");
            var scaleInput = root.Q<Vector3Field>("scaleInput");
            translationInput.value = Vector3.zero;
            rotationInput.value = Vector3.zero;
            scaleInput.value = Vector3.one;
            
            var saveTransformButton = root.Q<Button>("applyTransformButton");
            var resetButton = root.Q<Button>("resetInputButton");
            var setAbsoluteTransformButton = root.Q<Button>("saveAbsolutePositionButton");
            var deleteTransformButton = root.Q<Button>("deleteTransformButton");

            var listRoot = new VisualElement();

            if (TransformMonobehaviour.TransformListContainer.Count > 0) // Sets values for the slider and the radiobutton group whenever the GUI is loaded
            {
                var activeDefaultTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
            
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
            
            #endregion
            
            #region Actions

            void SaveTransformAction()
            {
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;

                TransformGuiMethods.SaveRelativeTransformFromGui(translationInput.value, rotationInput.value, scaleInput.value, activeTransformList);
                if (TransformMonobehaviour.noEntry) return;
                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms: true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);

                switch (TransformMonobehaviour.applyToVertices)
                {
                    case true:
                        TransformCalculation.ApplyTransformToVertices(selectedGameObject, transformList[0], transformList[1], transformList[2]);
                        break;
                    case false:
                        TransformGoOperations.ApplyTransformToGo(selectedGameObject, transformList[0], transformList[1], transformList[2]);
                        break;
                }

                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;

                translationInput.value = Vector3.zero;
                rotationInput.value = Vector3.zero;
                scaleInput.value = Vector3.one;

                TransformMonobehaviour.positionInput = Vector3.zero;
                TransformMonobehaviour.rotationInput = Vector3.zero;
                TransformMonobehaviour.scaleInput = Vector3.one;
            }

            void ResetInputAction()
            {
                translationInput.value = Vector3.zero;
                rotationInput.value = Vector3.zero;
                scaleInput.value = Vector3.one;
            }

            void SaveAbsoluteTransformAction()
            {
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms: true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGuiMethods.SaveAbsoluteTransform(transformList[0], transformList[1], transformList[2], activeTransformList);
                TransformGuiMethods.DeactivateTransforms(lastAbsoluteTransform, activeTransformList);
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
            }

            void ResetAllTransformsAction()
            {
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                activeTransformList.RemoveRange(1, activeTransformList.Count - 1);
                activeTransformList[0].isActive = true;

                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms: true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.ApplyTransformToGo(selectedGameObject, transformList[0], transformList[1], transformList[2]);

                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
            }

            void UpdateRadioButtonDisplay()
            {
                var transformListNames = new List<string>(); // Iterate over helper all entries in the transform list container and create a new radio button for each entry

                foreach (TransformMonobehaviour.CustomTransformContainer transformList in TransformMonobehaviour.TransformListContainer)
                {
                    transformListNames.Add(transformList.transformListName);
                }

                radioButtonGroup.choices = transformListNames;

                if (radioButtonGroup.value == -1)
                {
                    uxmlFoldout.text = "No Transform List Selected";
                    return;
                }

                if (transformListNames.Count == 0)
                {
                    uxmlFoldout.text = "No Transform List Selected";
                    return;
                }

                uxmlFoldout.text = transformListNames[radioButtonGroup.value];
            }

            void UpdateTransformListAction()
            {
                if (TransformMonobehaviour.noEntry || radioButtonGroup.value == -1 || TransformMonobehaviour.TransformListContainer.Count == 0)
                {
                    return;
                }

                listRoot.Clear();
                uxmlFoldout.Add(listRoot);

                var transformContentPanelContainer = new VisualElement();

                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                for (var i = 0; i < activeTransformList.Count; i++)
                {
                    var selectedTransform = activeTransformList[i];
                    var index = i;

                    var listEntry = new VisualElement { style = { flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row), backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray) } }; // Create a new list entry

                    listEntry.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        switch (evt.button)
                        {
                            case 0: // Left click on list, highlights the entry and sets the active index
                                TransformMonobehaviour.selectedTransformListIndex.Clear();
                                TransformMonobehaviour.selectedTransformListIndex.Add(index);
                                TransformGuiMethods.HighlightListEntry(TransformMonobehaviour.selectedTransformListIndex, listRoot);
                                var singleTransform = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList[index];
                                TransformGuiMethods.CreatePopUp(transformContentPanelContainer, singleTransform.position, singleTransform.rotation, singleTransform.scale);
                                break;

                            case 1: // Right click, highlights multiple entries and adds entries
                                if (!TransformMonobehaviour.selectedTransformListIndex.Contains(index))
                                {
                                    TransformMonobehaviour.selectedTransformListIndex.Add(index);
                                    TransformMonobehaviour.selectedTransformListIndex.Sort();
                                }

                                TransformGuiMethods.HighlightListEntry(TransformMonobehaviour.selectedTransformListIndex, listRoot);
                                break;
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
                            var nextAbsoluteTransform = TransformCalculation.FindNextAbsoluteTransformIndex(index, onlyActiveTransforms: true, activeTransformList);
                            var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms: true, activeTransformList);

                            if (lastAbsoluteTransform == 0 && index < nextAbsoluteTransform && selectedTransform.transformType != TransformMonobehaviour.TransformTypes.Absolute && activeTransformList[nextAbsoluteTransform].transformType == TransformMonobehaviour.TransformTypes.Absolute && activeTransformList[nextAbsoluteTransform].isActive)
                            {
                                toggle.value = false;
                                return;
                            }

                            if (selectedTransform.transformType == TransformMonobehaviour.TransformTypes.Absolute)
                            {
                                lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms: false, activeTransformList);
                                TransformGuiMethods.ToggleAbsoluteTransform(index, lastAbsoluteTransform, selectedTransform, activeTransformList);
                            }

                            var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, nextAbsoluteTransform, activeTransformList);
                            TransformGoOperations.ApplyTransformToGo(selectedGameObject, transformList[0], transformList[1], transformList[2]);
                        });
                    }

                    toggle.schedule.Execute(() => // Update the toggle value every 100ms
                        {
                            toggle.SetValueWithoutNotify(selectedTransform.isActive);
                        })
                        .Every(100);

                    toggle.style.paddingRight = 5;

                    listEntry.Add(indexLabel);
                    listEntry.Add(toggle);
                    listEntry.Add(textField);
                    listRoot.Add(listEntry);
                }

                listRoot.Add(transformContentPanelContainer);
            }

            void DeleteTransformAction()
            {
                if (radioButtonGroup.value == -1 || TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList.Count == 0)
                {
                    return;
                }

                if (TransformMonobehaviour.selectedTransformListIndex.Count < 1)
                {
                    Debug.Log("No transform selected");
                    return;
                }

                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;

                for (var i = TransformMonobehaviour.selectedTransformListIndex.Count - 1; i >= 0; i--)
                {
                    activeTransformList.RemoveAt(TransformMonobehaviour.selectedTransformListIndex[i]);
                }

                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms: true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.ApplyTransformToGo(selectedGameObject, transformList[0], transformList[1], transformList[2]);

                slider.highValue = activeTransformList.Count - 1;
                TransformMonobehaviour.selectedTransformListIndex.Clear();
            }

            void DeleteListAction()
            {
                if (radioButtonGroup.value == -1 || TransformMonobehaviour.TransformListContainer.Count is 0 or 1)
                {
                    return;
                }

                TransformMonobehaviour.TransformListContainer.RemoveAt(radioButtonGroup.value);
                radioButtonGroup.value -= 1;
            }

            void DropdownFieldAction()
            {
                var dropdownFieldOptions = new List<String> { "Copy List", "Delete List", "Reset Transforms" };

                dropdownField.choices = dropdownFieldOptions;
                dropdownField.value = null;
            }

            #endregion

            #region Callback Events
            slider.RegisterValueChangedCallback(evt => // Changes in Slider position
            {
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                
                if(activeTransformList[evt.newValue].transformType != TransformMonobehaviour.TransformTypes.Absolute)
                {
                    var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(evt.newValue, onlyActiveTransforms:true, activeTransformList);
                    var transformList = TransformCalculation.CalculateSliderTransform(lastAbsoluteTransform, evt.newValue, activeTransformList);
                    TransformGoOperations.ApplyTransformToGo(selectedGameObject, transformList[0], transformList[1], transformList[2]);
                }

                if (activeTransformList[evt.newValue].transformType == TransformMonobehaviour.TransformTypes.Absolute && activeTransformList[evt.newValue].isActive) // In case we are setting the slider to an absolute transform
                {
                    TransformGoOperations.ApplyTransformToGo(selectedGameObject, activeTransformList[evt.newValue].position, activeTransformList[evt.newValue].rotation, activeTransformList[evt.newValue].scale);
                }
            });
            
            radioButtonGroup.RegisterValueChangedCallback(evt => // Changes of the RadioButtonGroup for selecting different transform LoLs
            {
                if (evt.newValue == -1)
                {
                    radioButtonGroup.value = evt.previousValue;    
                }
                
                UpdateTransformListAction();
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.ApplyTransformToGo(selectedGameObject, transformList[0], transformList[1], transformList[2]);
                
                uxmlFoldout.text = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].transformListName;
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
                
                TransformMonobehaviour.activeRadioButton = evt.newValue;
                TransformMonobehaviour.selectedTransformListIndex.Clear();
            });

            dropdownField.RegisterValueChangedCallback(evt => // DropdownField for Copying, Deleting and Resetting Transform Lists
            {
                switch (evt.newValue)
                {
                    case "Copy List":
                        TransformGuiMethods.CopyTransformListEntry(radioButtonGroup.value);
                        UpdateRadioButtonDisplay();
                        break;
                    case "Delete List":
                        DeleteListAction();
                        UpdateRadioButtonDisplay();  
                        UpdateTransformListAction();
                        break;
                    case "Reset Transforms":
                        ResetAllTransformsAction();
                        UpdateTransformListAction();
                        break;
                }
                dropdownField.value = null;
            });

            translationInput.RegisterValueChangedCallback(evt =>
            {
                if (TransformMonobehaviour.TransformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                var translationVector3 = transformList[0];
                TransformGoOperations.UpdateGameObjectTranslation(selectedGameObject, evt.newValue, translationVector3);
            });
            
            rotationInput.RegisterValueChangedCallback(evt =>
            {
                if (TransformMonobehaviour.TransformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }

                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.UpdateGameObjectRotation(selectedGameObject, evt.newValue, transformList[1]);
            });
            
            scaleInput.RegisterValueChangedCallback(evt =>
            {
                if (TransformMonobehaviour.TransformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                var lastScaleVector3 = transformList[2];
                TransformGoOperations.UpdateGameObjectScale(selectedGameObject, evt.newValue, lastScaleVector3);
            });
            #endregion
            
            #region Button Actions
            createNewEmptyListButton.clicked += TransformGuiMethods.CreateNewTransformListEntry;
            createNewEmptyListButton.clicked += UpdateRadioButtonDisplay;
            
            saveTransformButton.clicked += SaveTransformAction;
            saveTransformButton.clicked += UpdateTransformListAction;
            
            resetButton.clicked += ResetInputAction;
            
            setAbsoluteTransformButton.clicked += SaveAbsoluteTransformAction;
            setAbsoluteTransformButton.clicked += UpdateTransformListAction;

            deleteTransformButton.clicked += DeleteTransformAction;
            deleteTransformButton.clicked += UpdateTransformListAction;
            #endregion
            
            ResetInputAction();
            UpdateTransformListAction();
            DropdownFieldAction();
            
            UpdateRadioButtonDisplay();
            radioButtonGroup.value = 0;
            return root;
        }
    }
    #endif
}
