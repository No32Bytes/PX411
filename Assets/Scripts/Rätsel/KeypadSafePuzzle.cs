using UnityEngine;

public class KeypadSafePuzzle : MonoBehaviour
{
    [System.Serializable]
    public struct ColorCombination
    {
        public string colorName;
        public int requiredNumber;
    }

    [Header("Richtige Kombination & Reihenfolge")]
    public ColorCombination[] correctOrder = new ColorCombination[4];

    [Header("Safe Visuals (Zustände)")]
    public GameObject closedSafeVisual;
    public GameObject openedSafeVisual;

    [Header("Belohnung")]
    public GameObject generatorItemPrefab;
    public Transform spawnPoint;

    [Header("Audio-Quellen")]
    [SerializeField] private AudioSource buttonsAudioSource;
    [SerializeField] private AudioSource feedbackAudioSource;

    [Header("Audio-Clips")]
    [SerializeField] private BaseSoundEffect buttonClickSound;
    [SerializeField] private BaseSoundEffect failResetSound;
    [SerializeField] private BaseSoundEffect correctPhaseSound;
    [SerializeField] private BaseSoundEffect safeOpenSound;


    private string[] playerColorInputs = new string[4];
    private int[] playerNumberInputs = new int[4];
    
    private int colorIndex = 0;
    private int numberIndex = 0;
    
    private bool colorsValidated = false;
    private bool isUnlocked = false;

    private void Start()
    {
        if (closedSafeVisual != null) closedSafeVisual.SetActive(true);
        if (openedSafeVisual != null) openedSafeVisual.SetActive(false);
    }

    public void SelectColor(string colorName)
    {
        if (isUnlocked || colorsValidated) return;

        AudioUtil.PlaySoundEffect(buttonClickSound, buttonsAudioSource);
        playerColorInputs[colorIndex] = colorName;
        colorIndex++;

        if (colorIndex == 4)
        {
            CheckColorOrder();
        }
    }

    public void PressNumberButton(int pressedNumber)
    {
        if (isUnlocked) return;

        if (!colorsValidated)
        {
            AudioUtil.PlaySoundEffect(buttonClickSound, buttonsAudioSource);
            return;
        }

        AudioUtil.PlaySoundEffect(buttonClickSound, buttonsAudioSource);

        playerNumberInputs[numberIndex] = pressedNumber;
        numberIndex++;

        if (numberIndex == 4)
        {
            CheckNumberOrder();
        }
    }

    private void CheckColorOrder()
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerColorInputs[i].ToLower() != correctOrder[i].colorName.ToLower())
            {
                AudioUtil.PlaySoundEffect(failResetSound, feedbackAudioSource);

                ResetCode();
                return;
            }
        }

        colorsValidated = true;
        AudioUtil.PlaySoundEffect(correctPhaseSound, feedbackAudioSource);
    }

    private void CheckNumberOrder()
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerNumberInputs[i] != correctOrder[i].requiredNumber)
            {
                AudioUtil.PlaySoundEffect(failResetSound, feedbackAudioSource);
                numberIndex = 0;
                for (int j = 0; j < playerNumberInputs.Length; j++) playerNumberInputs[j] = 0;
                return;
            }
        }

        UnlockSafe();
    }

    public void ResetCode()
    {
        if (isUnlocked) return;

        colorIndex = 0;
        numberIndex = 0;
        colorsValidated = false;

        for (int i = 0; i < 4; i++)
        {
            playerColorInputs[i] = "";
            playerNumberInputs[i] = 0;
        }
    }

    private void UnlockSafe()
    {
        isUnlocked = true;
        AudioUtil.PlaySoundEffect(safeOpenSound, feedbackAudioSource);

        if (closedSafeVisual != null) closedSafeVisual.SetActive(false);
        if (openedSafeVisual != null) openedSafeVisual.SetActive(true);

        if (generatorItemPrefab != null && spawnPoint != null)
        {
            Instantiate(generatorItemPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private void PlaySound(AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }
    }
}