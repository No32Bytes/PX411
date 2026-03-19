using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(fileName = "KeybindManagerConfig", menuName = "MenuManger/KeybindMangerConfig")]
public class KeybindManagerConfig : ScriptableObject
{
    public bool showOnlyEnabled;
    public List<string> activeActionMaps = new();
    public List<KeybindMangerData> configData = new();
    [Serializable]
    public class KeybindMangerData
    {
        public string actionName;
        public int relativeBindingIndex;
        public string displayName;
        public bool showKeybind;
        public bool CheckActionValid()
        {
            try
            {
                GetAction();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public InputAction GetAction()
        {
            foreach (InputActionMap map in InputSystem.actions.actionMaps)
            {
                foreach (InputAction action in map)
                {
                    if (action.name == actionName)
                        return action;
                }
            }
            throw new Exception($"{actionName} should exist as an action in the InputAction asset.");
        }
    }
}