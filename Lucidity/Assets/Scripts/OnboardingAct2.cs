using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public class OnboardingAct2 : MonoBehaviour
{
    [SerializeField] GameObject flashObjectPrefab;
    [SerializeField] LoopManager loopManager;
    
    [Header("Doors")]
    [SerializeField] float doorsOpeningDelay = 0.5f;
    [SerializeField] List<DoorController> doorsToOpen;

    [Header("Camera UI")]
    [SerializeField] GameObject leftClickText;
    [SerializeField] CameraFunctionality cameraFunctionality;

    private bool objectSpawned = false;
    private int originalMaxReels;

    private void Start()
    {
        LoopManager.OnLoopStarted += HandleObjectSpawning;
        GameManager.GrabHandlerRef.OnStunUnlocked += HandleStunUnlocked;

        originalMaxReels = cameraFunctionality.maxReels;
        cameraFunctionality.maxReels = 0;
        cameraFunctionality.ResetReels();
    }

    private void HandleObjectSpawning(int loopIndex)
    {
        if (objectSpawned || loopIndex != 1) return;

        SpawnObjects();
        LoopManager.OnLoopStarted -= HandleObjectSpawning;
    }

    private void SpawnObjects()
    {
        Instantiate(flashObjectPrefab);
        objectSpawned = true;
    }

    private void HandleStunUnlocked()
    {
        leftClickText.SetActive(true);

        cameraFunctionality.maxReels = originalMaxReels;
        cameraFunctionality.ResetReels();

        StartCoroutine(OpenDoorsWithDelay());
    }

    private IEnumerator OpenDoorsWithDelay()
    {
        yield return new WaitForSeconds(doorsOpeningDelay);

        foreach (DoorController door in doorsToOpen)
        {
            door.Open(true);
        }
    }

    private void OnDestroy()
    {
        GameManager.GrabHandlerRef.OnStunUnlocked += HandleStunUnlocked;
    }
}
