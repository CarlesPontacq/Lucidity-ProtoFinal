using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public class OnboardingAct2 : MonoBehaviour
{
    [SerializeField] GameObject flashObjectPrefab;
    [SerializeField] LoopManager loopManager;
    [SerializeField] float doorsOpeningDelay = 0.5f;
    [SerializeField] List<DoorInteraction> doorsToOpen;

    private bool objectSpawned = false;

    private void Start()
    {
        LoopManager.OnLoopStarted += HandleObjectSpawning;
        GameManager.Instance.OnStunUnlocked += HandleStunUnlocked;
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
        StartCoroutine(OpenDoorsWithDelay());
    }

    private IEnumerator OpenDoorsWithDelay()
    {
        yield return new WaitForSeconds(doorsOpeningDelay);

        foreach (DoorInteraction door in doorsToOpen)
        {
            door.Open(true);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStunUnlocked += HandleStunUnlocked;
    }
}
