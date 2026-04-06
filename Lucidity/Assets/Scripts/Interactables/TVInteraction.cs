using UnityEngine;

public class TVInteraction : ObjectInteraction
{
    private bool isOn = false;

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
        Debug.Log("TV is now on");
    }

    private void TurnOff()
    {
        isOn = false;
        Debug.Log("TV is now off");
    }
}
