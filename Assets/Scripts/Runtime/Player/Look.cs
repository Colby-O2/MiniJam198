using MJ198.Math;
using MJ198.MonoSystems;
using MJ198.Player;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using System.Threading;
using UnityEngine;

namespace MJ198.Player
{
    public class Look : MonoBehaviour
    {
        [SerializeField] private MovementSettings _settings;
        [SerializeField] private Transform _playerBody;

        [SerializeField, ReadOnly] private float _pitch = 0f;
        [SerializeField, ReadOnly] private float _yaw = 0f;

        [SerializeField, ReadOnly] private Vector3 _camDefaultLocalPos;
        [SerializeField, ReadOnly] private Vector3 _camSlideTarget;

        private Controller _pc;
        private IInputMonoSystem _input;

        private float _headBobTimer;
        private float _currentFOV;
        private float _targetFOV;
        private float _currentTilt;
        private float _targetTilt;

        private void StartSlide()
        {
            if (_pc.CantSlide()) return;
            _camSlideTarget = _camDefaultLocalPos + new Vector3(0, _settings.SlideHeadDrop, _settings.SlideHeadForward);
        }

        private void ProcessCameraSlide()
        {
            Vector3 target = _pc.IsSliding() ? _camSlideTarget : _camDefaultLocalPos;
            UpdatePosition(Vector3.Lerp(transform.localPosition, target,
                (_pc.IsSliding() ? _settings.SlideHeadSpeed : _settings.SlideHeadReturnSpeed) * Time.deltaTime));
        }

        private void ProcessHead()
        {
            _pitch = transform.localEulerAngles.AngleAs180().x;
            _pitch -= (_settings.InvertLookY ? -1 : 1) * _settings.Sensitivity * _input.RawLook.y;
            _pitch = Mathf.Clamp(_pitch, _settings.YLookLimit.x, _settings.YLookLimit.y);
            transform.localEulerAngles = transform.localEulerAngles.SetX(_pitch);
        }

        private void ProcessBody()
        {
            _yaw = _playerBody.transform.localEulerAngles.y;
            _yaw += (_settings.InvertLookX ? -1 : 1) * _settings.Sensitivity * _input.RawLook.x;
            _playerBody.transform.localEulerAngles = _playerBody.transform.localEulerAngles.SetY(_yaw);
        }

        private void ProcessHeadBob()
        {
            if (_pc.State == Controller.PlayerState.WallRunning || _pc.State == Controller.PlayerState.Airborne) return;

            Vector3 velocity = new Vector3(_input.RawMovement.x, 0, _input.RawMovement.y);
            float speed = velocity.magnitude;
            if (speed > 0.01f)
            {
                float bobMultiplier = 1f + (_input.SprintHeld ? _settings.HeadBobRunMultiplier : 0f);
                _headBobTimer += Time.deltaTime * _settings.HeadBobFrequency * speed;
                float bobX = Mathf.Sin(_headBobTimer) * _settings.HeadBobAmplitudeX * bobMultiplier;
                float bobY = Mathf.Cos(_headBobTimer * 2f) * _settings.HeadBobAmplitudeY * bobMultiplier;
                transform.localPosition += new Vector3(bobX, bobY, 0);
            }
        }

        private void ProcessFOV()
        {
            _targetFOV = (_pc.IsSliding() || _input.SprintHeld) ? _settings.FOVKickAmount : _settings.DefaultFOV;
            _currentFOV = Mathf.Lerp(_currentFOV, _targetFOV, _settings.FOVKickSpeed * Time.deltaTime);
            Camera.main.fieldOfView = _currentFOV;
        }

        private void ProcessTilt()
        {
            if (_pc.IsSliding())
            {
                _targetTilt = _settings.SlideTiltAngle;
            }
            else if (_pc.State == Controller.PlayerState.WallRunning)
            {
                Vector3 wallNormal = _pc.CurrentWallNormal;
                Vector3 right = _playerBody.right;
                float dir = Vector3.Dot(right, wallNormal) > 0 ? -1 : 1;
                _targetTilt = _settings.WallRunTiltAngle * dir;
            }
            else
            {
                _targetTilt = 0f;
            }

            _currentTilt = Mathf.Lerp(_currentTilt, _targetTilt, _settings.TiltSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, _currentTilt);
        }

        private void UpdateRotation()
        {
            ProcessHead();
            ProcessBody();
        }

        private void UpdatePosition(Vector3 target)
        {
            transform.localPosition = target;
        }

        private void Awake()
        {
            _pc = _playerBody.GetComponent<Controller>();
            _input = GameManager.GetMonoSystem<IInputMonoSystem>();
            _camDefaultLocalPos = transform.localPosition;
            _currentFOV = _settings.DefaultFOV;
        }

        private void Update()
        {
            UpdateRotation();
            ProcessCameraSlide();
            ProcessHeadBob();
            ProcessFOV();
            ProcessTilt();
        }

        private void OnEnable()
        {
            _input.SlideAction.AddListener(StartSlide);
        }

        private void OnDisable()
        {
            _input.SlideAction.RemoveListener(StartSlide);
        }
    }
}
