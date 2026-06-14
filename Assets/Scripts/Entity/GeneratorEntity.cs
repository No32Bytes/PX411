using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Generator : BaseEntity
{
    [System.Serializable]
    public struct GeneratorItemRequirement
    {
        public ItemData targetItemData;
        public int requiredAmount;
    }
    [Header("Items")]
    [SerializeField] private GeneratorItemRequirement screwRequirement;
    [SerializeField] private GeneratorItemRequirement cableRequirement;
    [SerializeField] private GeneratorItemRequirement fuelRequirement;
    [SerializeField] private GeneratorItemRequirement oilRequirement;
    [SerializeField] private ItemData hammerItemData;
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
    [SerializeField] private BaseSoundEffect screwSound;
    [SerializeField] private BaseSoundEffect cableSound;
    [SerializeField] private BaseSoundEffect fuelSound;
    [SerializeField] private BaseSoundEffect oilSound;
    [SerializeField] private BaseSoundEffect hammerSound;
    [SerializeField] private BaseSoundEffect failSound;
    [SerializeField] private BaseSoundEffect startingSound;
    [SerializeField] private BaseSoundEffect runningSound;

    [Header("Visuals")]
    [SerializeField] private GameObject brokenVisuals;
    [SerializeField] private GameObject repairedVisuals;

    private AudioSource generatorAudioSource;
    private bool IsRepaired => data.isRepaired;

    enum MotorState
    {
        Disabled,
        Starting,
        Running,
    }
    MotorState motorState;

    private void SaveData()
    {
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
            if (TryInstallStoreableItem(inventory, itemHandler, ref screwRequirement, ref data.screws, screwSound)) return;
            if (TryInstallStoreableItem(inventory, itemHandler, ref cableRequirement, ref data.cables, cableSound)) return;
            if (TryInstallStoreableItem(inventory, itemHandler, ref fuelRequirement, ref data.fuel, fuelSound)) return;
            if (TryInstallStoreableItem(inventory, itemHandler, ref oilRequirement, ref data.oil, oilSound)) return;

            //Debug.Log("Du hast keine passenden Teile (Schrauben, Kabel, Benzin oder Öl) im Inventar!");
            AudioUtil.PlaySoundEffect(failSound, generatorAudioSource);
            return;
        }

        if (allPartsInstalled)
        {
            if (itemHandler.EquippedItemInternalName == hammerItemData.internalName)
            {
                data.isRepaired = true;
                SaveData();
                AudioUtil.PlaySoundEffect(hammerSound, generatorAudioSource);
                UpdateGeneratorState();
                //Debug.Log("REPARATUR ERFOLGREICH! Der Generator brummt!");
            }
            else
            {
                //Debug.Log($"Alle Teile sind verbaut! Rüste jetzt den Hammer ({hammerItemData.internalName}) aus, um den Generator final zu reparieren!");
                AudioUtil.PlaySoundEffect(failSound, generatorAudioSource);
            }
        }
    }

    private bool TryInstallStoreableItem(Inventory inventory, PlayerItemHandler itemHandler, ref GeneratorItemRequirement req, ref int currentAmount, BaseSoundEffect actionSound)
    {
        if (currentAmount >= req.requiredAmount) return false;

        if (inventory.GetInventoryItem(req.targetItemData.internalName, out InventoryItem item))
        {
            if (item.RemoveItemForever())
            {
                currentAmount++;
                //Debug.Log($"[Generator] {req.internalName} installiert! ({req.currentAmount}/{req.requiredAmount})");

                AudioUtil.PlaySoundEffect(actionSound, generatorAudioSource);

                if (itemHandler.EquippedItemInternalName == req.targetItemData.internalName)
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
        return data.screws >= screwRequirement.requiredAmount &&
               data.cables >= cableRequirement.requiredAmount &&
               data.fuel >= fuelRequirement.requiredAmount &&
               data.oil >= oilRequirement.requiredAmount;
    }

    private void UpdateGeneratorState()
    {
        if (brokenVisuals != null) brokenVisuals.SetActive(!IsRepaired);
        if (repairedVisuals != null) repairedVisuals.SetActive(IsRepaired);

        if (IsRepaired && motorState == MotorState.Disabled)
            motorState = MotorState.Starting;
    }

    private void FixedUpdate()
    {
        if (!IsRepaired)
            return;
        if (generatorAudioSource.isPlaying)
            return;

        switch (motorState)
        {
            case MotorState.Disabled:
                return;
            case MotorState.Starting:
                AudioUtil.PlaySoundEffect(startingSound, generatorAudioSource);
                motorState = MotorState.Running;
                return;
            case MotorState.Running:
                generatorAudioSource.clip = null;
                generatorAudioSource.loop = true;
                AudioUtil.PlaySoundEffect(runningSound, generatorAudioSource);
                return;
            default:
                break;
        }
    }
}