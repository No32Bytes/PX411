using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    private void DrawInternalNameHashCopySection()
    {
        ItemData itemData = target as ItemData;
        string hash = itemData.InternalNameIntHash.ToString();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(nameof(itemData.InternalNameIntHash));
        if (GUILayout.Button(hash))
        {
            GUIUtility.systemCopyBuffer = hash;
        }
        EditorGUILayout.EndHorizontal();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (target == null)
            return;

        DrawInternalNameHashCopySection();
    }
}
