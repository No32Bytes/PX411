using UnityEngine;

public class FuseBoxPuzzle : MonoBehaviour
{
    [System.Serializable]
    public struct LeverRequirement
    {
        public string leverColor;
        public int requiredPosition;
    }

    [Header("Richtige Hebelstellungen")]
    public LeverRequirement[] requiredLevers = new LeverRequirement[3];

    [Header("Ziel-Objekt (z.B. die Tür, die VERSCHWINDEN soll)")]
    public GameObject targetToDeactivate;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private BaseSoundEffect leverClickSound;
    [SerializeField] private BaseSoundEffect powerOnSound;

    private int[] currentPositions = new int[3];
    private bool isSolved = false;

    private void Start()
    {
        if (targetToDeactivate != null) targetToDeactivate.SetActive(true);
    }

    public void UpdateLeverPosition(string color, int position)
    {
        if (isSolved) return;

        AudioUtil.PlaySoundEffect(leverClickSound, audioSource);

        for (int i = 0; i < requiredLevers.Length; i++)
        {
            if (requiredLevers[i].leverColor.ToLower() == color.ToLower())
            {
                currentPositions[i] = position;
                
                if (position == requiredLevers[i].requiredPosition)
                {
                    Debug.Log("<color=green>[Rätsel-Check]</color> Hebel <b>" + color + "</b> steht jetzt in der <b>RICHTIGEN</b> Position!");
                }
                else
                {
                    Debug.Log("<color=orange>[Rätsel-Check]</color> Hebel <b>" + color + "</b> wurde bewegt, ist aber in dieser Position noch <b>FALSCH</b>.");
                }
                break;
            }
        }

        CheckCombination();
    }

    private void CheckCombination()
    {
        for (int i = 0; i < requiredLevers.Length; i++)
        {
            if (currentPositions[i] != requiredLevers[i].requiredPosition)
            {
                return;
            }
        }

        SolvePuzzle();
    }

    private void SolvePuzzle()
    {
        isSolved = true;

        Debug.Log("<color=green><b>[Rätsel-Erfolg] ALLE HEBEL STEHEN KORREKT! Das Rätsel ist gelöst!</b></color>");

        AudioSource sucess = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        AudioUtil.PlaySoundEffect(powerOnSound, sucess);

        if (targetToDeactivate != null)
        {
            targetToDeactivate.SetActive(false);
        }
    }
}