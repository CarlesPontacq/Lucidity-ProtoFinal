using System;
using System.Collections;
using UnityEngine;

public class DocumentationMode : CameraMode
{
    public int maxReels = 5;
    public int currentReels;

    [SerializeField] private float flasDuration = 0.3f;

    private Coroutine flashCoroutine;

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

    protected override void OnActivated()
    {
        base.OnActivated();

        if(flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        if(flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }

    private IEnumerator FlashCoroutine()
    {
        yield return new WaitForSeconds(flasDuration);

        FindObjectOfType<CameraManager>().DeactivateMode();
    }
}
