using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SaveMenuHelper
{
    

[Serializable]
    internal class SaveSlot
    {
        private readonly string EmptySaveName = "Empty";
        [SerializeField] private TMP_Text saveName;
        [SerializeField] private Image borderImage;
        private Button button;
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
        private void OnClick(SaveMenu saveMenu)
        {
            saveMenu.ReturnToTitleMenu(saveName.text);
        }
        public void Initalize(SaveMenu saveMenu)
        {
            button = borderImage.GetComponent<Button>();
            button.onClick.AddListener(() => OnClick(saveMenu));
        }
    };
}