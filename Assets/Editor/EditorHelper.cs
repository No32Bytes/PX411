using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class EditorHelper
{
    public static void FindAssetsAndSaveToList<AssetType>(List<AssetType> assetList,Object target) 
        where AssetType : Object
    {
        assetList.Clear();

        GUID[] assetGuids = AssetDatabase.FindAssetGUIDs($"t:{typeof(AssetType).Name}");
        foreach(GUID guid in assetGuids)
        {
            AssetType asset = AssetDatabase.LoadAssetByGUID<AssetType>(guid);
            assetList.Add(asset);
        }

        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
}