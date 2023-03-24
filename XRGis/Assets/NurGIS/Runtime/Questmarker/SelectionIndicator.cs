using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LODCesium.Terranigma.Runtime.Questmarker
{
    public class SelectionIndicator : MonoBehaviour
    {
        [Serializable]
        public class GeneralOptions
        {
            [Tooltip("The prefab of the indicator when in view")] [SerializeField]
            public GameObject indicatorPrefab;

            [Tooltip("The relative size of the indicator")] [SerializeField]
            public float indicatorSize = 0.01f;

            [Tooltip("The main camera")] [SerializeField]
            public Camera mainCamera;

            [Tooltip("Highlight the selected game object")] [SerializeField]
            public bool highlightSystem;
        }

        [Tooltip("The selected game object")] [SerializeField]
        public GameObject selectedGo;

        private GameObject _selectedGo;
        private GameObject _indicator;
        private float _distClippingPlane;

        [Serializable]
        public class ScreenSpaceOptions
        {
            [Tooltip("The indicator of the selected game object if it is out of sight")]
            public Image offScreenTargetIndicatorImage;

            [Tooltip("The GO of the indicator of the selected game object if it is out of sight")] [SerializeField]
            public GameObject offScreenTargetIndicatorGo;

            [Tooltip("The indicator of the selected game object if it is culled")] [SerializeField]
            public GameObject targetIndicatorGo;

            [Tooltip("The canvas to draw the indicator on")] [SerializeField]
            public Canvas canvas;

            [Tooltip("The text of the distance between the player and the selected game object")] [SerializeField]
            public TextMeshProUGUI targetDistanceText;

            [Tooltip("The offset of the indicator from the left and right edge of the screen")] [SerializeField]
            public float outOfSightOffsetHorizontal = 20f;

            [Tooltip("The offset of the indicator from the top and bot edge of the screen")] [SerializeField]
            public float outOfSightOffsetVertical = 20f;
        }

        private RectTransform _canvasRect;
        private RectTransform _rectTransformOffScreen;
        private RectTransform _rectTransformOnScreen;
        private Vector3 _indicatorPosition;

        [Serializable]
        public class WorldSpaceOptions
        {
            [Tooltip("The indicator of the selected game object if it is out of sight")]
            public Image offScreenTargetIndicatorImageWorld;

            [Tooltip("The GO of the indicator of the selected game object if it is out of sight")] [SerializeField]
            public GameObject offScreenTargetIndicatorGoWorld;

            [Tooltip("The indicator of the selected game object if it is culled")] [SerializeField]
            public GameObject targetIndicatorGoWorld;

            [Tooltip("The canvas to draw the indicator on")] [SerializeField]
            public Canvas canvasWorld;

            [Tooltip("The text of the distance between the player and the selected game object")] [SerializeField]
            public TextMeshProUGUI targetDistanceTextWorld;
        }

        private RectTransform _canvasRectWorld;
        private RectTransform _rectTransformOffScreenWorld;
        private RectTransform _rectTransformOnScreenWorld;
        
        private Color _savedColor;

        [SerializeField] public GeneralOptions generalOptions = new();
        [SerializeField] public ScreenSpaceOptions screenSpaceOptions = new();
        [SerializeField] public WorldSpaceOptions worldSpaceOptions = new();

        [Tooltip("Mouse Selection Key")] [SerializeField]
        public bool handleMouseInput = true;

        [Tooltip("Toggle XR")] [SerializeField]
        public bool xrToggle;

        //--------------------------------------------------------------------------------------------------------------

        private GameObject FindActiveLOD(GameObject go) // Finds the active LOD of the selected game object
        {
            GameObject activeLODGameObject = null;
            GameObject child0 = go.transform.GetChild(1).gameObject; // _UMS_LODs_

            foreach (Transform child in child0.transform) // gets active LOD game object
            {
                if (child.gameObject.activeSelf)
                {
                    activeLODGameObject = child.gameObject;
                    break;
                }
            }
            
            if (activeLODGameObject != null)
            {
                var meshContainerGo = activeLODGameObject.transform.GetChild(0).gameObject; // 000_static_default
                return meshContainerGo;
            }
            return null;
        }

        private void ChangeAllColors(GameObject go, Color color)
        {
            if (_savedColor != null)
            {
                GameObject child0 = go.transform.GetChild(1).gameObject; // _UMS_LODs_

                foreach (Transform child1 in child0.transform) // gets active LOD game object
                {
                    var child2 = child1.gameObject.transform.GetChild(0).gameObject; // 000_static_default
                    child2.GetComponent<Renderer>().material.color = color;
                }
            }
        }

        private void UpdateIndicatorPosition()
        {
            // Function that checks how we should position the indicator
            _indicatorPosition = generalOptions.mainCamera.WorldToScreenPoint(_selectedGo.transform.position);

            // In case the target is on screen and visible
            if (_indicatorPosition.z <= _distClippingPlane && _indicatorPosition.z > 0
                                                           && _indicatorPosition.x <= _canvasRect.rect.width *
                                                           _canvasRect.localScale.x
                                                           && _indicatorPosition.y <= _canvasRect.rect.height *
                                                           _canvasRect.localScale.x
                                                           && _indicatorPosition.x >= 0f & _indicatorPosition.y >= 0f
                                                           && generalOptions.highlightSystem == false)
            {
                // Screenspace
                var position = _indicator.transform.position;
                var size = (generalOptions.mainCamera.transform.position - position).magnitude;
                _indicator.transform.localScale = new Vector3(size * generalOptions.indicatorSize,
                    size * generalOptions.indicatorSize, size * generalOptions.indicatorSize);

                screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
                screenSpaceOptions.targetIndicatorGo.SetActive(false);

                // World Space
                if (!xrToggle) return;
                worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);
                worldSpaceOptions.targetIndicatorGoWorld.SetActive(false);
            }

            // In case the target is out of sight but in front of the screen
            else if (_indicatorPosition.z >= _distClippingPlane && _indicatorPosition.x <=
                                                                _canvasRect.rect.width * _canvasRect.localScale.x
                                                                && _indicatorPosition.y <= _canvasRect.rect.height *
                                                                _canvasRect.localScale.x
                                                                && _indicatorPosition.x >= 0f &
                                                                _indicatorPosition.y >= 0f)
            {
                var position1 = _selectedGo.transform.position;
                var position2 = generalOptions.mainCamera.transform.position;
                SetOutOfSightIndicator(false);

                // Screenspace
                _rectTransformOnScreen.position = new Vector3(_indicatorPosition.x, _indicatorPosition.y, 0f);
                screenSpaceOptions.targetDistanceText.text = ((int)Vector3.Distance(position1, position2)) + " m";
                // World Space
                if (!xrToggle) return;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectWorld, _indicatorPosition,
                    generalOptions.mainCamera, out var localPoint);
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
                    screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.rotation =
                        Quaternion.Euler(RotationOutOfSightScreen(_indicatorPosition));
                    // World Space
                    if (!xrToggle) return;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectWorld, _indicatorPosition,
                        generalOptions.mainCamera, out var localPoint);
                    _rectTransformOffScreenWorld.localPosition = localPoint;
                    worldSpaceOptions.offScreenTargetIndicatorImageWorld.rectTransform.localRotation =
                        screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.localRotation;
                }

                else
                {
                    _indicatorPosition = OutOfRangeIndicatorPosition(_indicatorPosition);
                    SetOutOfSightIndicator(true);

                    // Screenspace
                    _rectTransformOffScreen.position = _indicatorPosition;
                    screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.rotation =
                        Quaternion.Euler(RotationOutOfSightScreen(_indicatorPosition));
                    // World Space
                    if (!xrToggle) return;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectWorld, _indicatorPosition,
                        generalOptions.mainCamera, out var localPoint);
                    _rectTransformOffScreenWorld.anchoredPosition = localPoint;
                    worldSpaceOptions.offScreenTargetIndicatorImageWorld.rectTransform.localRotation =
                        screenSpaceOptions.offScreenTargetIndicatorImage.rectTransform.localRotation;
                }
            }
        }


        private Vector3 OutOfRangeIndicatorPosition(Vector3 indicatorPosition) // Defines the position of the indicator when the target is out of sight
        {
            var rect = _canvasRect.rect;
            var localScale = _canvasRect.localScale;

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
                                      (rect.width / 2f - screenSpaceOptions.outOfSightOffsetHorizontal) * localScale.x;
                indicatorPosition.y = Mathf.Tan(angle * Mathf.Deg2Rad) * indicatorPosition.x;
            }

            
            else // Indicator spawns on top or bottom of the screen
            {
                var angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);
                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) *
                                      (rect.height / 2f - screenSpaceOptions.outOfSightOffsetVertical) * localScale.y;
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

                if (screenSpaceOptions.targetIndicatorGo.activeSelf)
                {
                    screenSpaceOptions.targetIndicatorGo.SetActive(false);
                }

                if (!xrToggle) return;
                worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(true);

                if (!xrToggle) return;
                worldSpaceOptions.targetIndicatorGoWorld.SetActive(false);
            }

            else
            {
                if (screenSpaceOptions.offScreenTargetIndicatorGo.activeSelf)
                {
                    screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
                }

                if (screenSpaceOptions.targetIndicatorGo.activeSelf == false)
                {
                    screenSpaceOptions.targetIndicatorGo.SetActive(true);
                }

                if (!xrToggle) return;
                worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);

                if (!xrToggle) return;
                worldSpaceOptions.targetIndicatorGoWorld.SetActive(true);
            }
        }

        private Vector3 RotationOutOfSightScreen(Vector3 indicatorPosition) // Calculate the angle between the indicator and the center of the canvas
        {
            var localScale = _canvasRect.localScale;
            var rect = _canvasRect.rect;
            var canvasCenter = new Vector3(rect.width * localScale.x / 2f, rect.height * localScale.x / 2f, 0f);
            var angle = Vector3.SignedAngle(Vector3.up, indicatorPosition - canvasCenter, Vector3.forward);

            // Rotate the indicator to face the target
            return new Vector3(0f, 0f, angle);
        }

        private void UpdateMouseInteraction() // When left mouse button is pressed selection indicator is initialized 
        {
            if (Input.GetMouseButtonDown(0))
            {
                var rayOver = generalOptions.mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(rayOver, out var hitInfo))
                {
                    if (generalOptions.highlightSystem)
                    {
                        if (_selectedGo != null) // Revert the colors of the previously selected object
                        {
                            ChangeAllColors(_selectedGo, _savedColor);
                        }
                        
                        _selectedGo = hitInfo.transform.gameObject; // Update the selected object
                        selectedGo = _selectedGo;
                        
                        _savedColor = FindActiveLOD(_selectedGo).GetComponent<Renderer>().material.color; // Save the color of the selected object
                        ChangeAllColors(_selectedGo, Color.white); // Update the color of the selected object
                    }

                    else
                    {
                        _selectedGo = hitInfo.transform.gameObject;
                        selectedGo = _selectedGo; 
                        InitializeHighlighter(_selectedGo);
                    }
                }
            }
        }

        private void InitializeHighlighter(GameObject go)
        {
            var rend = go.GetComponentInChildren<Renderer>();
            var diameter = rend.bounds.size.x;
            var position = go.transform.position;
            
            if (_indicator == null) // Spawn indicator if there is no indicator yet
            {
                _indicator = Instantiate(generalOptions.indicatorPrefab, new Vector3(position.x, (float)(position.y + 0.25 * diameter), position.z), Quaternion.identity);
                _indicator.transform.localScale = new Vector3(diameter * 0.1f, diameter * 0.1f, diameter * 0.1f);
                
            }

            else // If there is already an indicator, move it to the selected object
            {
                Destroy(_indicator);
                _indicator = Instantiate(generalOptions.indicatorPrefab, new Vector3(position.x, (float)(position.y + 0.25 * diameter), position.z), Quaternion.identity);
                _indicator.transform.localScale = new Vector3(diameter * 0.1f, diameter * 0.1f, diameter * 0.1f);
            }
        }

//------------------------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            _distClippingPlane = generalOptions.mainCamera.farClipPlane;
            _canvasRect = screenSpaceOptions.canvas.GetComponent<RectTransform>();
            _rectTransformOffScreen = screenSpaceOptions.offScreenTargetIndicatorGo.GetComponent<RectTransform>();
            _rectTransformOnScreen = screenSpaceOptions.targetIndicatorGo.GetComponent<RectTransform>();
            _canvasRectWorld = worldSpaceOptions.canvasWorld.GetComponent<RectTransform>();
            _rectTransformOffScreenWorld = worldSpaceOptions.offScreenTargetIndicatorGoWorld.GetComponent<RectTransform>();
            _rectTransformOnScreenWorld = worldSpaceOptions.targetIndicatorGoWorld.GetComponent<RectTransform>();
            
            screenSpaceOptions.offScreenTargetIndicatorGo.SetActive(false);
            screenSpaceOptions.targetIndicatorGo.SetActive(false);
            worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);
            worldSpaceOptions.targetIndicatorGoWorld.SetActive(false);
        }

        private void Update()
        {
            if (handleMouseInput)
                UpdateMouseInteraction();
            else
            {
                _selectedGo = selectedGo;
                InitializeHighlighter(_selectedGo);
            }

            if (_selectedGo != null)
            {
                UpdateIndicatorPosition();
            }
            
            if (xrToggle) return;
            worldSpaceOptions.offScreenTargetIndicatorGoWorld.SetActive(false);
            worldSpaceOptions.targetIndicatorGoWorld.SetActive(false);
        }
    }
}