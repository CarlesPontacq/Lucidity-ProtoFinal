using UnityEngine;
using System;
using System.Collections.Generic;

public class AnomalyManager : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string id;
        public Anomaly prefab;
        public Transform anchor;
    }

    [SerializeField] private List<Entry> entries = new();

    // Instancias vivas del loop
    private readonly List<Anomaly> spawnedThisLoop = new();

    // Documentadas
    private readonly HashSet<string> documentedAnomalies = new();

    // Número de entries configuradas
    public int EntryCount => entries != null ? entries.Count : 0;

    [Header("Loop Selection")]
    [SerializeField] private int anomaliesPerLoop = 3;

    [Header("Auto Start")]
    [Tooltip("Si LoopManager ya llama StartNewLoop(), desactiva esto para evitar doble arranque.")]
    [SerializeField] private bool autoStartOnBegin = false;

    private readonly List<Entry> selectedEntriesThisLoop = new();

    public int ExpectedAnomaliesThisLoop { get; private set; } = 0;

    // Instancias vivas REALES en escena
    public int ActiveSpawnedCount
    {
        get
        {
            for (int i = spawnedThisLoop.Count - 1; i >= 0; i--)
                if (spawnedThisLoop[i] == null) spawnedThisLoop.RemoveAt(i);

            return spawnedThisLoop.Count;
        }
    }

    public int GetExpectedAnomalies()
    {
        return ExpectedAnomaliesThisLoop;
    }

    private void Start()
    {
        Debug.Log($"[AnomalyManager {GetInstanceID()}] Start() called. EntryCount={EntryCount}");

        if (autoStartOnBegin)
            StartNewLoop();
    }

    /// <summary>
    /// Empieza un loop: selecciona entries y spawnea.
    /// ExpectedAnomaliesThisLoop queda fijado al número REAL spawneado.
    /// </summary>
    public void StartNewLoop()
    {
        Debug.Log($"[AnomalyManager {GetInstanceID()}] StartNewLoop() called. EntryCount={EntryCount}");

        documentedAnomalies.Clear();
        ClearSpawned();

        SelectEntriesForThisLoop();
        SpawnSelectedEntries();

        Debug.Log($"[AnomalyManager {GetInstanceID()}] Loop ready. Expected={ExpectedAnomaliesThisLoop} ActiveSpawnedCount={ActiveSpawnedCount}");
    }

    private void SpawnSelectedEntries()
    {
        ExpectedAnomaliesThisLoop = 0;

        if (selectedEntriesThisLoop == null || selectedEntriesThisLoop.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] selectedEntriesThisLoop is empty!");
            return;
        }

        foreach (var e in selectedEntriesThisLoop)
        {
            if (e == null || e.prefab == null || e.anchor == null)
                continue;

            var instance = Instantiate(e.prefab, e.anchor.position, e.anchor.rotation, e.anchor);
            instance.MarkSpawned();
            instance.Activate();

            spawnedThisLoop.Add(instance);
            ExpectedAnomaliesThisLoop++; 
        }
    }

    public void ClearSpawned()
    {
        for (int i = 0; i < spawnedThisLoop.Count; i++)
        {
            if (spawnedThisLoop[i] != null)
            {
                spawnedThisLoop[i].Deactivate();
                spawnedThisLoop[i].MarkUnspawned();
                Destroy(spawnedThisLoop[i].gameObject);
            }
        }
        spawnedThisLoop.Clear();
        ExpectedAnomaliesThisLoop = 0; 
    }

    public bool IsDocumented(string anomalyId) => documentedAnomalies.Contains(anomalyId);

    public void RegisterDocumentation(Anomaly anomaly)
    {
        if (anomaly == null) return;
        documentedAnomalies.Add(anomaly.Id);
    }

    private void SelectEntriesForThisLoop()
    {
        selectedEntriesThisLoop.Clear();

        if (entries == null || entries.Count == 0) return;

        int count = Mathf.Min(anomaliesPerLoop, entries.Count);

        List<Entry> bag = new(entries);

        for (int i = 0; i < count; i++)
        {
            int index = UnityEngine.Random.Range(0, bag.Count);
            selectedEntriesThisLoop.Add(bag[index]);
            bag.RemoveAt(index);
        }
    }

    public List<Anomaly> GetSpawnedEnemiesThisLoop()
    {
        return spawnedThisLoop;
    }
}
