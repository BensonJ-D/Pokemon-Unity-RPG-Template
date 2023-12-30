using System.Collections;
using System.Collections.Generic;
using GameSystem.Utilities.Tasks;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace System.Utilities.Input
{
    public static class InputController
    {
        public enum Action
        {
            Confirm, Cancel, Move,
            NavigateUp, NavigateDown, NavigateLeft,
            NavigateRight, Test
        }

        public enum InputState
        {
            Press, Pressed, Hold,
            Release, Released
        }

        private static readonly LocalPlayerInput _playerInput;
        private static readonly InputActionMap _actionMap;

        static InputController() {
            InputStateMap = new Dictionary<string, InputState>();

            _playerInput = new LocalPlayerInput();
            _playerInput.Player.Enable();

            _actionMap = _playerInput.Player.Get();
            _actionMap.ForEach(action => {
                action.started += OnInputStart;
                action.performed += OnInputPerformed;
                action.canceled += OnInputCancel;

                InputStateMap.Add(action.name, InputState.Release);
            });
        }

        private static Dictionary<string, InputState> InputStateMap { get; }

        public static Vector3Int GetMoveInput() {
            var raw = Vector3Int.RoundToInt(GetInputValue<Vector2>(Action.Move));

            return raw.x != 0
                ? new Vector3Int(raw.x, 0)
                : new Vector3Int(0,     raw.y);
        }

        public static int GetVerticalNavigateInput =>
            GetNavigateValue(Action.NavigateUp) - GetNavigateValue(Action.NavigateDown);

        public static int GetHorizontalNavigateInput =>
            GetNavigateValue(Action.NavigateRight) - GetNavigateValue(Action.NavigateLeft);

        public static Vector2Int GetNavigateVector => new(GetHorizontalNavigateInput, GetVerticalNavigateInput);

        public static bool NavigateVertical => CheckInputState(Action.NavigateUp,   InputState.Press) ||
                                               CheckInputState(Action.NavigateDown, InputState.Press);

        public static bool NavigateHorizontal => CheckInputState(Action.NavigateRight, InputState.Press) ||
                                                 CheckInputState(Action.NavigateLeft,  InputState.Press);

        public static bool Navigate => NavigateVertical || NavigateHorizontal;
        public static bool Confirm => CheckInputState(Action.Confirm, InputState.Press);
        public static bool Cancel => CheckInputState(Action.Cancel,   InputState.Press);
        public static bool ConfirmOrCancel => Confirm || Cancel;

        public static bool ConfirmPressed => CheckInputState(Action.Confirm, InputState.Pressed);
        public static bool CancelPressed => CheckInputState(Action.Cancel,   InputState.Pressed);
        public static bool ConfirmHold => CheckInputState(Action.Confirm,    InputState.Hold);
        public static bool CancelHold => CheckInputState(Action.Cancel,      InputState.Hold);

        public static bool ConfirmDown => Confirm || ConfirmPressed || ConfirmHold;
        public static bool CancelDown => Cancel || CancelPressed || CancelHold;
        public static bool ConfirmOrCancelDown => ConfirmDown || CancelDown;

        public static bool ConfirmRelease => CheckInputState(Action.Confirm,  InputState.Release);
        public static bool CancelRelease => CheckInputState(Action.Cancel,    InputState.Release);
        public static bool ConfirmReleased => CheckInputState(Action.Confirm, InputState.Released);
        public static bool CancelReleased => CheckInputState(Action.Cancel,   InputState.Released);
        public static bool ConfirmUp => ConfirmRelease || ConfirmReleased;
        public static bool CancelUp => CancelRelease || CancelReleased;
        public static bool ConfirmAndCancelUp => ConfirmUp && CancelUp;

        public static bool Test => CheckInputState(Action.Test, InputState.Press);

        public static IEnumerator WaitForConfirm => new WaitUntil(() => Confirm);

        private static IEnumerator UpdateInputState(InputAction action, InputState inputState) {
            InputStateMap[action.name] = inputState;
            yield return null;

            InputStateMap[action.name] = InputStateMap[action.name] switch {
                InputState.Press   => InputState.Pressed,
                InputState.Release => InputState.Released,
                _                  => InputStateMap[action.name]
            };
        }

        private static void OnInputStart(InputAction.CallbackContext context) {
            new Task(UpdateInputState(context.action, InputState.Press));
        }

        private static void OnInputPerformed(InputAction.CallbackContext context) {
            new Task(UpdateInputState(context.action, InputState.Hold));
        }

        private static void OnInputCancel(InputAction.CallbackContext context) {
            new Task(UpdateInputState(context.action, InputState.Release));
        }

        private static bool CheckInputState(string action, InputState inputState) {
            return InputStateMap[action] == inputState;
        }

        private static T GetInputValue<T>(string action) where T : struct {
            return _actionMap[action].ReadValue<T>();
        }

        public static bool CheckInputState(Action action, InputState inputState) {
            return CheckInputState(action.ToString(), inputState);
        }

        public static T GetInputValue<T>(Action action) where T : struct {
            return _actionMap[action.ToString()].ReadValue<T>();
        }

        private static int GetNavigateValue(Action action) {
            return CheckInputState(action, InputState.Press) ? 1 : 0;
        }
    }
}