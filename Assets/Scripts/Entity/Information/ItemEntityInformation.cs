using TMPro;
using UnityEngine;

class ItemEntityInformation : BaseEntityInformation
{
    [SerializeField] private TMP_Text itemEntityName;
    public void Awake()
    {
        if (!SourceGameObject.TryGetComponent(out ItemEntity itemEntity))
            return;

        itemEntityName.text = itemEntity.ItemData.displayName;
    }
};