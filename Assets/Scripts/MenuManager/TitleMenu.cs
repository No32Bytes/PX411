using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text currentSaveIDLabel;
    void OnEnable()
    {
        currentSaveIDLabel.SetText(GlobalDataStore.Instance.saveManager.CurrentSaveID);
    }

    public void StartButtonOnClick()
    {
        GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitButtonOnClick()
    {
        Application.Quit();
    }
}

