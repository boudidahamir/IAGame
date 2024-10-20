//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/CharacterControllerInputSystem.inputactions
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

public partial class @CharacterControllerInputSystem: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterControllerInputSystem()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterControllerInputSystem"",
    ""maps"": [
        {
            ""name"": ""NMVM"",
            ""id"": ""da9e1bb9-9630-4f0b-b145-7c3c867e1af7"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""1c3ccf08-d97b-4860-b3bd-68f2e4f661b8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""c3ff15de-9f17-4f1a-aff8-6c87b9cc1280"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""95f392bf-32c4-428b-80ae-a84cc22a6936"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""69ade74c-3fd2-46d2-8915-d15af99f6db1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""bf1e1118-fcb1-4e7a-8e4d-0171bb461ae9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""df7bd5a2-1ba4-4cab-be0a-8a7f5fbf17ef"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""94c78ff3-7167-4d0c-96b2-26129720c21d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""aafbe52c-2de8-4f90-b59c-15364a9280ba"",
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
                    ""id"": ""938ed202-cc06-4059-9822-7bc17ffe4db7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c41e567-a01d-48d8-9d5c-da35e1d5cd7f"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // NMVM
        m_NMVM = asset.FindActionMap("NMVM", throwIfNotFound: true);
        m_NMVM_Movement = m_NMVM.FindAction("Movement", throwIfNotFound: true);
        m_NMVM_Jump = m_NMVM.FindAction("Jump", throwIfNotFound: true);
        m_NMVM_Sprint = m_NMVM.FindAction("Sprint", throwIfNotFound: true);
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

    // NMVM
    private readonly InputActionMap m_NMVM;
    private List<INMVMActions> m_NMVMActionsCallbackInterfaces = new List<INMVMActions>();
    private readonly InputAction m_NMVM_Movement;
    private readonly InputAction m_NMVM_Jump;
    private readonly InputAction m_NMVM_Sprint;
    public struct NMVMActions
    {
        private @CharacterControllerInputSystem m_Wrapper;
        public NMVMActions(@CharacterControllerInputSystem wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_NMVM_Movement;
        public InputAction @Jump => m_Wrapper.m_NMVM_Jump;
        public InputAction @Sprint => m_Wrapper.m_NMVM_Sprint;
        public InputActionMap Get() { return m_Wrapper.m_NMVM; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NMVMActions set) { return set.Get(); }
        public void AddCallbacks(INMVMActions instance)
        {
            if (instance == null || m_Wrapper.m_NMVMActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_NMVMActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
        }

        private void UnregisterCallbacks(INMVMActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
        }

        public void RemoveCallbacks(INMVMActions instance)
        {
            if (m_Wrapper.m_NMVMActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(INMVMActions instance)
        {
            foreach (var item in m_Wrapper.m_NMVMActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_NMVMActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public NMVMActions @NMVM => new NMVMActions(this);
    public interface INMVMActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
    }
}
