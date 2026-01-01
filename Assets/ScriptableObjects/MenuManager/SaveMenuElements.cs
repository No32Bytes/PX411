using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SaveMenuElements
{
    

[Serializable]
    internal class SaveSlot
    {
        private readonly string EmptySaveName = "Empty";
        private static SaveMenu saveMenuReference;
        [SerializeField] private TMP_Text saveName;
        [SerializeField] private Image borderImage;
        [SerializeField] private Button deleteButton;
        public static void SetGlobalSaveMenuReference(SaveMenu saveMenu)
        {
            saveMenuReference = saveMenu;
        }
        private void SetActiveSaveState(bool active)
        {
            if (active)
                borderImage.color = Color.forestGreen;
            else
                borderImage.color = Color.red;
        }
        public void SetSaveName(string saveName)
        {
            this.saveName.text = saveName;
            SetActiveSaveState(saveName == GlobalDataStore.Instance.saveManager.CurrentSaveID);
        }
        public void DisplaySaveStats()
        {
            // TODO when there are stats
        }
        public void Clear()
        {
            SetSaveName(EmptySaveName);
        }
        private void OnSaveLoadClick()
        {
            GlobalDataStore.Instance.saveManager.Load(saveName.text);
            saveMenuReference.ReturnToTitleMenu();
        }
        private void OnDeleteButtonOnClick()
        {
            GlobalDataStore.Instance.saveManager.Delete(saveName.text);
            saveMenuReference.ForceReload();
        }
        public void Initalize()
        {
            borderImage.GetComponent<Button>().onClick.AddListener(()=> OnSaveLoadClick());
            
            deleteButton.onClick.AddListener(() => OnDeleteButtonOnClick());
        }
    };
}