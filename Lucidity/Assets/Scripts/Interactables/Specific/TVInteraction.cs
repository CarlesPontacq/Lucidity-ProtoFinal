using UnityEngine;

public class TVInteraction : ObjectInteraction
{
    [SerializeField] GameObject screenOff;
    [SerializeField] GameObject screenOn;

    private bool isOn = false;

    public override void ResetState()
    {
        TurnOff(false);
    }

    public override void Interact()
    {
        if (isOn)
            TurnOff(true);
        else
            TurnOn(true);
    }

    private void TurnOn(bool makeSound)
    {
        isOn = true;

        screenOff.SetActive(false);
        screenOn.SetActive(true);

        if (makeSound)
            SFXManager.Instance.PlaySpatialSound("tvOn", transform.position, 1f);
    }

    private void TurnOff(bool makeSound)
    {
        isOn = false;

        screenOff.SetActive(true);
        screenOn.SetActive(false);

        if (makeSound)
            SFXManager.Instance.PlaySpatialSound("tvOff", transform.position, 1f);
    }
}
