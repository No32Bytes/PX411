using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    public Transform playerBody;
    public Camera playerCamera;
    public GameObject handyScreenUI;
    public GameObject leftPlayerArmItemAnchor;
    public GameObject overlayFPS;
    public GameObject overlayInformationView;

    private void Awake()
    {
        GlobalDataStore.GetStateManager().playerState.playerRef = this;
    }
}