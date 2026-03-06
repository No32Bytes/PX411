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
        List<ItemData> itemDataBaseList = (target as ItemDataBase).InternalGetItemDataBaseList;
        EditorHelper.FindAssetsAndSaveToList(itemDataBaseList,target);
    }
}