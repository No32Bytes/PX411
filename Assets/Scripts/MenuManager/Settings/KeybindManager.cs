using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class KeybindManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuRef;
    [SerializeField] private GameObject settingsKeybindMenuRef;
    [SerializeField] private KeybindManagerConfig keybindManagerData;
    [SerializeField] private GameObject keybindHelperPrefab;
    [SerializeField] private ScrollRect keybindScrollView;
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMPro.TMP_Text errorMessageText;
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

    private void OnEnable()
    {
        errorMessage.SetActive(false);
    }
    private void OnDisable()
    {
        CompleteRebindKeyOperation();
    }
    private void Awake()
    {
        Initalize();
    }
    private void Initalize()
    {
        foreach (KeybindManagerConfig.KeybindMangerData data in keybindManagerData.configData)
        {
            if (!data.showKeybind)
                continue;
            AddKeybind(data.GetAction(), data.relativeBindingIndex, data.displayName);
        }
    }
    private void AddKeybind(InputAction inputAction, int relativeBindingIndex, string displayName)
    {
        KeybindHelper keybindHelper = Instantiate(keybindHelperPrefab, keybindScrollView.content.transform).GetComponent<KeybindHelper>();
        keybindHelper.Initalize(inputAction, inputAction.bindings[relativeBindingIndex], this, displayName);
    }
    public bool IsActionKeybindEnabled(string actionName)
    {
        return keybindManagerData.configData.FindIndex(data => data.actionName == actionName && data.showKeybind) != -1;
    }
    public void ErrorRebindKeyOperation(string usedKey)
    {
        errorMessageText.text = $"The key {usedKey} is already in use";
        errorMessage.SetActive(true);
        CompleteRebindKeyOperation();
    }
    public void CompleteRebindKeyOperation()
    {
        if (!rebindKeyOperationActive)
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

    void Update()
    {
        if (GlobalDataStore.GetStateManager().playerState.player == null)
            return;

        if (!GlobalDataStore.GetStateManager().playerState.player.PauseActionRef.InteractWithCooldown())
            return;

        settingsMenuRef.SetActive(true);
        settingsKeybindMenuRef.SetActive(false);
    }
}
