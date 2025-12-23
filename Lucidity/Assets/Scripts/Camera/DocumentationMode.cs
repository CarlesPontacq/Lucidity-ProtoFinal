using UnityEngine;

public class DocumentationMode : CameraMode
{
    public int maxReels = 5;
    public int currentReels;

    void Start()
    {
        base.Start();

        currentReels = maxReels;
        isUnlocked = true;
    }


    void Update()
    {
        base.Update();
    }
}
