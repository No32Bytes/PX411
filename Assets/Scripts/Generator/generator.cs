using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] private ItemData screwItemData;
    [SerializeField] private int requiredScrews = 4;
    [SerializeField] private ItemData cableItemData;
    [SerializeField] private int requiredCables = 1;
    [SerializeField] private ItemData fuelItemData;
    [SerializeField] private int requiredFuel = 1;
    [SerializeField] private ItemData oilItemData;
    [SerializeField] private int requriedOil = 1;
    [SerializeField] private ItemData hammerItemData;
    private GeneratorItemRequirement screwRequirement;
    private GeneratorItemRequirement cableRequirement;
    private GeneratorItemRequirement fuelRequirement;
    private GeneratorItemRequirement oilRequirement;
    [System.Serializable]
    struct GeneratorData
    {
        public int screws;
        public int cables;
        public int fuel;
        public int oil;
        public bool isRepaired;
    };
    private GeneratorData data = new();

    [Header("Sounds")]
    [SerializeField] private SimpleSoundEffect screwSound;
    [SerializeField] private SimpleSoundEffect cableSound;
    [SerializeField] private SimpleSoundEffect fuelSound;
    [SerializeField] private SimpleSoundEffect oilSound;
    [SerializeField] private SimpleSoundEffect hammerSound;
    [SerializeField] private SimpleSoundEffect failSound;

    [Header("Visuals")]
    [SerializeField] private GameObject brokenVisuals;
    [SerializeField] private GameObject repairedVisuals;

    private AudioSource generatorAudioSource;
    private bool IsRepaired => data.isRepaired;

    protected override void EntityAwake()
    {
        screwRequirement = new() { internalName = screwItemData.internalName, requiredAmount = requiredScrews };
        cableRequirement = new() { internalName = cableItemData.internalName, requiredAmount = requiredCables };
        fuelRequirement = new() { internalName = fuelItemData.internalName, requiredAmount = requiredFuel };
        oilRequirement = new() { internalName = oilItemData.internalName, requiredAmount = requriedOil };
    }

    private void SaveData()
    {
        data.screws = screwRequirement.currentAmount;
        data.cables = cableRequirement.currentAmount;
        data.fuel = fuelRequirement.currentAmount;
        data.oil = oilRequirement.currentAmount;
        OnDisable();
    }
    private void OnDisable()
    {
        GlobalDataStore.GetSaveData().entityStateStore.SetEntityStateDataObject(GetBaseEntityId(), data);
    }

    private void Start()
    {
        GlobalDataStore.GetSaveData().entityStateStore.GetEntityStateDataObject(GetBaseEntityId(), out data);
        generatorAudioSource = GetComponent<AudioSource>();

        UpdateGeneratorState();
    }
    public override void EntityInteraction()
    {
        if (IsRepaired)
        {
            //Debug.Log("Der Generator läuft bereits!");
            return;
        }

        var inventory = GlobalDataStore.GetInventory();
        var stateManager = GlobalDataStore.GetStateManager();

        if (inventory == null || stateManager == null || stateManager.playerState == null || stateManager.playerState.playerItemHandler == null)
        {
            //Debug.LogError("[Generator] Inventar oder PlayerItemHandler nicht gefunden!");
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

            //Debug.Log("Du hast keine passenden Teile (Schrauben, Kabel, Benzin oder Öl) im Inventar!");
            PlaySound(failSound);
            return;
        }

        if (allPartsInstalled)
        {
            if (itemHandler.EquippedItemInternalName == hammerItemData.internalName)
            {
                data.isRepaired = true;
                SaveData();
                PlaySound(hammerSound);
                UpdateGeneratorState();
                //Debug.Log("REPARATUR ERFOLGREICH! Der Generator brummt!");
            }
            else
            {
                //Debug.Log($"Alle Teile sind verbaut! Rüste jetzt den Hammer ({hammerItemData.internalName}) aus, um den Generator final zu reparieren!");
                PlaySound(failSound);
            }
        }
    }

    private bool TryInstallStoreableItem(Inventory inventory, PlayerItemHandler itemHandler, ref GeneratorItemRequirement req, SimpleSoundEffect actionSound)
    {
        if (req.currentAmount >= req.requiredAmount) return false;

        if (inventory.GetStoreableInventoryItem(req.internalName, out InventoryItem item))
        {
            if (item.RemoveItemForever())
            {
                req.currentAmount++;
                //Debug.Log($"[Generator] {req.internalName} installiert! ({req.currentAmount}/{req.requiredAmount})");

                PlaySound(actionSound);

                if (itemHandler.EquippedItemInternalName == req.internalName)
                {
                    itemHandler.UnEquipCurrentItem();
                }

                SaveData();
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
        if (brokenVisuals != null) brokenVisuals.SetActive(!IsRepaired);
        if (repairedVisuals != null) repairedVisuals.SetActive(IsRepaired);

        if (IsRepaired)
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