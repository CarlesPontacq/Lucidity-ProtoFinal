using UnityEngine;

public class BasicSwitchableObject : SwitchableObject
{
    public override void TurnOn()
    {
        isOn = true;
        gameObject.SetActive(true);
    }

    public override void TurnOff()
    {
        isOn = false;
        gameObject.SetActive(false);
    }
}
