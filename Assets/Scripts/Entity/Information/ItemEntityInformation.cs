using TMPro;
using UnityEngine;

class ItemEntityInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text itemEntityName;
    private GameObject sourceGameObject;
    private ItemEntity itemEntity;
    public void Awake()
    {
        sourceGameObject = EntityInformationView.Current.SourceGameObject;
        if (!sourceGameObject.TryGetComponent(out itemEntity))
            return;

        itemEntityName.text = itemEntity.ItemData.displayName;
    }
};