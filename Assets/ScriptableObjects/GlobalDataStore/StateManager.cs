using UnityEngine;
using UnityEngine.EventSystems;

public class StateManager
{
    public class MenuMangerState
    {
        public bool TitleMenuOpen = true;
        public int MenuMangerScenceId = 0;
        public Camera menuOverlayCameraTarget;
    };
    public class PlayerState
    {
        public bool unLoadPauseMenuSignal = false;
        public int unLoadPauseMenuSceneCount = 0;  
        public Player playerReference;
    };
    public MenuMangerState menuManger = new();
    public PlayerState player = new();
    public EventSystem eventSystem;
};