using UnityEngine;

public abstract class CameraMode : MonoBehaviour
{
    [Header("General")]
    public bool isUnlocked;
    public bool isActive { get; private set; }

    protected bool unlocked;

    [SerializeField] protected CameraUIHandler ui;
    [SerializeField] protected CameraAudioHandler audioHandler;

    [Header("Post-Process")]
    [SerializeField] protected GameObject globalVolumeMode;

    protected void Start()
    {
        ui = FindAnyObjectByType<CameraUIHandler>();
        audioHandler = FindAnyObjectByType<CameraAudioHandler>();
    }

    protected void Update()
    {
        
    }

    //Funcion para activar la camara
    public virtual void ActivateMode()
    {
        isActive = true;
        globalVolumeMode.SetActive(true);
        OnActivated();
    }

    //Funcion para desactivar la camara
    public virtual void DeactivateMode() 
    {
        isActive = false;
        globalVolumeMode.SetActive(false);
        OnDeactivated();
    }

    public abstract void PerformCameraAction();

    protected virtual void OnActivated() { }

    protected virtual void OnDeactivated() { }
}
