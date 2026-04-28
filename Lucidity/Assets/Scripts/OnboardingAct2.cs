using UnityEngine;

public class OnboardingAct2 : MonoBehaviour
{
    [SerializeField] GameObject flashObjectPrefab;
    [SerializeField] LoopManager loopManager;

    private bool objectSpawned = false;

    private void Start()
    {
        LoopManager.OnLoopStarted += HandleObjectSpawning;
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
}
