using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NurGIS.Runtime.TransformHistory
{
    public static class ThMethods
    {
        ////////////////////////////// Transform Group Functions ////////////////////////////
        #region Transform Group Functions
        public static void DrawGUI(GameObject go, RadioButtonGroup radioButtonGroup)
        {
            var transformGroupNames = GetTransformGroupNames(ThMono.TransformListContainer);
            radioButtonGroup.choices = transformGroupNames;
            
            // Create a callback for when a new RadioButton is added, add all the necessary UI elements
            var allRadioButtons = radioButtonGroup.Query<RadioButton>();
            
            var radioButtonIndex = -1;
            allRadioButtons.ForEach(radioButton =>
            {
                radioButtonIndex++;
                radioButton.Children().First().style.paddingBottom = 10;
                
                var renameButton = new Button()
                {
                    text = "Rename",
                    style =
                    {
                        right = 117,
                        position = Position.Absolute
                    },
                };
                
                var copyButton = new Button()
                {
                    text = "Copy",
                    style =
                    {
                        right = 72,
                        position = Position.Absolute
                    },
                };
                
                var resetButton = new Button()
                {
                    text = "Reset",
                    style =
                    {
                        right = 24,
                        position = Position.Absolute
                    },
                };
                
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
                    CreateAllSingleTransforms(radioButtonIndex, radioButtonGroup, foldout, go);
                }

                radioButton.Children().First().Add(copyButton);
                radioButton.Children().First().Add(resetButton);
                radioButton.Children().First().Add(renameButton);
                radioButton.Children().First().Add(addTransformButton);
                radioButton.Add(foldout);

                copyButton.clicked += () => { CopyTransformGroup(radioButtonGroup.value, transformGroupNames, radioButtonGroup); };
                copyButton.clicked += () => { DrawGUI(go, radioButtonGroup); };
                resetButton.clicked += () => { ResetTransformGroup(radioButtonIndex, go, radioButtonGroup); };
                
                renameButton.clicked += () => { RenameTransformGroup(radioButtonIndex, transformGroupNames, radioButtonGroup); };
                addTransformButton.clicked += () => { AddSingleTransform(radioButtonIndex, foldout, radioButtonGroup, go); };
            });
        }
        
        private static void CopyTransformGroup(int index, List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {
            if (index < 0 || index >= ThMono.TransformListContainer.Count)
            {
                return;
            }
            
            ThMono.CustomTransformContainer transformListEntry =
                new ThMono.CustomTransformContainer
                {
                    transformListName = ThMono.TransformListContainer[index].transformListName + " Copy"
                };

            List<ThMono.CustomTransform> oldList =
                ThMono.TransformListContainer[index].singleTransformList;
            
            List<ThMono.CustomTransform> newList =
                new List<ThMono.CustomTransform>(oldList.Count);

            oldList.ForEach(item => { newList.Add(item.Clone()); });

            transformListEntry.singleTransformList = newList;
            ThMono.TransformListContainer.Add(transformListEntry);
            
            radioButtonGroupsList.Add(transformListEntry.transformListName);
            radioButtonGroup.choices = radioButtonGroupsList;
        }
        
        public static List<string> GetTransformGroupNames(List<ThMono.CustomTransformContainer> transformContainer)
        {
            var transformGroupNames = new List<string>();
            
            foreach (ThMono.CustomTransformContainer cTc in transformContainer)
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

            var newTransform = new ThMono.CustomTransform
            {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
                isActive = true,
                transformName = "Start Transform"
            };

            var emptyTransformList = new List<ThMono.CustomTransform> { newTransform };

            ThMono.CustomTransformContainer transformListEntry =
                new ThMono.CustomTransformContainer
                {
                    singleTransformList = emptyTransformList,
                    transformListName = "Transform List " + ThMono.TransformListContainer.Count
                };

            ThMono.TransformListContainer.Add(transformListEntry);
        }
        
        public static void DeleteTransformGroup(List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {
            if (radioButtonGroup.value == -1 || 
                ThMono.TransformListContainer.Count is 0 or 1)
            {
                return;
            }

            var newRadioValue = radioButtonGroup.value - 1;
            
            ThMono.TransformListContainer.RemoveAt(radioButtonGroup.value);
            radioButtonGroupsList.RemoveAt(radioButtonGroup.value);
            radioButtonGroup.choices = radioButtonGroupsList;
            
            radioButtonGroup.value -= newRadioValue;
        }
        
        private static void ResetTransformGroup(int index, GameObject go, RadioButtonGroup radioButtonGroup)
        {
            if (radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = index;
            }
            
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
            activeTransformList.RemoveRange(1, activeTransformList.Count - 1);

            DrawGUI(go, radioButtonGroup);
        }
        
        private static void RenameTransformGroup(int radioButtonGroupIndex, List<string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {

            radioButtonGroup.value = radioButtonGroupIndex;
            
            var radioButtonFoldout = radioButtonGroup.Q<Foldout>();
            
            var toggleRow = radioButtonGroup.Q<RadioButton>().Children().First();
            
            var renameTransformField = new TextField
            {
                value = radioButtonFoldout.text,
                style =
                {
                    width = 120,
                    position = Position.Absolute,
                    right = 190,
                }
            };
            
            var newTransformName = renameTransformField.value;
            
            var textContainer = renameTransformField.Children().First();
            textContainer.style.unityTextAlign = TextAnchor.MiddleCenter;
            textContainer.style.unityFontStyleAndWeight = FontStyle.Bold;
            textContainer.style.paddingBottom = 1;
            
            toggleRow.Add(renameTransformField);
            
            renameTransformField.RegisterValueChangedCallback(evt =>
            {
                newTransformName = evt.newValue;
            });
            
            // Callback for when the user clicks outside of the text field or presses enter
            renameTransformField.RegisterCallback<BlurEvent>(_ =>
            {
                ThMono.TransformListContainer[radioButtonGroup.value].transformListName = newTransformName;
                radioButtonGroupsList[radioButtonGroup.value] = newTransformName;
                radioButtonGroup.choices = radioButtonGroupsList;
                toggleRow.Remove(renameTransformField);
                radioButtonGroup.value = radioButtonGroupIndex;
            });
            
            renameTransformField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode != KeyCode.Return) return;
                ThMono.TransformListContainer[radioButtonGroup.value].transformListName = newTransformName;
                radioButtonGroupsList[radioButtonGroup.value] = newTransformName;
                radioButtonGroup.choices = radioButtonGroupsList;
                toggleRow.Remove(renameTransformField);
                radioButtonGroup.value = radioButtonGroupIndex;
            });
        }
        
        public static void CreateAllSingleTransforms(int transformGroupIndex, RadioButtonGroup radioButtonGroup, Foldout transformFoldout, GameObject go)
        {
            if(radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = transformGroupIndex;
            }
            
            var activeTransformList = ThMono.TransformListContainer[transformGroupIndex].singleTransformList;
            
            transformFoldout.Clear();

            var singleTransformIndex = 0;
            
            foreach (ThMono.CustomTransform cT in activeTransformList)
            {
                transformFoldout.Add(new ThSingleTContainer(cT.transformName, singleTransformIndex, transformGroupIndex, radioButtonGroup, transformFoldout, go));
                singleTransformIndex++;
            }
        }
        #endregion
        
        ////////////////////////////// Single Transform Functions ////////////////////////////
        #region Single Transform Functions
        private static void AddSingleTransform(int singleTransformIndex, Foldout radioButtonFoldout, RadioButtonGroup radioButtonGroup, GameObject go)
        {
            if (radioButtonGroup.value == -1)
            {
                radioButtonGroup.value = singleTransformIndex;
            }
            
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
            
            var customTransform = new ThMono.CustomTransform
            {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
                isActive = true,
                transformName = "Transform:"
            };
            
            activeTransformList.Add(customTransform);
            radioButtonFoldout.Add(new ThSingleTContainer(customTransform.transformName, activeTransformList.Count - 1, radioButtonGroup.value, radioButtonGroup, radioButtonFoldout, go));
        }
        
        public static void DeleteSingleTransform(int singleTransformIndex, RadioButtonGroup radioButtonGroup)
        {
            
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
            activeTransformList.RemoveAt(singleTransformIndex);
        }

        public static void RenameSingleTransformInputWindow(int singleTransformIndex, RadioButtonGroup radioButtonGroup, ThSingleTContainer thSingleTContainer)
        {
            var singleTransformFoldout = thSingleTContainer.Q<Foldout>();
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
            
            var newTransformName = "";

            // Callback for when the user changes the text field, updates the transform name in the GUI and the container
            renameTransformField.RegisterValueChangedCallback(evt =>
            {
                newTransformName = evt.newValue;
            });
            
            // Callback for when the user clicks outside of the text field or presses enter
            renameTransformField.RegisterCallback<BlurEvent>(_ =>
            {
                if (radioButtonGroup.value == -1) return;
                thSingleTContainer.Q<Foldout>().text = newTransformName;
                var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
                activeTransformList[singleTransformIndex].transformName = newTransformName;
                toggleRow.Remove(renameTransformField);
            });
            renameTransformField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (radioButtonGroup.value == -1) return;
                if (evt.keyCode != KeyCode.Return) return;
                thSingleTContainer.Q<Foldout>().text = newTransformName;
                var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
                activeTransformList[singleTransformIndex].transformName = newTransformName;
                toggleRow.Remove(renameTransformField);
            });
        }
        #endregion
        
        ////////////////////////////// Utility Functions ////////////////////////////
        #region Utility Functions
        public static void UpdateTransform(Vector3 translation, Vector3 rotation, Vector3 scale, RadioButtonGroup radioButtonGroup, int singleTransformIndex, GameObject go)
        {
            if (radioButtonGroup.value == -1) return;
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
            var selectedTransform = activeTransformList[singleTransformIndex];
            
            if (translation!= Vector3.zero)
            {
                selectedTransform.position = translation;
            }
            if (rotation != Vector3.zero)
            {
                selectedTransform.rotation = rotation;
            }
            if (scale != Vector3.one)
            {
                selectedTransform.scale = scale;
            }
            
            ApplyTransform(radioButtonGroup, go);
        }

        public static void ApplyTransform(RadioButtonGroup radioButtonGroup, GameObject go)
        {
            if (radioButtonGroup.value < 0) return;
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;

            var translation = Vector3.zero;
            var rotation = Vector3.zero;
            var scale = Vector3.one;

            for (var i = 0; i < activeTransformList.Count(); i++)
            {
                ThMono.CustomTransform customTransform = activeTransformList[i];
                if (!customTransform.isActive) continue;
                if(customTransform.appliedToVertices) continue;
                rotation += customTransform.rotation;
                translation += customTransform.position;
                scale = Vector3.Scale(scale, customTransform.scale);
            }
            
            go.transform.localPosition = translation;
            go.transform.localRotation = Quaternion.Euler(rotation);
            go.transform.localScale = scale;
        }

        public static void MakeTransformAbsolute(int singleTransformIndex, List< string> radioButtonGroupsList, RadioButtonGroup radioButtonGroup)
        {
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value];

            ThMono.CustomTransformContainer transformListEntry =
                new ThMono.CustomTransformContainer
                {
                    transformListName = activeTransformList.transformListName + " Collapsed",
                    singleTransformList = new List<ThMono.CustomTransform>()
                };
            
            var translation = Vector3.zero;
            var rotation = Vector3.zero;
            var scale = Vector3.one;

            for (var i = 0; i <= singleTransformIndex; i++)
            {
                ThMono.CustomTransform customTransform = activeTransformList.singleTransformList[i];
                if (!customTransform.isActive) continue;
                rotation += customTransform.rotation;
                translation += customTransform.position;
                scale = Vector3.Scale(scale, customTransform.scale);
            }
            
            transformListEntry.singleTransformList.Add(new ThMono.CustomTransform
            {
                position = translation,
                rotation = rotation,
                scale = scale,
                isActive = true,
                transformName = "Start Position:"
            });
            
            ThMono.TransformListContainer.Add(transformListEntry);
            radioButtonGroupsList.Add(transformListEntry.transformListName);
            radioButtonGroup.choices = radioButtonGroupsList;
        }

        public static void ApplyTransformToVertices(bool apply, int singleTransformIndex, RadioButtonGroup radioButtonGroup, GameObject go)
        {
            if (radioButtonGroup.value == -1) return;
            var activeTransformList = ThMono.TransformListContainer[radioButtonGroup.value].singleTransformList;
            var selectedTransform = activeTransformList[singleTransformIndex];
            
            var mesh = go.GetComponent<MeshFilter>().mesh;
            
            Vector3[] vertices = mesh.vertices;
            Matrix4x4 matrix = Matrix4x4.TRS(selectedTransform.position, Quaternion.Euler(selectedTransform.rotation), selectedTransform.scale);
            Matrix4x4 inverseMatrix = matrix.inverse;
            
            if (apply)
            {
                for (var i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
                }
            }
            else
            {
                for (var i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = inverseMatrix.MultiplyPoint3x4(vertices[i]);
                }
            }
            
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        public static void SaveStartPosition(GameObject go)
        {
            var newTransform = new ThMono.CustomTransform
            {
                position = go.transform.position,
                rotation = go.transform.rotation.eulerAngles,
                scale = go.transform.localScale,
                isActive = true,
                transformName = "Start Transform"
            };

            var emptyTransformList = new List<ThMono.CustomTransform> { newTransform };

            var transformListEntry = new ThMono.CustomTransformContainer
            {
                singleTransformList = emptyTransformList,
                transformListName = "Transform List " + ThMono.TransformListContainer.Count
            };

            ThMono.TransformListContainer.Add(transformListEntry);
        }
        #endregion
    }
}