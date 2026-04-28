using Unity.VisualScripting;
using UnityEngine;

public class BasicSwitchableObject : SwitchableObject
{
    [SerializeField] GameObject objectToToggle;

    public override void TurnOn()
    {
        isOn = true;
        objectToToggle.SetActive(true);
    }

    public override void TurnOff()
    {
        isOn = false;
        objectToToggle.SetActive(false);
    }
}
