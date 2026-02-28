using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScreen : MonoBehaviour
{
    [SerializeField] private ScrollRect inventoryScrollView;
    [SerializeField] private float scrollViewExtend = 500;
    [SerializeField] private GameObject inventoryItemUI;
    private void OnEnable()
    {
        RenderInventoryScreen();
    }
    private void Update()
    {
        if(inventoryScrollView.content.transform.localPosition.x > scrollViewExtend)
        {
            inventoryScrollView.content.transform.localPosition = new Vector3(scrollViewExtend,0,0);
            inventoryScrollView.velocity = Vector2.zero;
        }
        if(inventoryScrollView.content.transform.localPosition.x < -inventoryScrollView.content.sizeDelta.x - scrollViewExtend)
        {
            inventoryScrollView.content.transform.localPosition = new Vector3(-inventoryScrollView.content.sizeDelta.x - scrollViewExtend,0,0);
            inventoryScrollView.velocity = Vector2.zero;
        }

    }
    private void RenderInventoryScreen()
    {
        List<InventoryItem> inventory = GlobalDataStore.GetInventory().GetStoreableInventory();
        foreach(InventoryItem inventoryItem in inventory)
        {
            InventoryItemUI inventorySlot = Instantiate(inventoryItemUI,inventoryScrollView.content.transform).GetComponent<InventoryItemUI>();
            inventorySlot.SetInventoryItemUI(inventoryItem);
        }
        if(inventory.Count <= 2)
            inventoryScrollView.content.transform.localPosition = new Vector3(scrollViewExtend / 2,0,0);
    }
}
