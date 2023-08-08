using System.Collections;
using System.Collections.Generic;
using System.Utilities.Tasks;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace System.Utilities.Input
{
    public static class InputController
    {
        private static LocalPlayerInput _playerInput;
        
        private static Dictionary<string, InputState> InputStateMap { get; set; }
        private static InputActionMap _actionMap;

        public enum InputState { Press, Pressed, Hold, Release, Released }
        public enum Action { Confirm, Cancel, Move, NavigateUp, NavigateDown, NavigateLeft, NavigateRight }

        static InputController()
        {
            InputStateMap = new Dictionary<string, InputState>();
                
            _playerInput = new LocalPlayerInput();
            _playerInput.Player.Enable();
            
            _actionMap = _playerInput.Player.Get();
            _actionMap.ForEach(action =>
            {
                action.started += OnInputStart;
                action.performed += OnInputPerformed;
                action.canceled += OnInputCancel;

                InputStateMap.Add(action.name, InputState.Release);
            });
        }

        private static IEnumerator UpdateInputState(InputAction action, InputState inputState)
        {
            InputStateMap[action.name] = inputState;
            yield return null;

            InputStateMap[action.name] = InputStateMap[action.name] switch
            {
                InputState.Press => InputState.Pressed,
                InputState.Release => InputState.Released,
                _ => InputStateMap[action.name]
            };
        }

        private static void OnInputStart(InputAction.CallbackContext context) => new Task(UpdateInputState(context.action, InputState.Press));
        private static void OnInputPerformed(InputAction.CallbackContext context) => new Task(UpdateInputState(context.action, InputState.Hold));
        private static void OnInputCancel(InputAction.CallbackContext context) => new Task(UpdateInputState(context.action, InputState.Release));

        private static bool CheckInputState(string action, InputState inputState) => InputStateMap[action] == inputState;
        private static T GetInputValue<T>(string action) where T : struct => _actionMap[action].ReadValue<T>();
        
        public static bool CheckInputState(Action action, InputState inputState) => CheckInputState(action.ToString(), inputState);
        public static T GetInputValue<T>(Action action) where T : struct  => _actionMap[action.ToString()].ReadValue<T>();
        
        public static Vector2Int GetMoveInput => Vector2Int.RoundToInt(GetInputValue<Vector2>(Action.Move));

        private static int GetNavigateValue(Action action) => CheckInputState(action, InputState.Press) ? 1 : 0;
        public static int GetVerticalNavigateInput => GetNavigateValue(Action.NavigateUp) - GetNavigateValue(Action.NavigateDown);
        public static int GetHorizontalNavigateInput => GetNavigateValue(Action.NavigateRight) - GetNavigateValue(Action.NavigateLeft);
        public static Vector2Int GetNavigateVector => new Vector2Int(GetHorizontalNavigateInput, GetVerticalNavigateInput); 

        public static bool NavigateVertical => CheckInputState(Action.NavigateUp, InputState.Press) ||
                                       CheckInputState(Action.NavigateDown, InputState.Press);
        
        public static bool NavigateHorizontal => CheckInputState(Action.NavigateRight, InputState.Press) ||
                                               CheckInputState(Action.NavigateLeft, InputState.Press);

        public static bool Navigate => NavigateVertical || NavigateHorizontal;
        public static bool Confirm => CheckInputState(Action.Confirm, InputState.Press); 
        public static bool Cancel => CheckInputState(Action.Cancel, InputState.Press);
        public static bool ConfirmOrCancel => Confirm || Confirm;
        
        public static IEnumerator WaitForConfirm => new WaitUntil(() => Confirm);
    }
}