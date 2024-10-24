//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input Action/PlayerActionInput.inputactions
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

public partial class @PlayerActionInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActionInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActionInput"",
    ""maps"": [
        {
            ""name"": ""InputPlayerAction"",
            ""id"": ""d4ee359b-2cd3-4473-87c1-4692c2c35148"",
            ""actions"": [
                {
                    ""name"": ""Shooting"",
                    ""type"": ""Button"",
                    ""id"": ""a1a66122-41d9-44eb-bcb9-373f4612dc40"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SilentKill"",
                    ""type"": ""Button"",
                    ""id"": ""7bdecdc9-0fd9-456e-8d18-b74c23c791e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangingWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""00348e2f-2196-4c42-a40d-c98ec9b89632"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangePlayer"",
                    ""type"": ""Button"",
                    ""id"": ""071fe5e1-7e58-418d-82fa-ed09c5893767"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scope"",
                    ""type"": ""Button"",
                    ""id"": ""67bb825e-50c3-401d-9d2a-5ca407012fe9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""5ae68d93-695a-4e4d-9a7c-fba775db84a6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""709ab3e1-5252-4e58-9ddb-e56098262fb7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""a2ffa34d-e2f1-47da-b70d-bbd465b15076"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""e9c2ab17-b3b3-482d-accb-22c030686088"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Command"",
                    ""type"": ""Button"",
                    ""id"": ""1ec604b5-4781-4d40-8e48-83ff417424dc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UnCommand"",
                    ""type"": ""Button"",
                    ""id"": ""f2362205-11ac-4c79-b536-7e3b70145197"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HoldPosition"",
                    ""type"": ""Button"",
                    ""id"": ""a9cb49e1-b57b-4b41-a8d4-62092f74a2d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UnHoldPosition"",
                    ""type"": ""Button"",
                    ""id"": ""0a4e3c02-b9d2-42ef-8d1c-f6fe3c18ca50"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""44403a63-af6d-400c-b989-041ffc7ba145"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SkillButton"",
                    ""type"": ""Button"",
                    ""id"": ""7579b67e-6e66-4fe5-8f32-a21a83d7a398"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NightVision"",
                    ""type"": ""Button"",
                    ""id"": ""fc427d59-c9d3-4f11-89aa-58cc4e6f4251"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""48981095-f568-4301-981c-8525f9220f63"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shooting"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce50390c-ee25-4867-8064-458229c96c72"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SilentKill"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1c05c04-dee4-44f3-9329-624685e571ec"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangingWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62738c04-e284-4457-8049-049aeb8de3dd"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangePlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7bca3d4-785c-43d2-92a5-8c920d738d7b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scope"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""9a8c5271-1f20-466d-9166-97da362fafba"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ed510a29-e23b-4b04-a517-f9d2ed64d0b7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""914f02ea-3029-446a-a05a-428b4995fda4"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""85487b36-d284-4240-abda-3d058b115087"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""eda68aeb-b1f6-4359-b924-de6a00bdf5b5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""66628e6c-17d0-457a-8e93-d1bc9b5dc5d5"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""171cce9c-8a53-467b-82e3-0ca64e783415"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a973222-8b41-4b6a-9ca0-f9c2fb631734"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c6b6b74-b75c-4802-a5fe-e62fdfcdc78f"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Command"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2683275f-ef13-4cc8-ab81-71959eda5000"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UnCommand"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9fa93b2-0119-46ef-92c4-3f0fd1c057c2"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HoldPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f48e8ad4-d1d1-473e-8c54-f4ee456d2a86"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UnHoldPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f3e16bd-c721-4aba-8d18-89ac95e25f2d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8a77be8-faad-4d6d-b4fb-11793bee0786"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SkillButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9920ae0b-8d2d-4c44-bf96-86f1558d8c1a"",
                    ""path"": ""<Keyboard>/n"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NightVision"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // InputPlayerAction
        m_InputPlayerAction = asset.FindActionMap("InputPlayerAction", throwIfNotFound: true);
        m_InputPlayerAction_Shooting = m_InputPlayerAction.FindAction("Shooting", throwIfNotFound: true);
        m_InputPlayerAction_SilentKill = m_InputPlayerAction.FindAction("SilentKill", throwIfNotFound: true);
        m_InputPlayerAction_ChangingWeapon = m_InputPlayerAction.FindAction("ChangingWeapon", throwIfNotFound: true);
        m_InputPlayerAction_ChangePlayer = m_InputPlayerAction.FindAction("ChangePlayer", throwIfNotFound: true);
        m_InputPlayerAction_Scope = m_InputPlayerAction.FindAction("Scope", throwIfNotFound: true);
        m_InputPlayerAction_Movement = m_InputPlayerAction.FindAction("Movement", throwIfNotFound: true);
        m_InputPlayerAction_Crouch = m_InputPlayerAction.FindAction("Crouch", throwIfNotFound: true);
        m_InputPlayerAction_Run = m_InputPlayerAction.FindAction("Run", throwIfNotFound: true);
        m_InputPlayerAction_Reload = m_InputPlayerAction.FindAction("Reload", throwIfNotFound: true);
        m_InputPlayerAction_Command = m_InputPlayerAction.FindAction("Command", throwIfNotFound: true);
        m_InputPlayerAction_UnCommand = m_InputPlayerAction.FindAction("UnCommand", throwIfNotFound: true);
        m_InputPlayerAction_HoldPosition = m_InputPlayerAction.FindAction("HoldPosition", throwIfNotFound: true);
        m_InputPlayerAction_UnHoldPosition = m_InputPlayerAction.FindAction("UnHoldPosition", throwIfNotFound: true);
        m_InputPlayerAction_Interact = m_InputPlayerAction.FindAction("Interact", throwIfNotFound: true);
        m_InputPlayerAction_SkillButton = m_InputPlayerAction.FindAction("SkillButton", throwIfNotFound: true);
        m_InputPlayerAction_NightVision = m_InputPlayerAction.FindAction("NightVision", throwIfNotFound: true);
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

    // InputPlayerAction
    private readonly InputActionMap m_InputPlayerAction;
    private List<IInputPlayerActionActions> m_InputPlayerActionActionsCallbackInterfaces = new List<IInputPlayerActionActions>();
    private readonly InputAction m_InputPlayerAction_Shooting;
    private readonly InputAction m_InputPlayerAction_SilentKill;
    private readonly InputAction m_InputPlayerAction_ChangingWeapon;
    private readonly InputAction m_InputPlayerAction_ChangePlayer;
    private readonly InputAction m_InputPlayerAction_Scope;
    private readonly InputAction m_InputPlayerAction_Movement;
    private readonly InputAction m_InputPlayerAction_Crouch;
    private readonly InputAction m_InputPlayerAction_Run;
    private readonly InputAction m_InputPlayerAction_Reload;
    private readonly InputAction m_InputPlayerAction_Command;
    private readonly InputAction m_InputPlayerAction_UnCommand;
    private readonly InputAction m_InputPlayerAction_HoldPosition;
    private readonly InputAction m_InputPlayerAction_UnHoldPosition;
    private readonly InputAction m_InputPlayerAction_Interact;
    private readonly InputAction m_InputPlayerAction_SkillButton;
    private readonly InputAction m_InputPlayerAction_NightVision;
    public struct InputPlayerActionActions
    {
        private @PlayerActionInput m_Wrapper;
        public InputPlayerActionActions(@PlayerActionInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shooting => m_Wrapper.m_InputPlayerAction_Shooting;
        public InputAction @SilentKill => m_Wrapper.m_InputPlayerAction_SilentKill;
        public InputAction @ChangingWeapon => m_Wrapper.m_InputPlayerAction_ChangingWeapon;
        public InputAction @ChangePlayer => m_Wrapper.m_InputPlayerAction_ChangePlayer;
        public InputAction @Scope => m_Wrapper.m_InputPlayerAction_Scope;
        public InputAction @Movement => m_Wrapper.m_InputPlayerAction_Movement;
        public InputAction @Crouch => m_Wrapper.m_InputPlayerAction_Crouch;
        public InputAction @Run => m_Wrapper.m_InputPlayerAction_Run;
        public InputAction @Reload => m_Wrapper.m_InputPlayerAction_Reload;
        public InputAction @Command => m_Wrapper.m_InputPlayerAction_Command;
        public InputAction @UnCommand => m_Wrapper.m_InputPlayerAction_UnCommand;
        public InputAction @HoldPosition => m_Wrapper.m_InputPlayerAction_HoldPosition;
        public InputAction @UnHoldPosition => m_Wrapper.m_InputPlayerAction_UnHoldPosition;
        public InputAction @Interact => m_Wrapper.m_InputPlayerAction_Interact;
        public InputAction @SkillButton => m_Wrapper.m_InputPlayerAction_SkillButton;
        public InputAction @NightVision => m_Wrapper.m_InputPlayerAction_NightVision;
        public InputActionMap Get() { return m_Wrapper.m_InputPlayerAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InputPlayerActionActions set) { return set.Get(); }
        public void AddCallbacks(IInputPlayerActionActions instance)
        {
            if (instance == null || m_Wrapper.m_InputPlayerActionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InputPlayerActionActionsCallbackInterfaces.Add(instance);
            @Shooting.started += instance.OnShooting;
            @Shooting.performed += instance.OnShooting;
            @Shooting.canceled += instance.OnShooting;
            @SilentKill.started += instance.OnSilentKill;
            @SilentKill.performed += instance.OnSilentKill;
            @SilentKill.canceled += instance.OnSilentKill;
            @ChangingWeapon.started += instance.OnChangingWeapon;
            @ChangingWeapon.performed += instance.OnChangingWeapon;
            @ChangingWeapon.canceled += instance.OnChangingWeapon;
            @ChangePlayer.started += instance.OnChangePlayer;
            @ChangePlayer.performed += instance.OnChangePlayer;
            @ChangePlayer.canceled += instance.OnChangePlayer;
            @Scope.started += instance.OnScope;
            @Scope.performed += instance.OnScope;
            @Scope.canceled += instance.OnScope;
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @Reload.started += instance.OnReload;
            @Reload.performed += instance.OnReload;
            @Reload.canceled += instance.OnReload;
            @Command.started += instance.OnCommand;
            @Command.performed += instance.OnCommand;
            @Command.canceled += instance.OnCommand;
            @UnCommand.started += instance.OnUnCommand;
            @UnCommand.performed += instance.OnUnCommand;
            @UnCommand.canceled += instance.OnUnCommand;
            @HoldPosition.started += instance.OnHoldPosition;
            @HoldPosition.performed += instance.OnHoldPosition;
            @HoldPosition.canceled += instance.OnHoldPosition;
            @UnHoldPosition.started += instance.OnUnHoldPosition;
            @UnHoldPosition.performed += instance.OnUnHoldPosition;
            @UnHoldPosition.canceled += instance.OnUnHoldPosition;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @SkillButton.started += instance.OnSkillButton;
            @SkillButton.performed += instance.OnSkillButton;
            @SkillButton.canceled += instance.OnSkillButton;
            @NightVision.started += instance.OnNightVision;
            @NightVision.performed += instance.OnNightVision;
            @NightVision.canceled += instance.OnNightVision;
        }

        private void UnregisterCallbacks(IInputPlayerActionActions instance)
        {
            @Shooting.started -= instance.OnShooting;
            @Shooting.performed -= instance.OnShooting;
            @Shooting.canceled -= instance.OnShooting;
            @SilentKill.started -= instance.OnSilentKill;
            @SilentKill.performed -= instance.OnSilentKill;
            @SilentKill.canceled -= instance.OnSilentKill;
            @ChangingWeapon.started -= instance.OnChangingWeapon;
            @ChangingWeapon.performed -= instance.OnChangingWeapon;
            @ChangingWeapon.canceled -= instance.OnChangingWeapon;
            @ChangePlayer.started -= instance.OnChangePlayer;
            @ChangePlayer.performed -= instance.OnChangePlayer;
            @ChangePlayer.canceled -= instance.OnChangePlayer;
            @Scope.started -= instance.OnScope;
            @Scope.performed -= instance.OnScope;
            @Scope.canceled -= instance.OnScope;
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @Reload.started -= instance.OnReload;
            @Reload.performed -= instance.OnReload;
            @Reload.canceled -= instance.OnReload;
            @Command.started -= instance.OnCommand;
            @Command.performed -= instance.OnCommand;
            @Command.canceled -= instance.OnCommand;
            @UnCommand.started -= instance.OnUnCommand;
            @UnCommand.performed -= instance.OnUnCommand;
            @UnCommand.canceled -= instance.OnUnCommand;
            @HoldPosition.started -= instance.OnHoldPosition;
            @HoldPosition.performed -= instance.OnHoldPosition;
            @HoldPosition.canceled -= instance.OnHoldPosition;
            @UnHoldPosition.started -= instance.OnUnHoldPosition;
            @UnHoldPosition.performed -= instance.OnUnHoldPosition;
            @UnHoldPosition.canceled -= instance.OnUnHoldPosition;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @SkillButton.started -= instance.OnSkillButton;
            @SkillButton.performed -= instance.OnSkillButton;
            @SkillButton.canceled -= instance.OnSkillButton;
            @NightVision.started -= instance.OnNightVision;
            @NightVision.performed -= instance.OnNightVision;
            @NightVision.canceled -= instance.OnNightVision;
        }

        public void RemoveCallbacks(IInputPlayerActionActions instance)
        {
            if (m_Wrapper.m_InputPlayerActionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInputPlayerActionActions instance)
        {
            foreach (var item in m_Wrapper.m_InputPlayerActionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InputPlayerActionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InputPlayerActionActions @InputPlayerAction => new InputPlayerActionActions(this);
    public interface IInputPlayerActionActions
    {
        void OnShooting(InputAction.CallbackContext context);
        void OnSilentKill(InputAction.CallbackContext context);
        void OnChangingWeapon(InputAction.CallbackContext context);
        void OnChangePlayer(InputAction.CallbackContext context);
        void OnScope(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnCommand(InputAction.CallbackContext context);
        void OnUnCommand(InputAction.CallbackContext context);
        void OnHoldPosition(InputAction.CallbackContext context);
        void OnUnHoldPosition(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnSkillButton(InputAction.CallbackContext context);
        void OnNightVision(InputAction.CallbackContext context);
    }
}
