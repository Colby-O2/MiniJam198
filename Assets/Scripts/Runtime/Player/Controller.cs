using MJ198.Math;
using MJ198.MonoSystems;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.Windows;

namespace MJ198.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Controller : MonoBehaviour
    {
        private const float GRAVITY = -9.8f;

        public enum PlayerState
        {
            Grounded,
            Airborne,
            Sliding,
            WallRunning
        }

        [SerializeField, ReadOnly] private PlayerState _state = PlayerState.Airborne;

        [SerializeField] private MovementSettings _settings;
        [SerializeField] private CharacterController _controller;

        [Header("Flags")]
        [SerializeField, ReadOnly] private bool _isSprinting;
        [SerializeField, ReadOnly] private bool _justWallJumped;

        [Header("Movement")]
        [SerializeField, ReadOnly] private Vector3 _velocity;
        [SerializeField, ReadOnly] private Vector2 _inputMove;
        private Vector2 _horizontalVelocity;

        [Header("Slide")]
        [SerializeField, ReadOnly] private float _slideTimer;
        [SerializeField, ReadOnly] private Vector3 _slideDirection;

        [Header("WallRun")]
        [SerializeField, ReadOnly] private float _wallRunCooldown;
        [SerializeField, ReadOnly] private Vector3 _wallNormal;
        [SerializeField, ReadOnly] private float _wallRunTimer;
        [SerializeField, ReadOnly] private Vector3 _wallForward;

        [Header("WallJump")]
        [SerializeField, ReadOnly] private float _wallJumpCooldown;

        private IInputMonoSystem _input;
        private bool _jumpRequested;

        public bool IsSliding() => _state == PlayerState.Sliding;
        public bool CantSlide() => _state != PlayerState.Grounded || !_isSprinting || IsSliding();

        public PlayerState State => _state;
        public Vector3 CurrentWallNormal => _wallNormal;

        private float Gravity => GRAVITY * _settings.GravityMul;
        private float Speed => (_isSprinting ? _settings.RunningMul : 1f) * _settings.Speed;
        private bool HasForwardInput => _inputMove.y > 0.1f;

        private void Awake()
        {
            _input = GameManager.GetMonoSystem<IInputMonoSystem>();
        }

        private void OnEnable()
        {
            _input.JumpAction.AddListener(RequestJump);
            _input.SlideAction.AddListener(StartSlide);
        }

        private void OnDisable()
        {
            _input.JumpAction.RemoveListener(RequestJump);
            _input.SlideAction.RemoveListener(StartSlide);
        }

        private void Update()
        {
            _wallJumpCooldown -= Time.deltaTime;
            _wallRunCooldown -= Time.deltaTime;
            if (_wallJumpCooldown <= 0f) _justWallJumped = false;
            _inputMove = Vector2.ClampMagnitude(_input.RawMovement, 1f);
            _isSprinting = _input.SprintHeld;

            if (_jumpRequested)
            {
                HandleJump();
                _jumpRequested = false;
            }

            UpdatePlayerState();
            UpdateMovement();
            ApplyGravity();
            MoveController();
        }

        private void RequestJump() => _jumpRequested = true;

        private void HandleJump()
        {
            if (_state == PlayerState.WallRunning)
            {
                WallJump();
                return;
            }

            if (_controller.isGrounded || _state == PlayerState.Sliding)
            {
                _velocity.y = Mathf.Sqrt(_settings.JumpPower * -3f * Gravity);
                _state = PlayerState.Airborne;
                _slideDirection = Vector3.zero;
            }
        }

        private void UpdatePlayerState()
        {
            if (_state == PlayerState.Sliding) UpdateSliding();
            else if (_state == PlayerState.WallRunning) UpdateWallRunning();
            else _state = _controller.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;
        }

        private void UpdateMovement()
        {
            switch (_state)
            {
                case PlayerState.Grounded: UpdateGrounded(); break;
                case PlayerState.Airborne: UpdateAirborne(); break;
            }
        }

        private void UpdateGrounded()
        {
            Vector3 target3D = transform.TransformDirection(new Vector3(_inputMove.x, 0f, _inputMove.y)) * Speed;
            Vector2 target2D = new Vector2(target3D.x, target3D.z);
            float accel = _inputMove.sqrMagnitude > 0.01f ? _settings.GroundAcceleration : _settings.GroundDeceleration;
            _horizontalVelocity = Vector2.Lerp(_horizontalVelocity, target2D, accel * Time.deltaTime);
            _velocity.x = _horizontalVelocity.x;
            _velocity.z = _horizontalVelocity.y;
            if (!_controller.isGrounded) _state = PlayerState.Airborne;
        }

        private void UpdateAirborne()
        {
            if (_controller.isGrounded)
            {
                _state = PlayerState.Grounded;
                _velocity.y = -4f;
                return;
            }

            if (_wallJumpCooldown <= 0f)
            {
                Vector3 target3D = transform.TransformDirection(new Vector3(_inputMove.x, 0f, _inputMove.y)) * Speed;
                Vector2 target2D = new Vector2(target3D.x, target3D.z);
                _horizontalVelocity = Vector2.Lerp(_horizontalVelocity, target2D, _settings.AirControl * Time.deltaTime);
            }

            _velocity.x = _horizontalVelocity.x;
            _velocity.z = _horizontalVelocity.y;

            if (_wallJumpCooldown <= 0 && HasForwardInput && CheckForWall(out var normal) && ValidWallRunAngle(normal))
                StartWallRun(normal);
        }

        private void UpdateSliding()
        {
            _slideTimer -= Time.deltaTime;
            _slideDirection *= 1f / (1f + _settings.SlideFriction * Time.deltaTime);
            if (_slideTimer <= 0f || _slideDirection.magnitude < 0.1f || !_controller.isGrounded)
            {
                _state = _controller.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;
                _slideDirection = Vector3.zero;
            }
        }

        private void UpdateWallRunning()
        {
            _wallRunTimer -= Time.deltaTime;
            if (_wallRunTimer <= 0 || !CheckForWall(out var normal) || RawMovementDirection().magnitude == 0.0f)
            {
                _state = PlayerState.Airborne;
                _wallRunCooldown = _settings.WallRunningCooldownTime;
                return;
            }

            _velocity.x = _wallForward.x * Speed * _settings.WallRunSpeedMul;
            _velocity.z = _wallForward.z * Speed * _settings.WallRunSpeedMul;
        }

        private void ApplyGravity()
        {
            if (_state != PlayerState.WallRunning)
                _velocity.y = Mathf.MoveTowards(_velocity.y, -50f, -Gravity * Time.deltaTime);
            if (_controller.isGrounded && _state == PlayerState.Grounded && _velocity.y < 0f)
                _velocity.y = -4f;
        }

        private void MoveController()
        {
            Vector3 move = _state == PlayerState.Sliding ? _slideDirection + Vector3.up * _velocity.y : _velocity;
            _controller.Move(move * Time.deltaTime);
        }

        private void StartSlide()
        {
            if (CantSlide()) return;
            _state = PlayerState.Sliding;
            _slideTimer = _settings.SlideDuration;
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            _slideDirection = forward * _settings.SlideSpeedMul * _settings.Speed;
        }

        private Vector3 RawMovementDirection()
        {
            return new Vector3(_input.RawMovement.y, 0f, _input.RawMovement.x).normalized;
        }

        private void WallJump()
        {
            _justWallJumped = true;
            _wallJumpCooldown = _settings.WallJumpLockTime;
            _state = PlayerState.Airborne;
            _velocity += _wallNormal * _settings.WallJumpSideForce +
                        Vector3.up * _settings.WallJumpUpForce +
                        ((Mathf.Abs(Vector3.Dot(RawMovementDirection(), _wallForward)) < 0.2f) ? _wallForward : _wallNormal) * _settings.WallJumpForwardBoost;
            _horizontalVelocity = new Vector2(_velocity.x, _velocity.z);
            _wallRunCooldown = _settings.WallRunningCooldownTime;
        }

        private void StartWallRun(Vector3 normal)
        {
            if (_wallRunCooldown > 0f || _velocity.SetY(0f).magnitude < _settings.WallRunMinSpeed) return;
            _state = PlayerState.WallRunning;
            _wallNormal = normal;
            _wallRunTimer = _settings.WallRunDuration;
            _velocity.y = 0f;
            _wallForward =Vector3.Cross(_wallNormal, Vector3.up);
            if (Vector3.Dot(_wallForward, transform.forward) < 0) _wallForward = -_wallForward;
        }

        private bool CheckForWall(out Vector3 normal)
        {
            normal = Vector3.zero;
            Vector3 origin = transform.position + Vector3.up * (_controller.height * 0.5f);
            Collider[] hits = Physics.OverlapSphere(origin, _settings.WallDetectionDistance, ~LayerMask.GetMask("Player"));
            if (hits.Length > 0)
            {
                Collider hit = hits[0];
                normal =  (transform.position - hit.ClosestPoint(transform.position)).normalized;
                return true;
            }
 
            return false;
        }

        private bool ValidWallRunAngle(Vector3 normal)
        {
            Vector3 f = Vector3.Cross(normal, Vector3.up);
            return Mathf.Abs(Vector3.Dot(f, transform.forward)) > 0.2f;
        }
    }
}
