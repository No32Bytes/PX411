using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MenuManager menuManagerRef;
    public void ReturnButtonOnClick()
    {
        menuManagerRef.RemoveOverlayCameraFromTargetCamera();

        GlobalDataStore.GetStateManager().player.unLoadPauseMenuSceneCount = SceneManager.sceneCount;
        GlobalDataStore.GetStateManager().player.unLoadPauseMenuSignal = true;

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
    }
    public void ReturnTitleMenuButtonOnClick()
    {
        
    }
}
