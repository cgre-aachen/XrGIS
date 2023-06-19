﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public static class TransformGuiMethods
    {
        #region Old GUI Properties
        public static void HighlightListEntry(List<int> selectedIndexList, VisualElement listRoot)
        {
            if (listRoot.childCount < 1)
            {
                return;
            }

            for (var i = 0; i < listRoot.childCount; i++)
            {
                var elementSelected = false;
                var listEntry = listRoot.ElementAt(i);
                var index = 0;
                for (; index < selectedIndexList.Count; index++)
                {
                    var t = selectedIndexList[index];
                    if (i != t) continue;
                    elementSelected = true;
                    break;
                }

                if (elementSelected)
                {
                    listEntry.style.backgroundColor = new StyleColor(Color.blue);
                }
                else
                {
                    listEntry.style.backgroundColor =
                        i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray);
                }
            }
        }

        public static void CopyTransformListEntry(int index)
        {
            TransformMonobehaviour.CustomTransformContainer transformListEntry =
                new TransformMonobehaviour.CustomTransformContainer
                {
                    transformListName = TransformMonobehaviour.TransformListContainer[index].transformListName + " Copy"
                };

            var oldList = TransformMonobehaviour.TransformListContainer[index].singleTransformList;
            
            var newList = new List<TransformMonobehaviour.CustomTransform>(oldList.Count);

            oldList.ForEach(item => { newList.Add(item.Clone()); });

            transformListEntry.singleTransformList = newList;
            TransformMonobehaviour.TransformListContainer.Add(transformListEntry);
        }

        public static void SaveStartPosition(GameObject go)
        {
            var newTransform = new TransformMonobehaviour.CustomTransform
            {
                position = go.transform.position,
                rotation = go.transform.rotation.eulerAngles,
                scale = go.transform.localScale,
                transformType = TransformMonobehaviour.TransformTypes.Absolute,
                transformSpecifier = TransformMonobehaviour.TransformSpecifier.NoTransform,
                isActive = true,
                transformName = "Start Transform"
            };

            var emptyTransformList = new List<TransformMonobehaviour.CustomTransform> { newTransform };

            var transformListEntry = new TransformMonobehaviour.CustomTransformContainer
                {
                    singleTransformList = emptyTransformList,
                    transformListName = "Transform List " + TransformMonobehaviour.TransformListContainer.Count
                };

            TransformMonobehaviour.TransformListContainer.Add(transformListEntry);
        }

        public static void CreateNewTransformListEntry()
        {
            var newTransform = new TransformMonobehaviour.CustomTransform
            {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
                transformType = TransformMonobehaviour.TransformTypes.Absolute,
                transformSpecifier = TransformMonobehaviour.TransformSpecifier.NoTransform,
                isActive = true,
                transformName = "Start Transform"
            };

            var emptyTransformList = new List<TransformMonobehaviour.CustomTransform> { newTransform };

            var transformListEntry = new TransformMonobehaviour.CustomTransformContainer
                {
                    singleTransformList = emptyTransformList,
                    transformListName = "Transform List " + TransformMonobehaviour.TransformListContainer.Count
                };

            TransformMonobehaviour.TransformListContainer.Add(transformListEntry);
        }

        public static void ToggleAbsoluteTransform(int position, int previousAbsolute,
            TransformMonobehaviour.CustomTransform customTransform,
            List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            if (activeTransformListInput.Count == 1)
            {
                return;
            }

            if (customTransform.isActive) //Scenario 1; Absolute toggle gets activated
            {
                for (var i = position - 1; i >= 0; i--)
                {
                    if (position == 0) // If the absolute transform is the first transform in the list
                    {
                        break;
                    }

                    activeTransformListInput[i].isActive = false;
                }

                for (var i = position + 1; i < activeTransformListInput.Count; i++)
                {
                    activeTransformListInput[i].isActive = activeTransformListInput[i].transformType != TransformMonobehaviour.TransformTypes.Absolute;
                }
            }

            else // Scenario 2; Absolute toggle gets deactivated
            {
                for (var i = position - 1; i >= previousAbsolute; i--)
                {
                    if (activeTransformListInput[i].transformType != TransformMonobehaviour.TransformTypes.Absolute)
                    {
                        activeTransformListInput[i].isActive = true;
                    }
                    else
                    {
                        activeTransformListInput[i].isActive = true;
                        break;
                    }
                }
            }
        }

        public static void SaveRelativeTransformFromGui(Vector3 translation,
            Vector3 rotation, Vector3 scale, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            if (translation == Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                TransformMonobehaviour.noEntry = true;
                Debug.Log("Enter a value");
                return;
            }

            TransformMonobehaviour.CustomTransform customTransform = new TransformMonobehaviour.CustomTransform
            {
                transformType = TransformMonobehaviour.TransformTypes.Relative,
                isActive = true,
                position = translation,
                rotation = rotation,
                scale = scale,
            };

            if (translation != Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                customTransform.transformSpecifier = TransformMonobehaviour.TransformSpecifier.Translation;
            }
            else if (rotation != Vector3.zero &&
                     translation == Vector3.zero &&
                     scale == Vector3.one)
            {
                customTransform.transformSpecifier = TransformMonobehaviour.TransformSpecifier.Rotation;
            }
            else if (scale != Vector3.one &&
                     translation == Vector3.zero &&
                     rotation == Vector3.zero)
            {
                customTransform.transformSpecifier = TransformMonobehaviour.TransformSpecifier.Scale;
            }
            else
            {
                customTransform.transformSpecifier = TransformMonobehaviour.TransformSpecifier.MultipleTransforms;
            }

            var transformName = SetTransformName(customTransform);
            customTransform.transformName = transformName;
            TransformMonobehaviour.noEntry = false;
            activeTransformListInput.Add(customTransform);
        }

        public static void SaveAbsoluteTransform(Vector3 translation, Vector3 rotation, Vector3 scale, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            TransformMonobehaviour.CustomTransform absoluteTransform = new TransformMonobehaviour.CustomTransform()
            {
                position = translation,
                rotation = rotation,
                scale = scale,
                transformType = TransformMonobehaviour.TransformTypes.Absolute,
                transformSpecifier = TransformMonobehaviour.TransformSpecifier.AbsoluteTransform,
                isActive = true,
            };

            var transformName = SetTransformName(absoluteTransform);
            absoluteTransform.transformName = transformName;
            activeTransformListInput.Add(absoluteTransform);
        }

        private static string SetTransformName(TransformMonobehaviour.CustomTransform customTransform)
        {
            var transformName = "";

            transformName += customTransform.transformSpecifier switch
            {
                TransformMonobehaviour.TransformSpecifier.Translation => "Translation Changed",
                TransformMonobehaviour.TransformSpecifier.Rotation => "Rotation Changed",
                TransformMonobehaviour.TransformSpecifier.Scale => "Scale Changed",
                TransformMonobehaviour.TransformSpecifier.MultipleTransforms => "Transform Changed",
                TransformMonobehaviour.TransformSpecifier.AbsoluteTransform => "Transform Saved",
                TransformMonobehaviour.TransformSpecifier.NoTransform => "No Transform",
                _ => "Unknown Transform Type"
            };

            if (TransformMonobehaviour.applyToVertices)
            {
                transformName += " // Vertices";
            }

            return transformName;
        }

        public static void DeactivateTransforms(int startIndex,
            List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            var index = activeTransformListInput.Count - 1;

            for (var i = startIndex; i < index; i++)
            {
                activeTransformListInput[i].isActive = false;
            }
        }

        public static void CreatePopUp(VisualElement transformContentPanelContainer, Vector3 translation,
            Vector3 rotation, Vector3 scale) // Create a panel with the transform values
        {
            if (transformContentPanelContainer.childCount >
                0) // Check if there is already a selected detail window displayed and delete it
            {
                transformContentPanelContainer.RemoveAt(transformContentPanelContainer.childCount - 1);
            }

            var transformValuesPanel = new VisualElement
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column),
                    backgroundColor = new StyleColor(Color.gray)
                }
            };

            var positionValuesPanel = new VisualElement
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    backgroundColor = new StyleColor(Color.gray)
                }
            };

            var positionValuesChangedLabel = new Label
            {
                text = "Translation Changes"
            };

            var positionValueXLabel = new Label
            {
                text = "X: " + translation.x
            };

            var positionValueYLabel = new Label
            {
                text = "Y: " + translation.y
            };

            var positionValueZLabel = new Label
            {
                text = "Z: " + translation.z
            };

            var rotationValuesPanel = new VisualElement
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    backgroundColor = new StyleColor(Color.white)
                }
            };
            var rotationValuesChangedLabel = new Label
            {
                text = "Rotation Changes",
                style =
                {
                    color = new StyleColor(Color.black)
                }
            };

            var rotationValueXLabel = new Label
            {
                text = "X: " + rotation.x,
                style =
                {
                    color = new StyleColor(Color.black)
                }
            };

            var rotationValueYLabel = new Label
            {
                text = "Y: " + rotation.y,
                style =
                {
                    color = new StyleColor(Color.black)
                }
            };

            var rotationValueZLabel = new Label
            {
                text = "Z: " + rotation.z,
                style =
                {
                    color = new StyleColor(Color.black)
                }
            };

            var scaleValuesPanel = new VisualElement
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    backgroundColor = new StyleColor(Color.gray)
                }
            };

            var scaleValuesChangedLabel = new Label
            {
                text = "Scale Changes"
            };

            var scaleValueXLabel = new Label
            {
                text = "X: " + scale.x
            };

            var scaleValueYLabel = new Label
            {
                text = "Y: " + scale.y
            };

            var scaleValueZLabel = new Label
            {
                text = "Z: " + scale.z
            };

            var transformValuesButton = new Button
            {
                text = "Close"
            };

            transformValuesButton.clicked += () =>
            {
                transformValuesPanel.Clear();
                transformValuesPanel.RemoveFromHierarchy();
            };

            positionValuesPanel.Add(positionValuesChangedLabel);
            positionValuesPanel.Add(positionValueXLabel);
            positionValuesPanel.Add(positionValueYLabel);
            positionValuesPanel.Add(positionValueZLabel);

            rotationValuesPanel.Add(rotationValuesChangedLabel);
            rotationValuesPanel.Add(rotationValueXLabel);
            rotationValuesPanel.Add(rotationValueYLabel);
            rotationValuesPanel.Add(rotationValueZLabel);

            scaleValuesPanel.Add(scaleValuesChangedLabel);

            scaleValuesPanel.Add(scaleValueXLabel);
            scaleValuesPanel.Add(scaleValueYLabel);
            scaleValuesPanel.Add(scaleValueZLabel);

            transformValuesPanel.Add(positionValuesPanel);
            transformValuesPanel.Add(rotationValuesPanel);
            transformValuesPanel.Add(scaleValuesPanel);
            transformValuesPanel.Add(transformValuesButton);

            transformContentPanelContainer.Add(transformValuesPanel);
        }
        #endregion

        ////////////////////////////// Transform Group Functions - New GUI ////////////////////////////
        #region Transform Group Functions
        private static void CopyTransformListEntryNew(int index, List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {
            if (index < 0 || index >= TransformMonobehaviour.TransformListContainer.Count)
            {
                return;
            }
            
            TransformMonobehaviour.CustomTransformContainer transformListEntry =
                new TransformMonobehaviour.CustomTransformContainer
                {
                    transformListName = TransformMonobehaviour.TransformListContainer[index].transformListName + " Copy"
                };

            List<TransformMonobehaviour.CustomTransform> oldList =
                TransformMonobehaviour.TransformListContainer[index].singleTransformList;
            
            List<TransformMonobehaviour.CustomTransform> newList =
                new List<TransformMonobehaviour.CustomTransform>(oldList.Count);

            oldList.ForEach(item => { newList.Add(item.Clone()); });

            transformListEntry.singleTransformList = newList;
            TransformMonobehaviour.TransformListContainer.Add(transformListEntry);
            
            radioButtonGroupsList.Add(transformListEntry.transformListName);
            radioButtonGroup.choices = radioButtonGroupsList;
        }
        public static List<string> GetTransformGroupNames(List<TransformMonobehaviour.CustomTransformContainer> transformContainer)
        {
            var transformGroupNames = new List<string>();
            
            foreach (TransformMonobehaviour.CustomTransformContainer cTc in transformContainer)
            {
                var transformGroupName = cTc.transformListName;
                transformGroupNames.Add(transformGroupName);
            }
            
            return transformGroupNames;
        }
        public static void AddTransformGroup(string groupName, List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {
            radioButtonGroupsList.Add(groupName);
            radioButtonGroup.choices = radioButtonGroupsList;

            var newTransform = new TransformMonobehaviour.CustomTransform
            {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
                transformType = TransformMonobehaviour.TransformTypes.Absolute,
                transformSpecifier = TransformMonobehaviour.TransformSpecifier.NoTransform,
                isActive = true,
                transformName = "Start Transform"
            };

            var emptyTransformList = new List<TransformMonobehaviour.CustomTransform> { newTransform };

            TransformMonobehaviour.CustomTransformContainer transformListEntry =
                new TransformMonobehaviour.CustomTransformContainer
                {
                    singleTransformList = emptyTransformList,
                    transformListName = "Transform List " + TransformMonobehaviour.TransformListContainer.Count
                };

            TransformMonobehaviour.TransformListContainer.Add(transformListEntry);
        }
        public static void DeleteTransformGroup(List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {
            if (radioButtonGroup.value == -1 || 
                TransformMonobehaviour.TransformListContainer.Count is 0 or 1)
            {
                return;
            }
            
            TransformMonobehaviour.TransformListContainer.RemoveAt(radioButtonGroup.value);
            radioButtonGroupsList.RemoveAt(radioButtonGroup.value);
            radioButtonGroup.choices = radioButtonGroupsList;
            
            radioButtonGroup.value -= 1;
        }

        private static void AddSingleTransform(int index, Foldout radioButtonFoldout, RadioButtonGroup radioButtonGroup)
        {
            if (radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = index;
            }
            
            var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
            
            var customTransform = new TransformMonobehaviour.CustomTransform
            {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
                transformType = TransformMonobehaviour.TransformTypes.Relative,
                isActive = true,
                transformName = "Transform:"
            };
            
            activeTransformList.Add(customTransform);
            radioButtonFoldout.Add(new SingleTransform(customTransform.transformName, activeTransformList.Count - 1, radioButtonGroup, radioButtonFoldout));
        }

        private static void ResetAllTransformsInSelectedList(int index, GameObject go, RadioButtonGroup radioButtonGroup)
        {
            if (radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = index;
            }
            
            var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
            activeTransformList.RemoveRange(1, activeTransformList.Count - 1);

            CreateAndRegisterCallbackTransformList(go, radioButtonGroup);

        }

        private static void RenameTransformGroup(int index, List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup, string newGroupName)
        {
            if (radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = index;
            }
            
            TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].transformListName = newGroupName;
            radioButtonGroupsList[radioButtonGroup.value] = newGroupName;
            radioButtonGroup.choices = radioButtonGroupsList;
        }
        public static void CreateAllSingleTransforms(int index, RadioButtonGroup radioButtonGroup, Foldout transformFoldout)
        {
            if(radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = index;
            }
            
            var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
            
            transformFoldout.Clear();

            var singleTransformIndex = 0;
            
            foreach (TransformMonobehaviour.CustomTransform cT in activeTransformList)
            {
                transformFoldout.Add(new SingleTransform(cT.transformName, singleTransformIndex, radioButtonGroup, transformFoldout));
                singleTransformIndex++;
            }
        }
        public static void CreateAndRegisterCallbackTransformList(GameObject go, RadioButtonGroup radioButtonGroup) //rewrite so that it uses the list of transform groups
        {
            // Create a callback for when a new RadioButton is added, add all the necessary UI elements
            var allRadioButtons = radioButtonGroup.Query<RadioButton>();
            
            var index = -1;
            allRadioButtons.ForEach(radioButton =>
            {
                index++;
                radioButton.Children().First().style.paddingBottom = 10;

                // Copy group button
                var copyButton = new Button()
                {
                    text = "Copy",
                    style =
                    {
                        right = 133,
                        position = Position.Absolute
                    },
                };

                // Reset group button
                var resetButton = new Button()
                {
                    text = "Reset",
                    style =
                    {
                        right = 85,
                        position = Position.Absolute
                    },
                };

                // Rename group button
                var renameButton = new Button()
                {
                    text = "Rename",
                    style =
                    {
                        right = 24,
                        position = Position.Absolute
                    },
                };
                
                // Add new transform button
                var addTransformButton = new Button()
                {
                    text = "+",
                    style =
                    {
                        right = 0,
                        position = Position.Absolute
                    },
                };

                var foldout = radioButton.Q<Foldout>() ?? new Foldout
                {
                    text = "Transforms"
                };
                
                if (foldout.value)
                {
                    CreateAllSingleTransforms(index, radioButtonGroup, foldout);
                }

                radioButton.Children().First().Add(copyButton);
                radioButton.Children().First().Add(resetButton);
                radioButton.Children().First().Add(renameButton);
                radioButton.Children().First().Add(addTransformButton);
                radioButton.Add(foldout);

                copyButton.clicked += () => { CopyTransformListEntryNew(radioButtonGroup.value, GetTransformGroupNames(TransformMonobehaviour.TransformListContainer), radioButtonGroup); };
                copyButton.clicked += () => { CreateAndRegisterCallbackTransformList(go, radioButtonGroup); };
                resetButton.clicked += () => { ResetAllTransformsInSelectedList(index, go, radioButtonGroup); };
                
                renameButton.clicked += () =>
                {
                    RenameTransformGroup(index, GetTransformGroupNames(TransformMonobehaviour.TransformListContainer), radioButtonGroup, "test");
                };
                
                addTransformButton.clicked += () => { AddSingleTransform(index, foldout, radioButtonGroup); };
            });
        }
        #endregion
        
        ////////////////////////////// Single Transform Functions -- New GUI ////////////////////////////
        #region Single Transform Functions
        public static void DeleteSingleTransform(int i, RadioButtonGroup radioButtonGroup)
        {
            
            var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
            activeTransformList.RemoveAt(i);
        }

        public static void RenameInputWindow(int index, RadioButtonGroup radioButtonGroup, SingleTransform singleTransform)
        {
            var singleTransformFoldout = singleTransform.Q<Foldout>();
            var toggleRow = singleTransformFoldout.Q<Toggle>().Children().First();
            // Create a string input field for renaming the transform
            var renameTransformField = new TextField
            {
                value = singleTransformFoldout.text,
                style =
                {
                    width = 120,
                    position = Position.Absolute,
                    right = 160,
                }
            };
            var textContainer = renameTransformField.Children().First();
            textContainer.style.unityTextAlign = TextAnchor.MiddleCenter;
            textContainer.style.unityFontStyleAndWeight = FontStyle.Bold;
            textContainer.style.paddingBottom = 1;
            
            toggleRow.Add(renameTransformField);

            // Callback for when the user changes the text field, updates the transform name in the GUI and the container
            renameTransformField.RegisterValueChangedCallback(evt =>
            {
                RenameSingleTransform(index, radioButtonGroup, evt.newValue, singleTransform);
            });
            
            // Callback for when the user clicks outside of the text field or presses enter
            renameTransformField.RegisterCallback<BlurEvent>(_ =>
            {
                toggleRow.Remove(renameTransformField);
            });
            renameTransformField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return)
                {
                    toggleRow.Remove(renameTransformField);
                }
            });
        }

        private static void RenameSingleTransform(int i, RadioButtonGroup radioButtonGroup, string newTransformName, SingleTransform singleTransform)
        {
            var singleTransformFoldout = singleTransform.Q<Foldout>();
            singleTransformFoldout.text = newTransformName;
            
            var activeTransformList = TransformMonobehaviour.TransformListContainer[radioButtonGroup.value].singleTransformList;
            activeTransformList[i].transformName = newTransformName;
        }
        
        #endregion
    }
}