using NUnit.Framework;
using UnityEngine;

public class LightSwitchInteraction : ObjectInteraction
{
    [SerializeField] CeilingLampController ceilingLamp;

    public override void Interact()
    {
        Debug.Log("Interaction detected");
        if (ceilingLamp.IsLightOn())
            ceilingLamp.TurnOff();
        else
            ceilingLamp.TurnOn();
    }

}
