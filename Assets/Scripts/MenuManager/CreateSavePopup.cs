using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class CreateSavePopup : MonoBehaviour
{
    [SerializeField] private SaveMenu saveMenu;
    [SerializeField] private TMP_InputField inputTextTMP;
    [SerializeField] private GameObject inputErrorLabel;
    [SerializeField] private TMP_Text inputErrorTMP;
    void OnEnable()
    {
        inputTextTMP.text = "";
        inputTextTMP.ActivateInputField();
        inputErrorLabel.SetActive(false);
    }
    public void CreateButtonOnClick()
    {
        if (inputTextTMP.text == "")
        {
            SetInputError("Save Name cannot be empty");
            return;
        }

        List<string> exisitingSaveIDs = GlobalDataStore.Instance.saveManager.GetExistingSaveIDs();
        exisitingSaveIDs.ForEach((saveID) => saveID = saveID.ToLower());
        if (exisitingSaveIDs.Contains(inputTextTMP.text.ToLower()))
        {
            SetInputError($"{inputTextTMP.text} already exists");
            return;
        }

        GlobalDataStore.Instance.saveManager.Load(inputTextTMP.text);
        GlobalDataStore.Instance.saveManager.Save();
        GlobalDataStore.Instance.settingsManager.Save();
        transform.gameObject.SetActive(false);
        saveMenu.SetSaveMenuInteractableState(true);
        saveMenu.ForceReload();
    }
    private void SetInputError(string message)
    {
        inputErrorLabel.SetActive(true);
        inputErrorTMP.SetText(message);
    }
}
