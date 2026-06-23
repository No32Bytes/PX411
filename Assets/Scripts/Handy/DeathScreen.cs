using UnityEngine;
using UnityEngine.SceneManagement;

class DeathScreen : MonoBehaviour
{
    public string menuManagerScenceString;
    public void OnRespawnButtonOnClick()
    {
        GlobalDataStore.GetStateManager().menuManger.loadIntoGameScenceDirect = true;
        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = true;
        OnTitleMenuButtonOnClick();
    }

    public void OnTitleMenuButtonOnClick()
    {
        SceneManager.LoadScene(menuManagerScenceString);
    }
}