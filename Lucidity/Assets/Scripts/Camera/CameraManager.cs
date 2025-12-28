using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public Transform normalCamera;
    public CameraMode currentMode;

    public List<CameraMode> cameraModes;

    void Start()
    {

    }

    private void Update()
    {
        //Testeo para probar que funcione
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SetMode(cameraModes[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeactivateMode();
        }
    }

    //Funcion para activar el modo de la camera deseado
    public void SetMode(CameraMode mode)
    {
        DeactivateMode();

        if (!mode.isUnlocked) return;

        currentMode = mode;
        currentMode.ActivateMode();

        NotifyModeActivated(mode);
    }

    //Funcion para desactivar el modo de la camera
    public void DeactivateMode()
    {
        if (currentMode == null) return;
        
        NotifyModeDeactivated(currentMode);
        currentMode.DeactivateMode();
        currentMode = null;
        
    }

    private void NotifyModeActivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsOfType<Anomaly>())
            anomaly.OnCameraModeActivated(mode);
    }

    private void NotifyModeDeactivated(CameraMode mode)
    {
        foreach (var anomaly in FindObjectsOfType<Anomaly>())
            anomaly.OnCameraModeDeactivated(mode);
    }
}
