using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(ItemDataBase))]
class ItemDataBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate ItemDataBase content"))
            GenerateContent();
    }
    private void GenerateContent()
    {
        ItemDataBase itemDataBase = target as ItemDataBase;
        List<ItemData> itemDataBaseList = itemDataBase.EditorGetItemDataContent();
        itemDataBaseList.Clear();

        string[] itemDatGuid = AssetDatabase.FindAssets($"t:{typeof(ItemData).Name}");
        foreach(string guid in itemDatGuid)
        {
            ItemData itemData = AssetDatabase.LoadAssetByGUID<ItemData>(new GUID(guid));
            itemDataBaseList.Add(itemData);
        }

        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
}