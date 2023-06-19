# if false
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public static class TransformGuiMethods
    {
        public static void HighlightListEntry(List<int> selectedIndexList, VisualElement listRoot)
        {
            if (listRoot.childCount < 1)
            {
                return;
            }
            
            for (int i = 0; i < listRoot.childCount; i++)
            {
                var elementSelected = false;
                var listEntry = listRoot.ElementAt(i);
                foreach (var t in selectedIndexList)
                {
                    if (i == t)
                    {
                        elementSelected = true;
                        break;
                    }
                    
                }
                
                if (elementSelected)
                {
                    listEntry.style.backgroundColor = new StyleColor(Color.blue);
                }
                else
                {
                    listEntry.style.backgroundColor = i % 2 == 0 ? new StyleColor(Color.clear) : new StyleColor(Color.gray);
                }
            }
        }
        
        public static void CopyTransformListEntry(TransformMonobehaviour mono, int index)
        {
            TransformMonobehaviour.CustomTransformContainer transformListEntry = new TransformMonobehaviour.CustomTransformContainer
            {
                transformListName = mono.transformListContainer[index].transformListName + " Copy"
            };

            List<TransformMonobehaviour.CustomTransform> oldList = mono.transformListContainer[index].singleTransformList;
            List<TransformMonobehaviour.CustomTransform> newList = new List<TransformMonobehaviour.CustomTransform>(oldList.Count);
            
            oldList.ForEach((item)=>
            {
                newList.Add(item.Clone());
            });
            
            transformListEntry.singleTransformList = newList;
            mono.transformListContainer.Add(transformListEntry);
        }
        
        public static void CreateNewTransformListEntry(TransformMonobehaviour mono, bool calledAtStart)
        {
            var newTransform = new TransformMonobehaviour.CustomTransform();
            if (calledAtStart)
            {
                var o = mono.gameObject;

                newTransform.position = o.transform.position;   
                newTransform.rotation = o.transform.rotation.eulerAngles;
                newTransform.scale = o.transform.localScale;
                newTransform.transformType = TransformMonobehaviour.TransformTypes.Absolute;
                newTransform.transformSpecifier = TransformMonobehaviour.TransformSpecifier.NoTransform;
                newTransform.isActive = true;
                newTransform.transformName = "Start Transform";
            }
            else
            {
                newTransform.position = Vector3.zero;
                newTransform.rotation = Vector3.zero;
                newTransform.scale = Vector3.one;
                newTransform.transformType = TransformMonobehaviour.TransformTypes.Absolute;
                newTransform.transformSpecifier = TransformMonobehaviour.TransformSpecifier.NoTransform;
                newTransform.isActive = true;
                newTransform.transformName = "Start Transform";
            }
            
            var emptyTransformList = new List<TransformMonobehaviour.CustomTransform> { newTransform };
            
            TransformMonobehaviour.CustomTransformContainer transformListEntry = new TransformMonobehaviour.CustomTransformContainer
            {
                singleTransformList = emptyTransformList,
                transformListName = "Transform List " + mono.transformListContainer.Count
            };
                    
            mono.transformListContainer.Add(transformListEntry);
        }
        
        public static void ToggleAbsoluteTransform(int position, int previousAbsolute, TransformMonobehaviour.CustomTransform customTransform, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
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
        
        public static void SaveRelativeTransformFromGui(TransformMonobehaviour mono, Vector3 translation, Vector3 rotation, Vector3 scale, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            if (translation == Vector3.zero &&
                rotation == Vector3.zero &&
                scale == Vector3.one)
            {
                mono.noEntry = true;
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

            var transformName = SetTransformName(mono, customTransform);
            customTransform.transformName = transformName;
            mono.noEntry = false;
            activeTransformListInput.Add(customTransform);
        }

        public static void SaveAbsoluteTransform(TransformMonobehaviour mono, Vector3 translation, Vector3 rotation, Vector3 scale, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
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
            
            var transformName = SetTransformName(mono, absoluteTransform);
            absoluteTransform.transformName = transformName;
            activeTransformListInput.Add(absoluteTransform);
        }

        private static string SetTransformName(TransformMonobehaviour mono, TransformMonobehaviour.CustomTransform customTransform)
        {
            var transformName = "";
            
            switch (customTransform.transformSpecifier)
            {
                case TransformMonobehaviour.TransformSpecifier.Translation:
                    transformName += "Translation Changed";
                    break;    
                case TransformMonobehaviour.TransformSpecifier.Rotation:
                    transformName += "Rotation Changed";
                    break;
                case TransformMonobehaviour.TransformSpecifier.Scale:
                    transformName += "Scale Changed";
                    break;
                case TransformMonobehaviour.TransformSpecifier.MultipleTransforms:
                    transformName += "Transform Changed";
                    break;
                case TransformMonobehaviour.TransformSpecifier.AbsoluteTransform:
                    transformName += "Transform Saved";
                    break;
                case TransformMonobehaviour.TransformSpecifier.NoTransform:
                    transformName += "No Transform";
                    break;
                default:
                    transformName += "Unknown Transform Type";
                    break;
            }

            if (mono.applyToVertices)
            {
                transformName += " // Vertices";
            }

            return transformName;
        }
        
        public static void DeactivateTransforms(int startIndex, List<TransformMonobehaviour.CustomTransform> activeTransformListInput)
        {
            var index = activeTransformListInput.Count - 1;
            
            for (var i = startIndex; i < index; i++)
            {
                activeTransformListInput[i].isActive = false;
            }
        }

        public static void CreatePopUp(VisualElement transformContentPanelContainer, Vector3 translation, Vector3 rotation, Vector3 scale) // Create a panel with the transform values
        {
            if (transformContentPanelContainer.childCount > 0) // Check if there is already a selected detail window displayed and delete it
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

                //var scaleChangesVector3 = new Vector3Field();
                //scaleChangesVector3.SetValueWithoutNotify(scale);
                
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
    }
}
#endif