using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class KeybindManager : MonoBehaviour
{
    [SerializeField] private GameObject KeybindManagerPrefab;
    [SerializeField] private ScrollRect keybindScrollView;
    public struct RebindKeyOperation
    {
        public delegate void RebindActionExecute(InputControl inputControl);
        public RebindActionExecute rebindActionExecute;
        public delegate void RebindActionComplete();
        public RebindActionComplete rebindActionComplete;
    }
    private RebindKeyOperation currentRebindKeyOperation;
    private bool rebindKeyOperationActive = false;
    private IDisposable currentInputListener;

    private void OnDisable()
    {
        CompleteRebindKeyOperation();
    }
    private void Awake()
    {
        AddKeybind(InputSystem.actions.FindAction("ToggleViewHandy"),"Open Handy");
    }


    private void AddKeybind(InputAction inputAction,string displayName)
    {
        KeybindHelper keybindHelper = Instantiate(KeybindManagerPrefab,keybindScrollView.content.transform).GetComponent<KeybindHelper>();
        keybindHelper.Initalize(inputAction,inputAction.bindings[0],this,displayName);
    }
    public void CompleteRebindKeyOperation()
    {
        if(!rebindKeyOperationActive)
            return;

        currentInputListener?.Dispose();
        currentRebindKeyOperation.rebindActionComplete();
    }
    public void RegisterRebindKeyOperation(RebindKeyOperation rebindKeyOperation)
    {
        CompleteRebindKeyOperation();

        rebindKeyOperationActive = true;
        currentRebindKeyOperation = rebindKeyOperation;
        Action<InputControl> executeAction = new(currentRebindKeyOperation.rebindActionExecute);
        currentInputListener = InputSystem.onAnyButtonPress.Call(executeAction);
    }
}
