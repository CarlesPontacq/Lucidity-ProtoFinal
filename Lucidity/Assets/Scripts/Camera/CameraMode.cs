using UnityEngine;

public abstract class CameraMode : MonoBehaviour
{
    [Header("General")]
    public bool isUnlocked;
    public bool isActive { get; private set; }

    protected bool unlocked;

    [SerializeField] protected CameraUIHandler ui;
    [SerializeField] protected CameraAudioHandler audioHandler;


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
        OnActivated();
    }

    //Funcion para desactivar la camara
    public virtual void DeactivateMode() 
    {
        isActive = false;
        OnDeactivated();
    }

    public abstract void PerformCameraAction();

    public abstract void LookThroughCamera(bool look);

    protected virtual void OnActivated() { }

    protected virtual void OnDeactivated() { }
}
