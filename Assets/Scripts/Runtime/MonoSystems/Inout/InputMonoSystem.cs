using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MJ198.MonoSystems
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputMonoSystem : MonoBehaviour, IInputMonoSystem
    {
        [SerializeField] private PlayerInput _input;

        private IUIMonoSystem _ui;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _slideAction;

        public UnityEvent JumpAction { get; private set; }
        public UnityEvent SprintAction { get; private set; }
        public UnityEvent SlideAction { get; private set; }

        public bool SprintHeld { get; set; }
        public bool SlideHeld { get; set; }
        public Vector2 RawMovement { get; private set; }
        public Vector2 RawLook { get; private set; }

        private void HandleMoveAction(InputAction.CallbackContext e)
        {
            RawMovement = e.ReadValue<Vector2>();
        }

        private void HandleLookAction(InputAction.CallbackContext e)
        {
            RawLook = e.ReadValue<Vector2>();
        }

        private void HandleJumpAction(InputAction.CallbackContext e)
        {
            JumpAction.Invoke();
        }

        private void HandleSprintAction(InputAction.CallbackContext e)
        {
            SprintHeld = true;
            SprintAction.Invoke();
        }

        private void HandleSprintCanceledAction(InputAction.CallbackContext e)
        {
            SprintHeld = false;
        }

        private void HandleSlideAction(InputAction.CallbackContext e)
        {
            SlideHeld = true;
            SlideAction.Invoke();
        }

        private void HandleSlideCanceledAction(InputAction.CallbackContext e)
        {
            SlideHeld = false;
        }

        private void Awake()
        {
            if (!_input) _input = GetComponent<PlayerInput>();

            JumpAction           = new UnityEvent();
            SprintAction         = new UnityEvent();
            SlideAction          = new UnityEvent();


            _moveAction          = _input.actions["Move"];
            _lookAction          = _input.actions["Look"];
            _jumpAction          = _input.actions["Jump"];
            _sprintAction        = _input.actions["Sprint"];
            _slideAction         = _input.actions["Slide"];

            _moveAction.performed       += HandleMoveAction;
            _lookAction.performed       += HandleLookAction;
            _jumpAction.performed       += HandleJumpAction;
            _sprintAction.performed     += HandleSprintAction;
            _sprintAction.canceled      += HandleSprintCanceledAction;
            _slideAction.performed      += HandleSlideAction;
            _slideAction.canceled       += HandleSlideCanceledAction;
        }

        private void OnDestroy()
        {
            _moveAction.performed       -= HandleMoveAction;
            _lookAction.performed       -= HandleLookAction;
            _jumpAction.performed       -= HandleJumpAction;
        }
    }
}
