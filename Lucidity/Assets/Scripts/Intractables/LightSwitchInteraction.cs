using NUnit.Framework;
using UnityEngine;

public class LightSwitchInteraction : ObjectInteraction
{
    [SerializeField] CeilingLampController ceilingLamp;

    public override void Interact()
    {
        Debug.Log("Interaction detected");
        if (ceilingLamp.IsLightOn())
        {
            ceilingLamp.TurnOff();
            SFXManager.Instance.PlaySpatialSound("lightSwitchOff", transform.position, 1f);
        }
        else
        {
            ceilingLamp.TurnOn();
            SFXManager.Instance.PlaySpatialSound("lightSwitchOn", transform.position, 1f);
        }
    }

}
