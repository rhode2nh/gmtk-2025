using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wannabuh.FPSController
{
    [RequireComponent(typeof(Rigidbody))]
    public class FPSController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _mouseSensitivity = 20.0f;
        [SerializeField] private float _moveSpeed = 0.1f;
        [SerializeField] private float _acceleration = 2.0f;
        [SerializeField] private float _deceleration = 2.0f;
        
        [Header("Jumping")]
        [SerializeField] private Vector3 _gravity = Physics.gravity;
        [SerializeField] private float _groundCheckRadius;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _jumpForce;
        [SerializeField] private int _maxJumps = 1;
        
        [Header("Camera Tilt Settings (Strafing)")]
        [SerializeField] private float _maxTiltAngle;
        [SerializeField] private float _tiltSpeed;
        
        [Header("Collision Settings")]
        [SerializeField] private float _skinWidth = 0.015f;
        [SerializeField] private int _maxBounces = 5;
        [SerializeField] private float _maxSlopeAngle = 55;
        
        [Header("References")]
        [SerializeField] private CinemachineRecomposer _recomposer;
        [SerializeField] private CinemachineInputAxisController _axisController;
        [SerializeField] private Transform _cameraRig;
        
        private FPSActions _fpsActions;

        // References
        private Rigidbody _rb;

        // Private Variables
        private bool _isGrounded;
        private Vector3 _velocity;
        private Vector2 _moveInput;
        private bool _jumpInput;
        private float _cameraPitch;
        private float _verticalVelocity;
        private float _currentTilt;
        private Bounds _bounds;
        private int _currentJump;
        private int _extraJumps;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Awake()
        {
            _fpsActions = InputManager.Instance.FPSActions;
            _rb = GetComponent<Rigidbody>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _bounds = GetComponent<Collider>().bounds;
            _bounds.Expand(-2 * _skinWidth);
        }

        private void OnEnable()
        {
            _fpsActions.Player.Move.performed += OnMove;
            _fpsActions.Player.Move.Enable();

            _fpsActions.Player.Jump.performed += OnJump;
            _fpsActions.Player.Jump.Enable();
        }

        private void OnDisable()
        {
            _fpsActions.Player.Move.Disable();
            _fpsActions.Player.Jump.Disable();
            _fpsActions.Player.Look.Disable();
        }

        private void Start()
        {
            foreach (var controller in _axisController.Controllers)
            {
                controller.Input.Gain = controller.Name == "Look X (Pan)" ? _mouseSensitivity : -_mouseSensitivity;
            }
        }

        private void LateUpdate()
        {
            Look();
            Tilt();
        }

        private void FixedUpdate()
        {
            Jump();
            Move();
        }

        private Vector3 CollideAndSlide(Vector3 velocity, Vector3 pos, int depth, bool gravityPass, Vector3 initialVelocity)
        {
            if (depth >= _maxBounces)
            {
                return Vector3.zero;
            }

            float distance = velocity.magnitude + _skinWidth;

            RaycastHit hit;
            if (Physics.SphereCast(pos, _bounds.extents.x, velocity.normalized, out hit, distance, _groundMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 snapToSurface = velocity.normalized * (hit.distance - _skinWidth);
                Vector3 leftOver = velocity - snapToSurface;
                float angle = Vector3.Angle(Vector3.up, hit.normal);

                if (snapToSurface.magnitude <= _skinWidth)
                {
                    snapToSurface = Vector3.zero;
                }

                if (angle <= _maxSlopeAngle)
                {
                    if (gravityPass)
                    {
                        return snapToSurface;
                    }
                    leftOver = ProjectAndScale(leftOver, hit.normal);
                }
                else
                {
                    float scale = 1 - Vector3.Dot(
                        new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
                        -new Vector3(initialVelocity.x, 0, initialVelocity.z).normalized
                    );

                    leftOver = ProjectAndScale(leftOver, hit.normal) * scale;
                }

                return snapToSurface + CollideAndSlide(leftOver, pos + snapToSurface, depth + 1, gravityPass, initialVelocity);
            }

            return velocity;
        }

        private Vector3 ProjectAndScale(Vector3 vec, Vector3 normal)
        {
            float magnitude = vec.magnitude;
            vec = Vector3.ProjectOnPlane(vec, normal).normalized;
            return vec *= magnitude;
        }
        
        private void Move()
        {
            Vector3 targetDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
            Vector3 targetVelocity = transform.TransformDirection(targetDirection) * _moveSpeed;

            float accelRate = (targetDirection.sqrMagnitude > 0.01f) ? _acceleration : _deceleration;
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, accelRate * Time.fixedDeltaTime);
            
            if (_velocity.magnitude > _moveSpeed)
            {
                _velocity = _velocity.normalized * _moveSpeed;
            }

            Vector3 _gravityVel = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.fixedDeltaTime;
            
            _rb.MovePosition(_rb.position + CollideAndSlide(_velocity, _rb.position, 0, false, _velocity) + CollideAndSlide(_gravityVel, _rb.position, 0, true, _gravityVel));
        }
        
        private void Jump()
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _groundCheckRadius, _groundMask, QueryTriggerInteraction.Ignore))
            {
                _isGrounded = true;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = 0.0f;
                    _currentJump = 0;
                }

                if (_jumpInput)
                {
					_verticalVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity.y);
                    _currentJump++;
                }
            }
            else
            {
                _isGrounded = false;
                if (_jumpInput && _currentJump < _maxJumps + _extraJumps)
                {
					_verticalVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity.y);
                    _currentJump++;
                }
                _verticalVelocity += _gravity.y * Time.fixedDeltaTime;
            }

            _jumpInput = false;
            _extraJumps = 0;
        }

        private void Look()
        {
            float cameraYaw = _cameraRig.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
        }

        private void Tilt()
        {
            float targetTilt = -(_moveInput.x * _velocity.magnitude / _moveSpeed) * _maxTiltAngle;

            _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, _tiltSpeed * Time.deltaTime);
            _recomposer.Dutch = _currentTilt;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            _jumpInput = Math.Abs(ctx.ReadValue<float>() - 1.0f) < 0.001f ? true : false;
        }
        
        public void SetExtraJumps(int maxJumps)
        {
            Debug.Log(maxJumps);
            _extraJumps += maxJumps;
        }
    }
}
