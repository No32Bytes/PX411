using UnityEngine;

public class Generator : BaseEntity
{
    [System.Serializable]
    public struct GeneratorItemRequirement
    {
        public string internalName;
        public int requiredAmount;
        [HideInInspector] public int currentAmount;
    }

    [Header("Item Einstellungen")]
    [SerializeField] private string hammerInternalName = "hammer";
    [SerializeField] private GeneratorItemRequirement screwRequirement = new GeneratorItemRequirement { internalName = "screw", requiredAmount = 4 };
    [SerializeField] private GeneratorItemRequirement cableRequirement = new GeneratorItemRequirement { internalName = "cable", requiredAmount = 1 };
    [SerializeField] private GeneratorItemRequirement fuelRequirement = new GeneratorItemRequirement { internalName = "fuel", requiredAmount = 1 };
    [SerializeField] private GeneratorItemRequirement oilRequirement = new GeneratorItemRequirement { internalName = "oil", requiredAmount = 1 };

    [Header("Generator Zustand")]
    [SerializeField] private bool isRepaired = false;

    [Header("Sounds")]
    [SerializeField] private AudioSource generatorAudioSource;
    [SerializeField] private SimpleSoundEffect screwSound;
    [SerializeField] private SimpleSoundEffect cableSound;
    [SerializeField] private SimpleSoundEffect fuelSound;
    [SerializeField] private SimpleSoundEffect oilSound;
    [SerializeField] private SimpleSoundEffect hammerSound;
    [SerializeField] private SimpleSoundEffect failSound;

    [Header("Visuals")]
    [SerializeField] private GameObject brokenVisuals;
    [SerializeField] private GameObject repairedVisuals;

    public bool IsRepaired => isRepaired;

    protected override void EntityAwake()
    {
        UpdateGeneratorState();
    }

    public override void EntityInteraction()
    {
        if (isRepaired)
        {
            Debug.Log("Der Generator läuft bereits!");
            return;
        }

        var inventory = GlobalDataStore.GetInventory();
        var stateManager = GlobalDataStore.GetStateManager();
        
        if (inventory == null || stateManager == null || stateManager.playerState == null || stateManager.playerState.playerItemHandler == null)
        {
            Debug.LogError("[Generator] Inventar oder PlayerItemHandler nicht gefunden!");
            return;
        }

        PlayerItemHandler itemHandler = stateManager.playerState.playerItemHandler;

        bool allPartsInstalled = AllPartsInstalled();

        if (!allPartsInstalled)
        {
            if (TryInstallStoreableItem(inventory, itemHandler, ref screwRequirement, screwSound)) return;
            if (TryInstallStoreableItem(inventory, itemHandler, ref cableRequirement, cableSound)) return;
            if (TryInstallStoreableItem(inventory, itemHandler, ref fuelRequirement, fuelSound)) return;
            if (TryInstallStoreableItem(inventory, itemHandler, ref oilRequirement, oilSound)) return;

            Debug.Log("Du hast keine passenden Teile (Schrauben, Kabel, Benzin oder Öl) im Inventar!");
            PlaySound(failSound);
            return;
        }

        if (allPartsInstalled)
        {
            if (itemHandler.EquippedItemInternalName == hammerInternalName)
            {
                isRepaired = true;
                PlaySound(hammerSound);
                UpdateGeneratorState();
                Debug.Log("REPARATUR ERFOLGREICH! Der Generator brummt!");
            }
            else
            {
                Debug.Log($"Alle Teile sind verbaut! Rüste jetzt den Hammer ({hammerInternalName}) aus, um den Generator final zu reparieren!");
                PlaySound(failSound);
            }
        }
    }

    private bool TryInstallStoreableItem(Inventory inventory, PlayerItemHandler itemHandler, ref GeneratorItemRequirement req, SimpleSoundEffect actionSound)
    {
        if (req.currentAmount >= req.requiredAmount) return false;

        if (inventory.GetStoreableInventoryItem(req.internalName, out _))
        {
            if (inventory.DropItem(req.internalName, out _))
            {
                req.currentAmount++;
                Debug.Log($"[Generator] {req.internalName} installiert! ({req.currentAmount}/{req.requiredAmount})");
                
                PlaySound(actionSound);

                if (itemHandler.EquippedItemInternalName == req.internalName)
                {
                    itemHandler.UnEquipCurrentItem();
                }

                return true;
            }
        }

        return false;
    }

    private bool AllPartsInstalled()
    {
        return screwRequirement.currentAmount >= screwRequirement.requiredAmount &&
               cableRequirement.currentAmount >= cableRequirement.requiredAmount &&
               fuelRequirement.currentAmount >= fuelRequirement.requiredAmount &&
               oilRequirement.currentAmount >= oilRequirement.requiredAmount;
    }

    private void PlaySound(SimpleSoundEffect effect)
    {
        if (effect != null && generatorAudioSource != null)
        {
            effect.Play(generatorAudioSource);
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