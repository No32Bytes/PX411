using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MenuManager : MonoBehaviour
{

    public GameObject titleMenuRef;
    public GameObject pauseMenuRef;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera menuCamera;

    private void Awake()
    {
        if (GlobalDataStore.GetStateManager().menuManger.TitleMenuOpen)
        {
            return;   
        }
        
        mainCamera.enabled = false;
        titleMenuRef.SetActive(false);
        pauseMenuRef.SetActive(true);
        AddOverlayCameraToTargetCamera();
    }
    public void AddOverlayCameraToTargetCamera()
    {
        Camera targetCamera = GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget;
        targetCamera.GetUniversalAdditionalCameraData().cameraStack.Add(menuCamera);
    }
    public void RemoveOverlayCameraFromTargetCamera()
    {
        Camera targetCamera = GlobalDataStore.GetStateManager().menuManger.menuOverlayCameraTarget;
        targetCamera.GetUniversalAdditionalCameraData().cameraStack.Remove(menuCamera);
    }
}
