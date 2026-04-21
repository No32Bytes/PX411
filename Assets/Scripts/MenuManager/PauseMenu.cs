using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MenuManager menuManagerRef;
    public void ReturnButtonOnClick()
    {
        menuManagerRef.RemoveOverlayCameraFromTargetCamera();

        GlobalDataStore.GetStateManager().playerState.signalUnloadPauseMenu.TriggerSignal(SceneManager.sceneCount);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
    }
    public void ReturnTitleMenuButtonOnClick()
    {
        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = true;
        menuManagerRef.ForceAwake();
        menuManagerRef.RemoveOverlayCameraFromTargetCamera();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
    }

   void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
         ReturnButtonOnClick();
        }
    }
}

