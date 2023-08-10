//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/LocalPlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @LocalPlayerInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @LocalPlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""LocalPlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""7ad54c69-f60f-45a5-aa5c-fcbad5c0d69d"",
            ""actions"": [
                {
                    ""name"": ""Confirm"",
                    ""type"": ""Value"",
                    ""id"": ""d350f552-c169-41ce-ba93-c49df8ab1c04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Value"",
                    ""id"": ""f0fd9a67-900f-4ebd-9552-ecde90e655b7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""753aa369-de52-48b3-b757-f6f38823fb95"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.1)"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NavigateUp"",
                    ""type"": ""Value"",
                    ""id"": ""dc07d1b7-9d7b-4576-8a63-c0d636f532ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NavigateLeft"",
                    ""type"": ""Value"",
                    ""id"": ""b1868d8a-bd19-44f4-a9e3-daacf47fadc5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NavigateDown"",
                    ""type"": ""Value"",
                    ""id"": ""5434f69e-5e49-44f8-a930-35bc96b8acfa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NavigateRight"",
                    ""type"": ""Value"",
                    ""id"": ""32b2766e-1e35-4714-bdb1-6922611e3031"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3a1d9d67-b4ad-4f79-b581-246cb19bf11f"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c726a350-c6a2-497d-b9bf-6228cdbda52c"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""0405eefa-ea5d-4986-aa78-6543c004b1f1"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b5f40317-f08b-4eeb-84b7-b502dc286538"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""09905245-b874-4c44-bb2d-c1910c24420f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ab975421-1ca7-4933-ae30-4ae3abcae16b"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""56990bdb-9928-40b4-82a5-a0946443868d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2922ac49-6e5e-4911-894b-c2ec3b6d4d4b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NavigateUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""edfc2c03-0d86-4b3d-af7e-8b473488abb0"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NavigateLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3b8d62e9-2cb6-4e35-b9fe-a8ec2fbeb1c1"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NavigateDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f4f8145-3875-418e-bece-08b937fac6b8"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NavigateRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Confirm = m_Player.FindAction("Confirm", throwIfNotFound: true);
        m_Player_Cancel = m_Player.FindAction("Cancel", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_NavigateUp = m_Player.FindAction("NavigateUp", throwIfNotFound: true);
        m_Player_NavigateLeft = m_Player.FindAction("NavigateLeft", throwIfNotFound: true);
        m_Player_NavigateDown = m_Player.FindAction("NavigateDown", throwIfNotFound: true);
        m_Player_NavigateRight = m_Player.FindAction("NavigateRight", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Confirm;
    private readonly InputAction m_Player_Cancel;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_NavigateUp;
    private readonly InputAction m_Player_NavigateLeft;
    private readonly InputAction m_Player_NavigateDown;
    private readonly InputAction m_Player_NavigateRight;
    public struct PlayerActions
    {
        private @LocalPlayerInput m_Wrapper;
        public PlayerActions(@LocalPlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Confirm => m_Wrapper.m_Player_Confirm;
        public InputAction @Cancel => m_Wrapper.m_Player_Cancel;
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @NavigateUp => m_Wrapper.m_Player_NavigateUp;
        public InputAction @NavigateLeft => m_Wrapper.m_Player_NavigateLeft;
        public InputAction @NavigateDown => m_Wrapper.m_Player_NavigateDown;
        public InputAction @NavigateRight => m_Wrapper.m_Player_NavigateRight;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Confirm.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConfirm;
                @Confirm.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConfirm;
                @Confirm.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConfirm;
                @Cancel.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @NavigateUp.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateUp;
                @NavigateUp.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateUp;
                @NavigateUp.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateUp;
                @NavigateLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateLeft;
                @NavigateLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateLeft;
                @NavigateLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateLeft;
                @NavigateDown.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateDown;
                @NavigateDown.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateDown;
                @NavigateDown.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateDown;
                @NavigateRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateRight;
                @NavigateRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateRight;
                @NavigateRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNavigateRight;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Confirm.started += instance.OnConfirm;
                @Confirm.performed += instance.OnConfirm;
                @Confirm.canceled += instance.OnConfirm;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @NavigateUp.started += instance.OnNavigateUp;
                @NavigateUp.performed += instance.OnNavigateUp;
                @NavigateUp.canceled += instance.OnNavigateUp;
                @NavigateLeft.started += instance.OnNavigateLeft;
                @NavigateLeft.performed += instance.OnNavigateLeft;
                @NavigateLeft.canceled += instance.OnNavigateLeft;
                @NavigateDown.started += instance.OnNavigateDown;
                @NavigateDown.performed += instance.OnNavigateDown;
                @NavigateDown.canceled += instance.OnNavigateDown;
                @NavigateRight.started += instance.OnNavigateRight;
                @NavigateRight.performed += instance.OnNavigateRight;
                @NavigateRight.canceled += instance.OnNavigateRight;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnConfirm(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnNavigateUp(InputAction.CallbackContext context);
        void OnNavigateLeft(InputAction.CallbackContext context);
        void OnNavigateDown(InputAction.CallbackContext context);
        void OnNavigateRight(InputAction.CallbackContext context);
    }
}