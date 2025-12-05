using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public void StartButtonOnClick()
    {
        GlobalDataStore.Instance.menuManager.TitleMenuOpen = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitButtonOnClick()
    {
        Application.Quit();
    }
}

