using UnityEngine;

class BaseEntityInformation : MonoBehaviour
{
    public GameObject SourceGameObject => EntityInformationView.Current.gameObject;
}
