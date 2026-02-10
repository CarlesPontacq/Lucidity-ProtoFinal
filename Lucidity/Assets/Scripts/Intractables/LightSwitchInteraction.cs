using NUnit.Framework;
using UnityEngine;

public class LightSwitchInteraction : ObjectInteraction
{
    [SerializeField] CeilingLampController ceilingLamp;

    protected override void Start()
    {
        if (ceilingLamp == null)
            Debug.LogWarning("Missing ceilingLamp in light switch: " + gameObject.name);
    }

    public override void Interact()
    {
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
