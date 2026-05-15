using UnityEngine;

public class StunModeUnlocker : MonoBehaviour
{
    [SerializeField] GameObject stunModeObjectUnlockerPrefab;
    private bool objectSpawned = false;
    [SerializeField] private int numberLoopToSapwnObject = 4; 

    void Start()
    {
        LoopManager.OnLoopStarted += HandleObjectSpawning;
    }

    private void HandleObjectSpawning(int loopIndex)
    {
        if (objectSpawned || loopIndex != numberLoopToSapwnObject) return;

        SpawnObjects();
        LoopManager.OnLoopStarted -= HandleObjectSpawning;
    }

    private void SpawnObjects()
    {
        Instantiate(stunModeObjectUnlockerPrefab);
        objectSpawned = true;
    }
}
