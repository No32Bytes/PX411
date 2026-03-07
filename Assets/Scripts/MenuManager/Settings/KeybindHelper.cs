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
        currentActionKeyName.text = inputAction.GetBindingDisplayString(inputAction.GetBindingIndex(inputBinding));
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

    private void RebindActionExecute(InputControl inputControl)
    {
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
        inputBinding.overridePath = null;
        Reload();
    }
}
