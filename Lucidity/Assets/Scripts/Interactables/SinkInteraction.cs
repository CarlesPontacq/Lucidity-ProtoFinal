using UnityEngine;

public class SinkInteraction : ObjectInteraction
{
    private bool isOpen = false;

    public override void Interact()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    private void Open()
    {
        isOpen = true;
        Debug.Log("Sink is now open");
    }

    private void Close()
    {
        isOpen = false;
        Debug.Log("Sink is now closed");
    }
}
