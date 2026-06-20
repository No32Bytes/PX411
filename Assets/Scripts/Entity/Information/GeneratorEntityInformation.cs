using TMPro;
using UnityEngine;
class GeneratorEntityInformation : BaseEntityInformation
{
    [System.Serializable]
    struct ItemRequirementDisplay
    {
        public TMP_Text displayName;
        public TMP_Text displayAmount;
        public GameObject completedActive;
    };

    [SerializeField] private ItemRequirementDisplay[] itemRequirementDisplays;
    [SerializeField] private TMP_Text lastInfoDisplay;
    GeneratorEntity GeneratorObject => SourceGameObject.GetComponent<GeneratorEntity>();
    GeneratorEntity.GeneratorData generatorStateHere;
    private void Awake()
    {
        UpdateDisplay();
    }

    private void FixedUpdate()
    {
        if (generatorStateHere.Equals(GeneratorObject.GeneratorState))
            return;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        generatorStateHere = GeneratorObject.GeneratorState;
        if (itemRequirementDisplays.Length != 4)
        {
            Debug.Log("Length of ItemRequirementDisplays must be 4");
            return;
        }

        UpdateDisplayRequirement(itemRequirementDisplays[0], GeneratorObject.screwRequirement, generatorStateHere.screws);
        UpdateDisplayRequirement(itemRequirementDisplays[1], GeneratorObject.cableRequirement, generatorStateHere.cables);
        UpdateDisplayRequirement(itemRequirementDisplays[2], GeneratorObject.fuelRequirement, generatorStateHere.fuel);
        UpdateDisplayRequirement(itemRequirementDisplays[3], GeneratorObject.oilRequirement, generatorStateHere.oil);

        if (generatorStateHere.isRepaired)
            lastInfoDisplay.text = "Der Generator läuft und hat etwas geöffnet";
        else if (GeneratorObject.AllPartsInstalled())
            lastInfoDisplay.text = "Ein Hammer wird benötigt";
        else
            lastInfoDisplay.text = "???";
    }

    private void UpdateDisplayRequirement(ItemRequirementDisplay display, GeneratorEntity.GeneratorItemRequirement itemRequirement, int currentAmount)
    {
        display.displayName.text = itemRequirement.targetItemData.displayName;
        display.displayAmount.text = currentAmount + "/" + itemRequirement.requiredAmount;
        display.completedActive.SetActive(itemRequirement.requiredAmount == currentAmount);
    }
}