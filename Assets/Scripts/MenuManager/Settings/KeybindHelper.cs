using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindHelper : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text actionName;
    [SerializeField] private TMPro.TMP_Text currentActionKeyName;
    [SerializeField] private Button rebindActionButton;
    [SerializeField] private Button resetActionButton;
    private KeybindManager keybindManager;
    private InputAction inputAction;
    private InputBinding inputBinding;
    private string inputBindingName;
    private string KeyDisplayString => inputAction.GetBindingDisplayString(inputAction.GetBindingIndex(inputBinding));

    private void Awake()
    {
        rebindActionButton.onClick.AddListener(RebindActionOnClick);
        resetActionButton.onClick.AddListener(ResetActionOnClick);
    }
    public void Initalize(InputAction targetInputAction,InputBinding targetInputBinding, KeybindManager targetkeybindManager,string targetInputBindingName)
    {
        inputAction = targetInputAction;
        inputBinding = targetInputBinding;
        keybindManager = targetkeybindManager;
        inputBindingName = targetInputBindingName;
        Reload();
    }
    private void Reload()
    {
        actionName.text = inputBindingName;
        currentActionKeyName.text = KeyDisplayString;
    }

    private void RebindActionOnClick()
    {
        inputAction.Disable();
        KeybindManager.RebindKeyOperation rebindKeyOperation = new()
        {
            rebindActionExecute = RebindActionExecute,
            rebindActionComplete = RebindActionComplete
        };
        keybindManager.RegisterRebindKeyOperation(rebindKeyOperation);
    }

    private bool CheckIfBindingIsNotInUse(string newDisplayString)
    {
        IEnumerator<InputBinding> enumerator = InputSystem.actions.bindings.GetEnumerator();
        while(enumerator.MoveNext())
        {
            if(enumerator.Current.isComposite)
                continue;
            if(enumerator.Current.Matches(inputBinding))
                continue;
            if(!keybindManager.IsActionKeybindEnabled(enumerator.Current.action))
                continue;

            if(newDisplayString.ToUpper() == enumerator.Current.ToDisplayString().ToUpper())
                return false;
        }
        return true;
    }

    private void RebindActionExecute(InputControl inputControl)
    {
        string newDisplayString = new InputBinding(inputControl.path).ToDisplayString();
        if (!CheckIfBindingIsNotInUse(newDisplayString))
        {
            keybindManager.ErrorRebindKeyOperation(newDisplayString);
            return;
        }
        
        int bindingIndex = inputAction.GetBindingIndex(inputBinding);
        inputAction.ApplyBindingOverride(bindingIndex,inputControl.path);
        keybindManager.CompleteRebindKeyOperation();
    }

    private void RebindActionComplete()
    {
        Reload();
        inputAction.Enable();
    }

    private void ResetActionOnClick()
    {
        string newDisplayString = inputBinding.ToDisplayString(InputBinding.DisplayStringOptions.IgnoreBindingOverrides);
        if(!CheckIfBindingIsNotInUse(newDisplayString))
        {
            keybindManager.ErrorRebindKeyOperation(newDisplayString);
            return;
        }
        int bindingIndex = inputAction.GetBindingIndex(inputBinding);
        inputAction.RemoveBindingOverride(bindingIndex);
        Reload();
    }
}
