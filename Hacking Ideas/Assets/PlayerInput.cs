// GENERATED AUTOMATICALLY FROM 'Assets/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        this.asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""1fa02662-9f71-4626-9d74-9dbaff561951"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""507f40de-ff45-40be-a600-1708c021ab21"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rot"",
                    ""type"": ""PassThrough"",
                    ""id"": ""40ec3813-0c0b-412d-8bb2-745db72d9e86"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GrabRight"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2a7a044c-0d48-4540-a3a3-d6b533ea4aa4"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GrabLeft"",
                    ""type"": ""Button"",
                    ""id"": ""b5f62c51-cec7-44ca-a996-d57d3d40b7ae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""90ad3000-40ff-4e0a-b15a-44a48d2d568b"",
                    ""path"": ""<XRController>{RightHand}/grip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GrabRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0205018-67f4-4710-a4a7-960f4be852cf"",
                    ""path"": ""<XRController>{LeftHand}/grip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GrabLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a897ed6-50ce-4c2d-9f12-3cf5f706986a"",
                    ""path"": ""<XRController>{RightHand}/thumbstick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4c1c849-d126-4154-a6b5-49526989cd65"",
                    ""path"": ""<XRController>{LeftHand}/thumbstick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        this.m_Player = this.asset.FindActionMap("Player", throwIfNotFound: true);
        this.m_Player_Move = this.m_Player.FindAction("Move", throwIfNotFound: true);
        this.m_Player_Rot = this.m_Player.FindAction("Rot", throwIfNotFound: true);
        this.m_Player_GrabRight = this.m_Player.FindAction("GrabRight", throwIfNotFound: true);
        this.m_Player_GrabLeft = this.m_Player.FindAction("GrabLeft", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(this.asset);
    }

    public InputBinding? bindingMask
    {
        get => this.asset.bindingMask;
        set => this.asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => this.asset.devices;
        set => this.asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => this.asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return this.asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return this.asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void Enable()
    {
        this.asset.Enable();
    }

    public void Disable()
    {
        this.asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Rot;
    private readonly InputAction m_Player_GrabRight;
    private readonly InputAction m_Player_GrabLeft;
    public struct PlayerActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActions(@PlayerInput wrapper) {
            this.m_Wrapper = wrapper; }
        public InputAction @Move => this.m_Wrapper.m_Player_Move;
        public InputAction @Rot => this.m_Wrapper.m_Player_Rot;
        public InputAction @GrabRight => this.m_Wrapper.m_Player_GrabRight;
        public InputAction @GrabLeft => this.m_Wrapper.m_Player_GrabLeft;
        public InputActionMap Get() { return this.m_Wrapper.m_Player; }
        public void Enable() {
            this.Get().Enable(); }
        public void Disable() {
            this.Get().Disable(); }
        public bool enabled => this.Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (this.m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                this.@Move.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                this.@Move.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                this.@Move.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                this.@Rot.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnRot;
                this.@Rot.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnRot;
                this.@Rot.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnRot;
                this.@GrabRight.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnGrabRight;
                this.@GrabRight.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnGrabRight;
                this.@GrabRight.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnGrabRight;
                this.@GrabLeft.started -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnGrabLeft;
                this.@GrabLeft.performed -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnGrabLeft;
                this.@GrabLeft.canceled -= this.m_Wrapper.m_PlayerActionsCallbackInterface.OnGrabLeft;
            }

            this.m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                this.@Move.started += instance.OnMove;
                this.@Move.performed += instance.OnMove;
                this.@Move.canceled += instance.OnMove;
                this.@Rot.started += instance.OnRot;
                this.@Rot.performed += instance.OnRot;
                this.@Rot.canceled += instance.OnRot;
                this.@GrabRight.started += instance.OnGrabRight;
                this.@GrabRight.performed += instance.OnGrabRight;
                this.@GrabRight.canceled += instance.OnGrabRight;
                this.@GrabLeft.started += instance.OnGrabLeft;
                this.@GrabLeft.performed += instance.OnGrabLeft;
                this.@GrabLeft.canceled += instance.OnGrabLeft;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRot(InputAction.CallbackContext context);
        void OnGrabRight(InputAction.CallbackContext context);
        void OnGrabLeft(InputAction.CallbackContext context);
    }
}
