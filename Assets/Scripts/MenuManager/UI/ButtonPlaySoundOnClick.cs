using UnityEngine;
using UnityEngine.UI;

public class ButtonPlaySoundOnClick : MonoBehaviour
{
    [SerializeField] private BaseSoundEffect selectSound;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(ButtonOnClick);
    }

    private void ButtonOnClick()
    {
        var audioSource = GlobalDataStore.GetAudioManager().GlobalSoundAudioSource;
        AudioUtil.PlaySoundEffect(selectSound, audioSource);
    }
}
