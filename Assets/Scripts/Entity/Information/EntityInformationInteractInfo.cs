using TMPro;
using UnityEngine;

class EntityInformationInteractInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text interactText;
    public void SetText(string text)
    {
        interactText.text = text;
    }
};

