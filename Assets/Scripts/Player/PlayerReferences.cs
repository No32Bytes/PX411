using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    public Transform playerBody;
    public Camera playerCamera;
    public Camera playerOverlayCamera;
    public GameObject handyScreenUI;
    public GameObject leftPlayerArmItemAnchor;
    public GameObject overlayFPS;
    public GameObject overlayInformationView;

    private void Awake()
    {
        GlobalDataStore.GetStateManager().playerState.playerRef = this;
    }
}