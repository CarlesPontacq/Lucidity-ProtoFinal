using UnityEngine;

public class UltravioletMode : CameraMode
{
    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        base.Update();
    }

    //Funcion para activar la camara
    public override void ActivateMode()
    {
        base .ActivateMode();
    }

    //Funcion para desactivar la camara
    public override void DeactivateMode()
    {
        base.DeactivateMode();
    }

    public override void PerformCameraAction() { }

    protected override void OnActivated() { }

    protected override void OnDeactivated() { }
}
