using UnityEngine;

public class SwitchableObject : MonoBehaviour
{
    [SerializeField] bool startOn;
    protected bool isOn;

    private void Start()
    {
        isOn = true;
        if (!startOn)
            TurnOff();

        isOn = startOn;
    }
    public bool IsOn()
    {
        return isOn;
    }

    public virtual void TurnOn() { }
    public virtual void TurnOff() { }
}
