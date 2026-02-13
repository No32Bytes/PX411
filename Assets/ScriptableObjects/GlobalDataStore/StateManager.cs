using UnityEngine;
using UnityEngine.EventSystems;

public class StateManager
{
    public class MenuManger
    {
        public bool TitleMenuOpen = true;
        public const int MenuMangerScenceId = 0;
        public Camera menuOverlayCameraTarget;
    };
    public class Player
    {
        public bool unLoadPauseMenuSignal = false;
        public int unLoadPauseMenuSceneCount = 0;  
    };
    public MenuManger menuManger = new();
    public Player player = new();
    public EventSystem eventSystem;
};