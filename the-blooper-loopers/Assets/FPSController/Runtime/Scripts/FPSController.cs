using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wannabuh.FPSController
{
    [RequireComponent(typeof(Rigidbody))]
    public class FPSController : MonoBehaviour
    {
        [SerializeField] private float _sensitivity = 20.0f;
        [SerializeField] private float _moveSpeed = 0.1f;
        [SerializeField] private float _acceleration = 2.0f;
        [SerializeField] private float _deceleration = 2.0f;
        [SerializeField] private Transform _cameraRig;
        [SerializeField] private CinemachineInputAxisController _axisController;
        [SerializeField] private Vector3 _gravity = Physics.gravity;
        [SerializeField] private float _groundCheckRadius;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private bool _isGrounded;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _maxTiltAngle;
        [SerializeField] private float _tiltSpeed;
        [SerializeField] private CinemachineRecomposer _recomposer;
        [SerializeField] private float _skinWidth = 0.015f;
        [SerializeField] private int _maxBounces = 5;
        [SerializeField] private float _maxSlopeAngle = 55;
        
        private FPSActions _fpsActions;

        // private Camera _camera;
        private Rigidbody _rb;

        private Vector3 _velocity;
        private Vector2 _moveInput;
        private bool _jumpInput;
        private float _cameraPitch;
        private float _verticalVelocity;
        private float _currentTilt;
        private Bounds _bounds;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _fpsActions = new FPSActions();
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
                controller.Input.Gain = controller.Name == "Look X (Pan)" ? _sensitivity : -_sensitivity;
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
            if (Physics.SphereCast(pos, _bounds.extents.x, velocity.normalized, out hit, distance, _groundMask))
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

                    if (_isGrounded && !gravityPass)
                    {
                        leftOver = ProjectAndScale(
                            new Vector3(leftOver.x, 0, leftOver.z),
                            new Vector3(hit.normal.x, 0, hit.normal.z)
                        ).normalized;
                        leftOver *= scale;
                    }
                    else
                    {
                        leftOver = ProjectAndScale(leftOver, hit.normal) * scale;
                    }
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
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _groundCheckRadius, _groundMask))
            {
                _isGrounded = true;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2.0f;
                }

                if (_jumpInput)
                {
					_verticalVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity.y);
                }
            }
            else
            {
                _isGrounded = false;
                _verticalVelocity += _gravity.y * Time.fixedDeltaTime;
            }
        }

        private void Look()
        {
            float cameraYaw = _cameraRig.eulerAngles.y;
            Debug.Log(cameraYaw);
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
    }
}
