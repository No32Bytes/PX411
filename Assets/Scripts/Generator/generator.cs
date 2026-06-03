using UnityEngine;

public class Generator : BaseEntity
{
    [Header("Generator Settings")]
    [SerializeField] private string hammerInternalName = "hammer";
    [SerializeField] private string screwInternalName = "screw";
    [SerializeField] private int requiredScrews = 4;

    [Header("Generator State")]
    [SerializeField] private bool isRepaired = false;
    [SerializeField] private int currentScrewsInstalled = 0;

    [Header("Visuals / Audio (Optional für Test)")]
    [SerializeField] private GameObject brokenVisuals;
    [SerializeField] private GameObject repairedVisuals;
    [SerializeField] private AudioSource generatorAudioSource;
    [SerializeField] private SimpleSoundEffect repairSoundEffect;

    public bool IsRepaired => isRepaired;

    protected override void EntityAwake()
    {
        UpdateGeneratorState();
    }

    public override void EntityInteraction()
    {
        if (isRepaired)
        {
            Debug.Log("Der Generator läuft bereits fleißig!");
            return;
        }

        var stateManager = GlobalDataStore.GetStateManager();
        if (stateManager == null || stateManager.playerState == null || stateManager.playerState.playerItemHandler == null)
        {
            Debug.LogError("[Generator] PlayerItemHandler konnte nicht gefunden werden!");
            return;
        }

        PlayerItemHandler itemHandler = stateManager.playerState.playerItemHandler;

        if (itemHandler.EquippedItemInternalName != hammerInternalName)
        {
            Debug.Log($"[TEST] Du hältst nicht den Hammer! Aktuell in der Hand: '{itemHandler.EquippedItemInternalName}'");
            return;
        }

        var inventory = GlobalDataStore.GetInventory();
        if (inventory.GetCollectableInventoryItem(screwInternalName, out var screwItem))
        {
            if (currentScrewsInstalled < requiredScrews)
            {
                currentScrewsInstalled++;
                Debug.Log($"[TEST] Schraube festgedreht! ({currentScrewsInstalled}/{requiredScrews})");

                
                if (repairSoundEffect != null && generatorAudioSource != null)
                    repairSoundEffect.Play(generatorAudioSource);

                inventory.DropItem(screwInternalName, out _); 
            }

            if (currentScrewsInstalled >= requiredScrews)
            {
                isRepaired = true;
                UpdateGeneratorState();
                Debug.Log("🎉 [TEST] DER GENERATOR IST REPARIERT!");
            }
        }
        else
        {
            Debug.Log($"[TEST] Dir fehlen Schrauben im Inventar! Gesucht wird: '{screwInternalName}'");
        }
    }

    private void UpdateGeneratorState()
    {
        
        if (brokenVisuals != null) brokenVisuals.SetActive(!isRepaired);
        if (repairedVisuals != null) repairedVisuals.SetActive(isRepaired);

        
        if (isRepaired)
        {
            if (generatorAudioSource != null && !generatorAudioSource.isPlaying)
            {
                generatorAudioSource.loop = true;
                generatorAudioSource.Play();
            }
        }
        else
        {
            if (generatorAudioSource != null)
                generatorAudioSource.Stop();
        }
    }
}