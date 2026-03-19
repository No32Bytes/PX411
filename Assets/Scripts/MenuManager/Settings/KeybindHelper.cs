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
    private string KeyDisplayString => inputAction.GetBindingDisplayString(GetBindingIndex());

    private int GetBindingIndex()
    {
            int index = inputAction.GetBindingIndex(inputBinding);
            if (index == -1)
            {
                for(int i = 0; i < inputAction.bindings.Count; i++)
                {
                    if(inputAction.bindings[i] != inputBinding)
                        continue;
                    index = i;
                    break;
                }
            }
            return index;
    }
    private void Awake()
    {
        rebindActionButton.onClick.AddListener(RebindActionOnClick);
        resetActionButton.onClick.AddListener(ResetActionOnClick);
    }
    public void Initalize(InputAction targetInputAction, InputBinding targetInputBinding, KeybindManager targetkeybindManager, string targetInputBindingName)
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
        while (enumerator.MoveNext())
        {
            InputBinding Current = enumerator.Current;
            if (Current.isComposite)
                continue;
            if (Current.Matches(inputBinding))
                continue;
            if (!keybindManager.IsActionKeybindEnabled(Current.action))
                continue;

            if (newDisplayString.ToUpper() == Current.ToDisplayString().ToUpper() && inputBinding.groups == Current.groups)
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

        int bindingIndex = GetBindingIndex();
        inputAction.ApplyBindingOverride(bindingIndex, inputControl.path);
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
        if (!CheckIfBindingIsNotInUse(newDisplayString))
        {
            keybindManager.ErrorRebindKeyOperation(newDisplayString);
            return;
        }
        int bindingIndex = GetBindingIndex();
        inputAction.RemoveBindingOverride(bindingIndex);
        Reload();
    }
}
