using UnityEngine;

public class TVInteraction : ObjectInteraction
{
    [SerializeField] GameObject screenOff;
    [SerializeField] GameObject screenOn;

    private bool isOn = false;

    public override void ResetState()
    {
        TurnOff();
    }

    public override void Interact()
    {
        if (isOn)
            TurnOff();
        else
            TurnOn();
    }

    private void TurnOn()
    {
        isOn = true;

        screenOff.SetActive(false);
        screenOn.SetActive(true);
    }

    private void TurnOff()
    {
        isOn = false;

        screenOff.SetActive(true);
        screenOn.SetActive(false);
    }
}
