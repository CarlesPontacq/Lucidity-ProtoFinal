using UnityEngine;

public class CameraFunctionality : MonoBehaviour
{
    [Header("General")]
    public bool isUnlocked;
    public bool isActive { get; private set; }

    protected bool unlocked;
    public bool isPerformingAction = false;

    [SerializeField] protected CameraUIHandler ui;
    [SerializeField] protected CameraAudioHandler audioHandler;

    [Header("Observation Function")]
    [SerializeField] private Camera normalCamera;

    [Header("Post-Process")]
    [SerializeField] protected GameObject globalVolumeMode;

    void Start()
    {
        ui = FindAnyObjectByType<CameraUIHandler>();
        audioHandler = FindAnyObjectByType<CameraAudioHandler>();
    }


    void Update()
    {
        
    }

    public void ActivateMode()
    {
        isActive = true;
        globalVolumeMode.SetActive(true);

        normalCamera.enabled = false;
    }

    public void DeactivateMode()
    {
        isActive = false;
        globalVolumeMode.SetActive(false);

        normalCamera.enabled = true;
    }

    public void PerformCameraPhoto()
    {

    }
}
