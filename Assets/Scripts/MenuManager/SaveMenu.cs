using System;
using System.Collections.Generic;
using UnityEngine;
using SaveMenuElements;
public class SaveMenu : MonoBehaviour
{
    [SerializeField] private GameObject TitleMenuReference;
    [Header("SaveSlots")]
    [SerializeField] private SaveSlot[] saveSlots;
    private int maxPage = 0;
    private List<string> cachedSaveIDs;
    private int currentPage = 0;
    void Start()
    {
        SaveSlot.SetGlobalSaveMenuReference(this);
        foreach (SaveSlot saveSlot in saveSlots)
            saveSlot.Initalize();

    }
    void OnEnable()
    {
        cachedSaveIDs = GlobalDataStore.Instance.saveManager.GetExistingSaveIDs();
        cachedSaveIDs = cachedSaveIDs.FindAll((str) => str != GlobalDataStore.Instance.saveManager.CurrentSaveID);
        cachedSaveIDs.Insert(0, GlobalDataStore.Instance.saveManager.CurrentSaveID);

        currentPage = 0;
        maxPage = (int)Math.Ceiling((double)((cachedSaveIDs.Count - 1) / saveSlots.Length));
        SaveSlotsUpdate();
    }
    public void ReturnToTitleMenu()
    {
        TitleMenuReference.SetActive(true);
        gameObject.SetActive(false);
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
    private void SaveSlotsUpdate()
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

    public void ForceReload()
    {
        OnEnable();
    }
    public void SetSaveMenuInteractableState(bool active)
    {
        UIHelper.SetSelectableObjectState(transform.gameObject, active);
    }
}
