using UnityEngine;
using UnityEngine.SceneManagement;

class CustomEventToScence : BaseCustomEventExecute
{
    [SerializeField] private string sceneName;
    public override void Execute()
    {
        SceneManager.LoadScene(sceneName);
    }
}