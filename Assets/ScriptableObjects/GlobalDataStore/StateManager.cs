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
        public SignalUtil.SignalUnloadPauseMenu signalUnloadPauseMenu = new();
        public Player player;
        public PlayerReferences playerRef;
        public PlayerItemHandler playerItemHandler;
        public PlayerLook playerLook;
        public string lastSoundTrackGroup; 
        public string lastSoundTrackId;
        public float lastSoundTrackTime;
    };
    public MenuMangerState menuManger = new();
    public PlayerState playerState = new();
    public EventSystem eventSystem;
};