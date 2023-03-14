using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Terranigma.SelectionIndicator.Runtime
{
    public class SelectionIndicator : MonoBehaviour
    {

        [Tooltip("The selected game object")] [SerializeField]
        public GameObject selectedGo;
        private float _distClippingPlane;

        [Serializable]
        public class RuntimeOptions
        {
            [Tooltip("The offset of the indicator from the left and right edge of the screen")] [SerializeField]
            public float outOfSightOffsetHorizontal = 30f;

            [Tooltip("The offset of the indicator from the top and bot edge of the screen")] [SerializeField]
            public float outOfSightOffsetVertical = 30f;
            
            [Tooltip("The distance at which indicators are spawned")] [SerializeField]
            public float manualCullingDistance = 200f;
            
            [Tooltip("Multiplier for the scale of the offscreen indicator"), SerializeField] 
            [Range(0.1f, 10.0f)]
            public float manualOffScreenIndicatorSize = 1f;
            
            [Tooltip("Multiplier for the scale of the far plane culling indicator"), SerializeField] 
            [Range(0.1f, 10.0f)]
            public float manualFarPlaneCulledIndicatorSize = 1f;
        }

        [Serializable]
        public class ScreenSpaceOptions
        {
            [Tooltip("The indicator of the selected game object if it is out of sight")]
            public Image offScreenTargetIndicatorImage;

            [Tooltip("The GO of the indicator of the selected game object if it is out of sight")] [SerializeField]
            public GameObject offScreenTargetIndicatorGo;

            [FormerlySerializedAs("targetIndicatorGo")] [Tooltip("The indicator of the selected game object if it is culled")] [SerializeField]
            public GameObject farPlaneCulledIndicatorGo;

            [Tooltip("The canvas to draw the indicator on")] [SerializeField]
            public Canvas canvas;

            [Tooltip("The text of the distance between the player and the selected game object")] [SerializeField]
            public TextMeshProUGUI targetDistanceText;
        }

        private RectTransform _canvasRectScreen;
        private RectTransform _rectTransformOffScreen;
        private RectTransform _rectTransformFarPlaneCulledIndicatorScreen;
        private Vector3 _indicatorPosition;

        [Serializable]
        public class WorldSpaceOptions
        {
            [Tooltip("The indicator of the selected game object if it is out of sight")]
            public Image offScreenTargetIndicatorImageWorld;

            [Tooltip("The GO of the indicator of the selected game object if it is out of sight")] [SerializeField]
            public GameObject offScreenTargetIndicatorGoWorld;

            [Tooltip("The indicator of the selected game object if it is culled")] [SerializeField]
            public GameObject farPlaneCulledIndicatorGoWorld;

            [Tooltip("The canvas to draw the indicator on")] [SerializeField]
            public Canvas canvasWorld;

            [Tooltip("The text of the distance between the player and the selected game object")] [SerializeField]
            public TextMeshProUGUI targetDistanceTextWorld;
        }

        private RectTransform _canvasRectWorld;
        private RectTransform _rectTransformOffScreenWorld;
        private RectTransform _rectTransformOnScreenWorld;
        private Camera _cameraMain;

        [SerializeField] public RuntimeOptions runtimeOptions = new();
        [SerializeField] public ScreenSpaceOptions screenSpaceOptions = new();
        [SerializeField] public WorldSpaceOptions worldSpaceOptions = new();
        

        [Tooltip("Mouse Selection Key")] [SerializeField]
        public bool handleMouseInput = true;
        
        [Tooltip("The text of the distance between the player and the selected game object")] [SerializeField]
        public bool useCustomCullingDistance;
        
        [Tooltip("The text of the distance between the player and the selected game object")] [SerializeField]
        public bool constantIndicatorSize;
        
        [Tooltip("Toggle XR")] [SerializeField]
        public bool xrToggle;

        //--------------------------------------------------------------------------------------------------------------

        private void UpdateIndicatorPosition()
        {
            if (useCustomCullingDistance)
            {
                _distClippingPlane = runtimeOptions.manualCullingDistance;
            }
            else
            {
                _distClippingPlane = _cameraMain.farClipPlane;
            }
            
            // Function that checks how we should position the indicator
            _indicatorPosition = _cameraMain.WorldToScreenPoint(selectedGo.transform.position);

            // In case the target is on screen and visible
            if (_indicatorPosition.z <= _distClippingPlane && _indicatorPosition.z > 0
                                                           && _indicatorPosition.x <= _canvasRectScreen.rect.width *
                                                           _canvasRectScreen.localScale.x
                                                           && _indicatorPosition.y <= _canvasRectScreen.rect.height *
                                                           _canvasRectScreen.localScale.x
                                                           && _indicatorPosition.x >= 0f & _indicatorPosition.y >= 0f)
            {
                screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
                screenSpaceOptions.farPlaneCulledIndicatorGo.SetActive(false);

                // World Space
                if (!xrToggle) return;
                worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);
                worldSpaceOptions.farPlaneCulledIndicatorGoWorld.SetActive(false);
            }

            // In case the target is out of sight but in front of the screen
            else if (_indicatorPosition.z >= _distClippingPlane && _indicatorPosition.x <=
                                                                _canvasRectScreen.rect.width * _canvasRectScreen.localScale.x
                                                                && _indicatorPosition.y <= _canvasRectScreen.rect.height *
                                                                _canvasRectScreen.localScale.x
                                                                && _indicatorPosition.x >= 0f &
                                                                _indicatorPosition.y >= 0f)
            {
                var position1 = selectedGo.transform.position;
                var position2 = _cameraMain.transform.position;
                SetOutOfSightIndicator(false);

                // Screenspace
                _rectTransformFarPlaneCulledIndicatorScreen.position = new Vector3(_indicatorPosition.x, _indicatorPosition.y, 0f);
                screenSpaceOptions.targetDistanceText.text = ((int)Vector3.Distance(position1, position2)) + " m";
                // World Space
                if (!xrToggle) return;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectWorld, _indicatorPosition, _cameraMain, out var localPoint);
                _rectTransformOnScreenWorld.anchoredPosition = localPoint;
                worldSpaceOptions.targetDistanceTextWorld.text = ((int)Vector3.Distance(position1, position2)) + " m";
            }
            
            else // In case the indicator is not in the camera view
            {
                if (_indicatorPosition.z < 0)
                {
                    _indicatorPosition *= -1;
                    _indicatorPosition = OutOfRangeIndicatorPosition(_indicatorPosition);
                    SetOutOfSightIndicator(true);

                    // Screenspace
                    _rectTransformOffScreen.position = _indicatorPosition;
                    screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.rotation = Quaternion.Euler(RotationOutOfSightScreen(_indicatorPosition));
                    // World Space
                    if (!xrToggle) return;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectWorld, _indicatorPosition, _cameraMain, out var localPoint);
                    _rectTransformOffScreenWorld.localPosition = localPoint;
                    worldSpaceOptions.offScreenTargetIndicatorImageWorld.rectTransform.localRotation = screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.localRotation;
                }

                else
                {
                    _indicatorPosition = OutOfRangeIndicatorPosition(_indicatorPosition);
                    SetOutOfSightIndicator(true);

                    // Screenspace
                    _rectTransformOffScreen.position = _indicatorPosition;
                    screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.rotation = Quaternion.Euler(RotationOutOfSightScreen(_indicatorPosition));
                    // World Space
                    if (!xrToggle) return;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectWorld, _indicatorPosition, _cameraMain, out var localPoint);
                    _rectTransformOffScreenWorld.anchoredPosition = localPoint;
                    worldSpaceOptions.offScreenTargetIndicatorImageWorld.rectTransform.localRotation = screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.localRotation;
                }
            }
        }


        private Vector3 OutOfRangeIndicatorPosition(Vector3 indicatorPosition) // Defines the position of the indicator when the target is out of sight
        {
            var rect = _canvasRectScreen.rect;
            var localScale = _canvasRectScreen.localScale;

            indicatorPosition.z = 0f;

            // Calculate the center of the canvas and subtract from the indicator position to get indicator position relative to the center of the canvas
            var canvasCenter = new Vector3(rect.width * localScale.x / 2f, rect.height * localScale.y / 2f, 0f);
            indicatorPosition -= canvasCenter;

            // Calculate if the vector to the target is more horizontal or vertical
            float divX = (rect.width / 2f) / Mathf.Abs(indicatorPosition.x);
            float divY = (rect.height / 2f) / Mathf.Abs(indicatorPosition.y);

            // Indicator spawns left or right of the screen
            if (divX < divY)
            {
                var angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);

                indicatorPosition.x = Mathf.Sign(indicatorPosition.x) *
                                      (rect.width / 2f - runtimeOptions.outOfSightOffsetHorizontal) * localScale.x;
                indicatorPosition.y = Mathf.Tan(angle * Mathf.Deg2Rad) * indicatorPosition.x;
            }

            
            else // Indicator spawns on top or bottom of the screen
            {
                var angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);
                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) *
                                      (rect.height / 2f - runtimeOptions.outOfSightOffsetVertical) * localScale.y;
                indicatorPosition.x = -Mathf.Tan(angle * Mathf.Deg2Rad) * indicatorPosition.y;
            }

            indicatorPosition += canvasCenter;
            return indicatorPosition;
        }


        private void SetOutOfSightIndicator(bool outOfSight) // Function called to activate and deactivate indicators when marker is out of sight
        {
            if (outOfSight)
            {
                if (screenSpaceOptions.offScreenTargetIndicatorGo.activeSelf == false)
                {
                    screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(true);
                }

                if (screenSpaceOptions.farPlaneCulledIndicatorGo.activeSelf)
                {
                    screenSpaceOptions.farPlaneCulledIndicatorGo.SetActive(false);
                }

                if (!xrToggle) return;
                worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(true);

                if (!xrToggle) return;
                worldSpaceOptions.farPlaneCulledIndicatorGoWorld.SetActive(false);
            }

            else
            {
                if (screenSpaceOptions.offScreenTargetIndicatorGo.activeSelf)
                {
                    screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
                }

                if (screenSpaceOptions.farPlaneCulledIndicatorGo.activeSelf == false)
                {
                    screenSpaceOptions.farPlaneCulledIndicatorGo.SetActive(true);
                }

                if (!xrToggle) return;
                worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);

                if (!xrToggle) return;
                worldSpaceOptions.farPlaneCulledIndicatorGoWorld.SetActive(true);
            }
        }

        private Vector3 RotationOutOfSightScreen(Vector3 indicatorPosition) // Calculate the angle between the indicator and the center of the canvas
        {
            var localScale = _canvasRectScreen.localScale;
            var rect = _canvasRectScreen.rect;
            var canvasCenter = new Vector3(rect.width * localScale.x / 2f, rect.height * localScale.x / 2f, 0f);
            var angle = Vector3.SignedAngle(Vector3.up, indicatorPosition - canvasCenter, Vector3.forward);

            // Rotate the indicator to face the target
            return new Vector3(0f, 0f, angle);
        }

        private void UpdateMouseInteraction() // When left mouse button is pressed selection indicator is initialized 
        {
            if (Input.GetMouseButtonDown(0))
            {
                var rayOver = _cameraMain.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(rayOver, out var hitInfo))
                {
                    selectedGo = hitInfo.transform.gameObject; // Update the selected object
                }
            }
        }
        
//------------------------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            _cameraMain = Camera.main;
            if (_cameraMain is null) throw new Exception("Failed to find the main camera.");
            
            if (!useCustomCullingDistance)
            {
                _distClippingPlane = _cameraMain.farClipPlane;
            }

            if (constantIndicatorSize)
            {
                CanvasScaler cScreen = screenSpaceOptions.canvas.GetComponent<CanvasScaler>();
                cScreen.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                
                CanvasScaler cWorld = worldSpaceOptions.canvasWorld.GetComponent<CanvasScaler>();
                cWorld.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            }

            _canvasRectScreen = screenSpaceOptions.canvas.GetComponent<RectTransform>();
            _rectTransformOffScreen = screenSpaceOptions.offScreenTargetIndicatorGo.GetComponent<RectTransform>();
            _rectTransformFarPlaneCulledIndicatorScreen = screenSpaceOptions.farPlaneCulledIndicatorGo.GetComponent<RectTransform>();
            _canvasRectWorld = worldSpaceOptions.canvasWorld.GetComponent<RectTransform>();
            _rectTransformOffScreenWorld = worldSpaceOptions.offScreenTargetIndicatorGoWorld.GetComponent<RectTransform>();
            _rectTransformOnScreenWorld = worldSpaceOptions.farPlaneCulledIndicatorGoWorld.GetComponent<RectTransform>();
            
            screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
            screenSpaceOptions.farPlaneCulledIndicatorGo.SetActive(false);
            worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);
            worldSpaceOptions.farPlaneCulledIndicatorGoWorld.SetActive(false);
        }

        private void Update()
        {
            if (handleMouseInput)
            {
                UpdateMouseInteraction();
            }

            if (selectedGo != null)
            {
                UpdateIndicatorPosition();
            }
            else
            {
                screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
                screenSpaceOptions.farPlaneCulledIndicatorGo.SetActive(false); 
            }

            if (!xrToggle)
            {
                worldSpaceOptions.canvasWorld.gameObject.SetActive(false);
                _rectTransformOffScreen.localScale = new Vector3(runtimeOptions.manualOffScreenIndicatorSize, runtimeOptions.manualOffScreenIndicatorSize, runtimeOptions.manualOffScreenIndicatorSize);
                _rectTransformFarPlaneCulledIndicatorScreen.localScale = new Vector3(runtimeOptions.manualFarPlaneCulledIndicatorSize, runtimeOptions.manualFarPlaneCulledIndicatorSize, runtimeOptions.manualFarPlaneCulledIndicatorSize);
            }
        }
    }
}