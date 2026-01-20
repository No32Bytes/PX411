using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public void ReturnButtonOnClick()
    {
        SceneManager.LoadScene(GlobalDataStore.GetStateManager().menuManger.returnScenceID);
    }
}
