using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
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
            GlobalDataStore.Instance.saveManager.Load(saveName.text);
            saveMenu.TitleMenuReference.SetActive(true);
            saveMenu.gameObject.SetActive(false);
        }
        public void Initalize(SaveMenu saveMenu)
        {
            button = borderImage.GetComponent<Button>();
            button.onClick.AddListener(() => OnClick(saveMenu));
        }
    };
    [SerializeField] private GameObject TitleMenuReference;
    [Header("Saves")]
    [SerializeField] private SaveSlot[] saveSlots;
    private int maxPage = 0;
    private List<string> cachedSaveIDs;
    private int currentPage = 0;
    void Start()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.Initalize(this);
        }
    }
    void OnEnable()
    {
        cachedSaveIDs = GlobalDataStore.Instance.saveManager.GetExistingSaveIDs();
        cachedSaveIDs = cachedSaveIDs.FindAll((str) => str != GlobalDataStore.Instance.saveManager.CurrentSaveID);
        cachedSaveIDs.Insert(0,GlobalDataStore.Instance.saveManager.CurrentSaveID);

        currentPage = 0;
        maxPage = (int)Math.Ceiling((double)((cachedSaveIDs.Count -1) / saveSlots.Length));
        SaveSlotsUpdate();
    }

    public void OnClickBackButton()
    {
        if (currentPage == 0) return;
        currentPage--;
        SaveSlotsUpdate();
    }
    public void OnClickNextButton()
    {
        if (currentPage == maxPage) return;
        currentPage++;
        SaveSlotsUpdate();
    }
    public void SaveSlotsUpdate()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            SaveSlot saveSlot = saveSlots[i];
            if (saveSlots.Length * currentPage + i == cachedSaveIDs.Count)
            {
                saveSlot.Clear();
                return;
            }
            saveSlot.SetSaveName(cachedSaveIDs[i + currentPage * saveSlots.Length]);
            saveSlot.DisplaySaveStats();
        }
    }
}
