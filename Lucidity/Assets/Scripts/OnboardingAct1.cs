using UnityEngine;

public class OnboardingAct1 : MonoBehaviour
{
    [SerializeField] GameObject cameraObjectPrefab;
    [SerializeField] GameObject reportSheetObjectPrefab;
    [SerializeField] LoopManager loopManager;

    [SerializeField] Vector3 cameraObjectPosition;
    [SerializeField] Quaternion cameraObjectRotation;
    [SerializeField] Vector3 reportSheetObjectPosition;
    [SerializeField] Quaternion reportSheetObjectRotation;

    private bool objectsSpawned = false;

    private void Start()
    {
        LoopManager.OnLoopStarted += HandleObjectSpawning;
    }

    private void HandleObjectSpawning(int loopIndex)
    {
        if (objectsSpawned || loopIndex != 1) return;

        SpawnObjects();
        LoopManager.OnLoopStarted -= HandleObjectSpawning;
    }

    private void SpawnObjects()
    {
        Instantiate(cameraObjectPrefab, cameraObjectPosition, cameraObjectRotation);
        Instantiate(reportSheetObjectPrefab, reportSheetObjectPosition, reportSheetObjectRotation);
        objectsSpawned = true;
    }
}
