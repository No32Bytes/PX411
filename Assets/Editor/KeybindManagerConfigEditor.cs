using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(KeybindManagerConfig))]
public class KeybindManagerConfigEditor : Editor
{
    KeybindManagerConfig keybindManagerConfig;
    private int selectActionIndex = 0;
    private string selectActionString = "None";

    private void SaveThis()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
    private void DrawSectionHeader(KeybindManagerConfig.KeybindMangerData data)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(data.actionName, EditorStyles.boldLabel);
        GUILayout.Label($"{data.relativeBindingIndex} - {data.GetAction().bindings[data.relativeBindingIndex].path}", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
    }
    private void DrawSectionBody(KeybindManagerConfig.KeybindMangerData data)
    {
        string displayName = EditorGUILayout.DelayedTextField(data.displayName);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Show Keybind");
        bool showKeybind = EditorGUILayout.Toggle(data.showKeybind);
        EditorGUILayout.EndHorizontal();

        if (displayName != data.displayName)
        {
            data.displayName = displayName;
            SaveThis();
        }

        if (showKeybind != data.showKeybind)
        {
            data.showKeybind = showKeybind;
            SaveThis();
        }
    }
    private void DrawSection(KeybindManagerConfig.KeybindMangerData data)
    {


        EditorGUILayout.BeginVertical(new GUIStyle("AC BoldHeader"));

        DrawSectionHeader(data);
        DrawSectionBody(data);

        EditorGUILayout.EndVertical();
    }

    private void DrawBaseConfigSelector()
    {
        EditorGUILayout.BeginVertical(new GUIStyle("AC BoldHeader"));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Show only enabled Keybinds");
        bool showOnlyEnabled = EditorGUILayout.Toggle(keybindManagerConfig.showOnlyEnabled);
        EditorGUILayout.EndHorizontal();

        foreach (InputActionMap map in InputSystem.actions.actionMaps)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(map.name);
            bool showMap = EditorGUILayout.Toggle(keybindManagerConfig.activeActionMaps.Contains(map.name));
            EditorGUILayout.EndHorizontal();

            if (showMap != keybindManagerConfig.activeActionMaps.Contains(map.name))
            {
                if (keybindManagerConfig.activeActionMaps.Contains(map.name))
                    while (keybindManagerConfig.activeActionMaps.Remove(map.name)) ;
                else
                    keybindManagerConfig.activeActionMaps.Add(map.name);
            }
        }

        List<string> actionNames = new();
        actionNames.Add("None");

        foreach (KeybindManagerConfig.KeybindMangerData data in keybindManagerConfig.configData)
        {
            if (actionNames.Contains(data.actionName))
                continue;
            actionNames.Add(data.actionName);
        }

        selectActionIndex = EditorGUILayout.Popup("Show target Action", selectActionIndex, actionNames.ToArray());
        selectActionString = actionNames[selectActionIndex];

        EditorGUILayout.EndVertical();

        if (showOnlyEnabled != keybindManagerConfig.showOnlyEnabled)
            keybindManagerConfig.showOnlyEnabled = showOnlyEnabled;

    }
    private void AddMissingActionsToConfig()
    {
        foreach (InputActionMap map in InputSystem.actions.actionMaps)
        {
            foreach (InputAction action in map)
            {
                List<KeybindManagerConfig.KeybindMangerData> actionConfig = keybindManagerConfig.configData.FindAll(data => data.actionName == action.name);

                AddMissingBindingToConfig(actionConfig, action);
            }
        }

    }
    private void AddMissingBindingToConfig(List<KeybindManagerConfig.KeybindMangerData> actionConfig, InputAction inputAction)
    {
        for(int i = 0; i < inputAction.bindings.Count; i++)
        {
            InputBinding inputBinding = inputAction.bindings[i];
            int index = actionConfig.FindIndex(data => data.relativeBindingIndex == i);
            if (index != -1)
                continue;
            if (inputBinding.isComposite)
                continue;

            KeybindManagerConfig.KeybindMangerData keybindMangerData = new()
            {
                actionName = inputAction.name,
                relativeBindingIndex = i,
            };
            keybindManagerConfig.configData.Add(keybindMangerData);
        }
    }

    public override void OnInspectorGUI()
    {
        keybindManagerConfig = (target as KeybindManagerConfig);
        AddMissingActionsToConfig();

        DrawBaseConfigSelector();

        foreach (KeybindManagerConfig.KeybindMangerData data in keybindManagerConfig.configData)
        {
            if (keybindManagerConfig.showOnlyEnabled && !data.showKeybind)
                continue;
            if (!keybindManagerConfig.activeActionMaps.Contains(data.GetAction().actionMap.name))
                continue;
            if (selectActionString != "None" && data.actionName != selectActionString)
                continue;
            DrawSection(data);
        }
    }


}