using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace NurGIS.Runtime.TransformHistory.Editor
{
    [CustomEditor(typeof(TransformMonobehaviour))]
    public class TransformUI : UnityEditor.Editor
    {
        public VisualTreeAsset mUxml;

        public override VisualElement CreateInspectorGUI()
        {
            #region Properties
            var root = new VisualElement();
            var transformMono = (TransformMonobehaviour)target;
            mUxml.CloneTree(root);
            
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

            if (transformMono.transformListContainer.Count > 0) // Sets values for the slider and the radiobutton group whenever the GUI is loaded
            {
                var activeDefaultTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
            
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
            Action saveTransformAction = () =>
            {
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                
                TransformGuiMethods.SaveRelativeTransformFromGui(transformMono, translationInput.value, rotationInput.value, scaleInput.value, activeTransformList);
                if (transformMono.noEntry) return;
                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);

                switch (transformMono.applyToVertices)
                {
                    case true:
                        TransformCalculation.ApplyTransformToVertices(transformMono, transformList[0], transformList[1], transformList[2]);
                        break;
                    case false:
                        TransformGoOperations.ApplyTransformToGo(transformMono, transformList[0], transformList[1], transformList[2]);
                        break;
                }
                    
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
                    
                translationInput.value = Vector3.zero;
                rotationInput.value = Vector3.zero;
                scaleInput.value = Vector3.one;
                    
                transformMono.positionInput = Vector3.zero;
                transformMono.rotationInput = Vector3.zero;
                transformMono.scaleInput = Vector3.one;
            };
            
            Action resetInputAction = () =>
            {
                translationInput.value = Vector3.zero;
                rotationInput.value = Vector3.zero;
                scaleInput.value = Vector3.one;
            };
            
            Action copyTransformListButtonAction = () =>
            {
                var selectedTransformListIndex = radioButtonGroup.value;
                TransformGuiMethods.CopyTransformListEntry(transformMono, selectedTransformListIndex);
            };
            
            Action createNewEmptyListButtonAction = () =>
            {
                TransformGuiMethods.CreateNewTransformListEntry(transformMono, false);
            };
            
            Action saveAbsoluteTransformAction = () =>
            {
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGuiMethods.SaveAbsoluteTransform(transformMono, transformList[0], transformList[1], transformList[2], activeTransformList);
                TransformGuiMethods.DeactivateTransforms(lastAbsoluteTransform, activeTransformList);
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
            };
            
            Action resetAllTransformsAction = () =>
            {
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                activeTransformList.RemoveRange(1, activeTransformList.Count - 1);
                activeTransformList[0].isActive = true;
                
                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.ApplyTransformToGo(transformMono, transformList[0], transformList[1], transformList[2]);
                
                slider.highValue = activeTransformList.Count -1;
                slider.value = activeTransformList.Count -1;
            };

            Action updateRadioButtonDisplay = () =>
            {
                var transformListNames = new List<string>(); // Iterate over helper all entries in the transform list container and create a new radio button for each entry
                
                foreach (TransformMonobehaviour.CustomTransformContainer transformList in transformMono.transformListContainer)
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
            };
            
            Action updateTransformListAction = () =>
            {
                if (transformMono.noEntry || radioButtonGroup.value == -1 || transformMono.transformListContainer.Count == 0)
                {
                    return;
                }
                
                listRoot.Clear();
                uxmlFoldout.Add(listRoot);

                var transformContentPanelContainer = new VisualElement();
                
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                for (var i = 0; i < activeTransformList.Count; i++)
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
                        switch (evt.button)
                        {
                            case 0: // Left click on list, highlights the entry and sets the active index
                                transformMono.selectedTransformListIndex.Clear();
                                transformMono.selectedTransformListIndex.Add(index);
                                TransformGuiMethods.HighlightListEntry(transformMono.selectedTransformListIndex, listRoot);
                                var singleTransform = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList[index];
                                TransformGuiMethods.CreatePopUp(transformContentPanelContainer, singleTransform.position, singleTransform.rotation, singleTransform.scale);
                                break;
                            
                            case 1: // Right click, highlights multiple entries and adds entries
                                if (!transformMono.selectedTransformListIndex.Contains(index))
                                {
                                    transformMono.selectedTransformListIndex.Add(index);
                                    transformMono.selectedTransformListIndex.Sort();
                                }
                                TransformGuiMethods.HighlightListEntry(transformMono.selectedTransformListIndex, listRoot);
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
                            var nextAbsoluteTransform = TransformCalculation.FindNextAbsoluteTransformIndex(index, onlyActiveTransforms:true, activeTransformList);
                            var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms:true, activeTransformList);
                            
                            if (lastAbsoluteTransform == 0  &&
                                index < nextAbsoluteTransform && 
                                selectedTransform.transformType != TransformMonobehaviour.TransformTypes.Absolute &&
                                activeTransformList[nextAbsoluteTransform].transformType == TransformMonobehaviour.TransformTypes.Absolute &&
                                activeTransformList[nextAbsoluteTransform].isActive)
                            {
                                toggle.value = false;
                                return;
                            }
                            
                            if (selectedTransform.transformType == TransformMonobehaviour.TransformTypes.Absolute)
                            {
                                lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(index, onlyActiveTransforms:false, activeTransformList);
                                TransformGuiMethods.ToggleAbsoluteTransform(index, lastAbsoluteTransform, selectedTransform, activeTransformList);
                            }
                            
                            var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, nextAbsoluteTransform, activeTransformList);
                            TransformGoOperations.ApplyTransformToGo(transformMono, transformList[0], transformList[1], transformList[2]);
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
                
                listRoot.Add(transformContentPanelContainer);
            };
            
            Action deleteTransformAction = () =>
            {
                if (radioButtonGroup.value == -1 || 
                    transformMono.transformListContainer[radioButtonGroup.value].singleTransformList.Count == 0)
                {
                    return;
                }
                
                if (transformMono.selectedTransformListIndex.Count < 1)
                {
                    Debug.Log("No transform selected");
                    return;
                }
                
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;

                for (var i = transformMono.selectedTransformListIndex.Count - 1; i >= 0; i--)
                {
                    activeTransformList.RemoveAt(transformMono.selectedTransformListIndex[i]);
                }
                
                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.ApplyTransformToGo(transformMono, transformList[0], transformList[1], transformList[2]);
                
                slider.highValue = activeTransformList.Count -1;
                transformMono.selectedTransformListIndex.Clear();
            };

            Action deleteListAction = () =>
            {
                if (radioButtonGroup.value == -1 || 
                    transformMono.transformListContainer.Count is 0 or 1)
                {
                    return;
                }
                
                transformMono.transformListContainer.RemoveAt(radioButtonGroup.value);
                radioButtonGroup.value -= 1;
            };
            
            Action dropdownFieldAction = () =>
            {
                var dropdownFieldOptions = new List<String>
                {
                    "Copy List",
                    "Delete List",
                    "Reset Transforms"
                };

                dropdownField.choices = dropdownFieldOptions;
                dropdownField.value = null;
            };
            #endregion

            #region Callback Events
            slider.RegisterValueChangedCallback(evt => // Changes in Slider position
            {
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                
                if(activeTransformList[evt.newValue].transformType != TransformMonobehaviour.TransformTypes.Absolute)
                {
                    var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(evt.newValue, onlyActiveTransforms:true, activeTransformList);
                    var transformList = TransformCalculation.CalculateSliderTransform(lastAbsoluteTransform, evt.newValue, activeTransformList);
                    TransformGoOperations.ApplyTransformToGo(transformMono, transformList[0], transformList[1], transformList[2]);
                }

                if (activeTransformList[evt.newValue].transformType == TransformMonobehaviour.TransformTypes.Absolute && activeTransformList[evt.newValue].isActive) // In case we are setting the slider to an absolute transform
                {
                    TransformGoOperations.ApplyTransformToGo(transformMono, activeTransformList[evt.newValue].position, activeTransformList[evt.newValue].rotation, activeTransformList[evt.newValue].scale);
                }
            });
            
            radioButtonGroup.RegisterValueChangedCallback(evt => // Changes of the RadioButtonGroup for selecting different transform LoLs
            {
                if (evt.newValue == -1)
                {
                    radioButtonGroup.value = evt.previousValue;    
                }
                
                updateTransformListAction();
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                var lastAbsoluteTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastAbsoluteTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.ApplyTransformToGo(transformMono, transformList[0], transformList[1], transformList[2]);
                
                uxmlFoldout.text = transformMono.transformListContainer[radioButtonGroup.value].transformListName;
                slider.highValue = activeTransformList.Count - 1;
                slider.value = activeTransformList.Count - 1;
                
                transformMono.activeRadioButton = evt.newValue;
                transformMono.selectedTransformListIndex.Clear();
            });

            dropdownField.RegisterValueChangedCallback(evt =>
            {
                switch (evt.newValue)
                {
                    case "Copy List":
                        copyTransformListButtonAction();
                        updateRadioButtonDisplay();
                        break;
                    case "Delete List":
                        deleteListAction();
                        updateRadioButtonDisplay();  
                        updateTransformListAction();
                        break;
                    case "Reset Transforms":
                        resetAllTransformsAction();
                        updateTransformListAction();
                        break;
                }
                dropdownField.value = null;
            });

            translationInput.RegisterValueChangedCallback(evt =>
            {
                if (transformMono.transformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                var translationVector3 = transformList[0];
                TransformGoOperations.UpdateGameObjectTranslation(transformMono, evt.newValue, translationVector3);
            });
            
            rotationInput.RegisterValueChangedCallback(evt =>
            {
                if (transformMono.transformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }

                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                TransformGoOperations.UpdateGameObjectRotation(transformMono, evt.newValue, transformList[1]);
            });
            
            scaleInput.RegisterValueChangedCallback(evt =>
            {
                if (transformMono.transformListContainer.Count == 0)
                {
                    return;
                }
                
                var activeTransformList = transformMono.transformListContainer[radioButtonGroup.value].singleTransformList;
                
                if (activeTransformList.Count == 0)
                {
                    return;
                }
                
                var lastActiveTransform = TransformCalculation.FindLastAbsoluteTransformIndex(activeTransformList.Count - 1, onlyActiveTransforms:true, activeTransformList);
                var transformList = TransformCalculation.CalculateTransform(lastActiveTransform, activeTransformList.Count - 1, activeTransformList);
                var lastScaleVector3 = transformList[2];
                TransformGoOperations.UpdateGameObjectScale(transformMono, evt.newValue, lastScaleVector3);
            });
            #endregion
            
            #region Button Actions
            createNewEmptyListButton.clicked += createNewEmptyListButtonAction;
            createNewEmptyListButton.clicked += updateRadioButtonDisplay;
            
            saveTransformButton.clicked += saveTransformAction;
            saveTransformButton.clicked += updateTransformListAction;
            
            resetButton.clicked += resetInputAction;
            
            setAbsoluteTransformButton.clicked += saveAbsoluteTransformAction;
            setAbsoluteTransformButton.clicked += updateTransformListAction;

            deleteTransformButton.clicked += deleteTransformAction;
            deleteTransformButton.clicked += updateTransformListAction;
            #endregion
            
            resetInputAction();
            updateTransformListAction();
            dropdownFieldAction();
            
            updateRadioButtonDisplay();
            radioButtonGroup.value = 0;
            return root;
        }
    }
}
