using UnityEditor.UIElements;
using UnityEngine;

namespace XRGiS_Project.ET_TestScene.Scripts.Camera_Control
{
    public class LODCameraControl : MonoBehaviour {
        
        [Tooltip("The rotation acceleration, in degrees / second")]
        [SerializeField] private Vector2 acceleration = new Vector2(1000f, 1000f);
        [Tooltip("A multiplier to the input. Describes the maximum speed in degrees / second. To flip vertical rotation, set Y to a negative value")]
        [SerializeField] private Vector2 sensitivityRotation = new Vector2(1000f, -1000f);
        [Tooltip("The maximum angle from the horizon the player can rotate, in degrees")]
        [SerializeField] private float maxVerticalAngleFromHorizon = 90f;
        [Tooltip("The period to wait until resetting the input value. Set this as low as possible, without encountering stuttering")]
        [SerializeField] private float inputLagPeriod = 0.005f;
        
        [Tooltip("The sensitivity of the mouse")]
        [SerializeField] private float sensitivity = .05f;
        [Tooltip("The speed of the camera")]
        [SerializeField] private float speed = 4;
        private float _savedSpeed;
        

        private Vector2 _velocity; // The current rotation velocity, in degrees
        private Vector2 _rotation; // The current rotation, in degrees
        private Vector2 _lastInputEvent; // The last received non-zero input value
        private float _inputLagTimer; // The time since the last received non-zero input value

        // When this component is enabled, we need to reset the state
        // and figure out the current rotation
        private void OnEnable() {
            // Reset the state
            _velocity = Vector2.zero;
            _inputLagTimer = 0;
            _lastInputEvent = Vector2.zero;

            // Calculate the current rotation by getting the gameObject's local euler angles
            Vector3 euler = transform.localEulerAngles;
            // Euler angles range from [0, 360), but we want [-180, 180)
            if(euler.x >= 180) {
                euler.x -= 360;
            }
            euler.x = ClampVerticalAngle(euler.x);
            // Set the angles here to clamp the current rotation
            transform.localEulerAngles = euler;
            // Rotation is stored as (horizontal, vertical), which corresponds to the euler angles
            // around the y (up) axis and the x (right) axis
            _rotation = new Vector2(euler.y, euler.x);
        }

        private float ClampVerticalAngle(float angle) {
            return Mathf.Clamp(angle, -maxVerticalAngleFromHorizon, maxVerticalAngleFromHorizon);
        }

        private Vector2 GetInput() {
            // Add to the lag timer
            _inputLagTimer += Time.deltaTime;
            // Get the input vector. This can be changed to work with the new input system or even touch controls
            Vector2 input = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );
            // Sometimes at fast framerates, Unity will not receive input events every frame, which results
            // in zero values being given above. This can cause stuttering and make it difficult to fine
            // tune the acceleration setting. To fix this, disregard zero values. If the lag timer has passed the
            // lag period, we can assume that the user is not giving any input, so we actually want to set
            // the input value to zero at that time.
            // Thus, save the input value if it is non-zero or the lag timer is met
            if((Mathf.Approximately(0, input.x) && Mathf.Approximately(0, input.y)) == false || _inputLagTimer >= inputLagPeriod) {
                _lastInputEvent = input;
                _inputLagTimer = 0;
            }

            
            return _lastInputEvent;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update() {
            // The wanted velocity is the current input scaled by the sensitivity
            // This is also the maximum velocity
            Vector2 wantedVelocity = GetInput() * sensitivityRotation;
            
            // Calculate new rotation
            _velocity = new Vector2(
                Mathf.MoveTowards(_velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime),
                Mathf.MoveTowards(_velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime));
            _rotation += _velocity * Time.deltaTime;
            _rotation.y = ClampVerticalAngle(_rotation.y);

            // Convert the rotation to euler angles
            transform.localEulerAngles = new Vector3(_rotation.y, _rotation.x, 0);
            
            // WASD movement
            transform.Translate(Input.GetAxisRaw("Horizontal") * speed * sensitivity, 0,Input.GetAxisRaw("Vertical") * speed * sensitivity);
            
            // Lock Mouse on keystroke
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                acceleration=Vector2.zero;
                _savedSpeed = speed;
                speed = 0;
            }
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                acceleration= new Vector2(1000f, 1000f);

                if (speed == 0)
                {
                    speed = _savedSpeed;    
                }

            }
            
            // Upward movement with e
            if(Input.GetKey(KeyCode.E))
            {
                transform.Translate(0, speed * sensitivity, 0);
            }
            
            // Downward movement with q
            if(Input.GetKey(KeyCode.Q))
            {
                transform.Translate(0, -speed * sensitivity, 0);
            }
        }
    }
}
