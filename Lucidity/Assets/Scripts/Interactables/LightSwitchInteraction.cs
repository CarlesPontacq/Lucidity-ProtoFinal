using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchInteraction : ObjectInteraction
{
    [SerializeField] List<SwitchableObject> switchableObjects;
    private bool isOn;

    protected override void Start()
    {
        if (switchableObjects == null)
            Debug.LogWarning("Missing ceilingLamp in light switch: " + gameObject.name);
    }

    public override void Interact()
    {
        foreach (SwitchableObject switchableObject in switchableObjects)
        {         
            if (switchableObject.IsOn())
                switchableObject.TurnOff();
            else
                switchableObject.TurnOn();
        }

        if (isOn)
            SFXManager.Instance.PlaySpatialSound("lightSwitchOff", transform.position, 1f);
        else
            SFXManager.Instance.PlaySpatialSound("lightSwitchOn", transform.position, 1f);

        isOn = !isOn;
    }

    public void Reset()
    {
        if (!isOn) return;

        foreach (SwitchableObject switchableObject in switchableObjects)
        {
            if (switchableObject.IsOn())
                switchableObject.TurnOff();
            else
                switchableObject.TurnOn();
        }

        isOn = false;
    }
}
