using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeldItemData))]
class HeldItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(target == null)
            return;

        string hash = "";
        if ((target as HeldItemData).GeldHeldItemAniamtionIdHash() != 0)
            hash = (target as HeldItemData).GeldHeldItemAniamtionIdHash().ToString();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("HeldItemDataIdHash");
        if (GUILayout.Button(hash))
        {
            GUIUtility.systemCopyBuffer = hash;
        }
        EditorGUILayout.EndHorizontal();

    }
}