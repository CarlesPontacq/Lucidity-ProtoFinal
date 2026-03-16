using UnityEngine;
using System;
using System.Collections.Generic;

public class AnomalyManager : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string id;
        public ZoneId zoneId;         
        public Anomaly prefab;
        public Transform anchor;
    }

    [Header("Dependencies")]
    [SerializeField] private ZonesManager zonesManager;  

    [SerializeField] private List<Entry> entries = new();

    // Instancias vivas del loop
    private readonly List<Anomaly> spawnedThisLoop = new();

    // Documentadas
    private readonly HashSet<string> documentedAnomalies = new();

    // Número de entries configuradas
    public int EntryCount => entries != null ? entries.Count : 0;

    [Header("Loop Selection")]
    [SerializeField] private int anomaliesPerLoop = 0;
    [SerializeField] private int minAnomaliesPerLoop = 2;
    [SerializeField] private int maxAnomaliesPerLoop = 3;

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

    public int GetExpectedAnomalies() => ExpectedAnomaliesThisLoop;

    private void Start()
    {
        Debug.Log($"[AnomalyManager {GetInstanceID()}] Start() called. EntryCount={EntryCount}");

        if (autoStartOnBegin)
            StartNewLoop();
    }

    /// <summary>
    /// Empieza un loop: selecciona entries filtradas por zonas y spawnea.
    /// ExpectedAnomaliesThisLoop queda fijado al número REAL spawneado.
    /// </summary>
    public void StartNewLoop()
    {
        int min = Mathf.Max(0, minAnomaliesPerLoop);
        int max = Mathf.Max(min, maxAnomaliesPerLoop);
        anomaliesPerLoop = UnityEngine.Random.Range(min, max + 1);

        Debug.Log($"[AnomalyManager {GetInstanceID()}] StartNewLoop() called. EntryCount={EntryCount} anomaliesPerLoop={anomaliesPerLoop}");

        documentedAnomalies.Clear();
        ClearSpawned();

        SelectEntriesForThisLoop();
        SpawnSelectedEntries();

        Debug.Log($"[AnomalyManager {GetInstanceID()}] Loop ready. Expected={ExpectedAnomaliesThisLoop} ActiveSpawnedCount={ActiveSpawnedCount}");
    }

    private void SelectEntriesForThisLoop()
    {
        selectedEntriesThisLoop.Clear();

        if (entries == null || entries.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] entries is empty.");
            return;
        }

        List<Entry> candidates = new();

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e == null) continue;

            bool allowed =
                zonesManager == null || zonesManager.IsZoneUnlocked(e.zoneId);

            if (allowed)
                candidates.Add(e);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] No hay anomalías candidatas: ninguna zona desbloqueada o no hay entries válidas.");
            return;
        }

        int count = Mathf.Min(anomaliesPerLoop, candidates.Count);

        List<Entry> bag = new(candidates);

        for (int i = 0; i < count; i++)
        {
            int index = UnityEngine.Random.Range(0, bag.Count);
            selectedEntriesThisLoop.Add(bag[index]);
            bag.RemoveAt(index);
        }

        Debug.Log($"[AnomalyManager {GetInstanceID()}] Selected {selectedEntriesThisLoop.Count}/{candidates.Count} from unlocked zones.");
    }

    private void SpawnSelectedEntries()
    {
        ExpectedAnomaliesThisLoop = 0;

        if (selectedEntriesThisLoop.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] selectedEntriesThisLoop is empty! (Nothing to spawn)");
            return;
        }

        foreach (var e in selectedEntriesThisLoop)
        {
            if (e == null)
                continue;

            if (e.prefab == null || e.anchor == null)
            {
                Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] Entry inválida: id={e.id} prefab={(e.prefab ? e.prefab.name : "NULL")} anchor={(e.anchor ? e.anchor.name : "NULL")}");
                continue;
            }

            Debug.Log("Anomalia : " + e.prefab.name);
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

    public List<Anomaly> GetSpawnedEnemiesThisLoop() => spawnedThisLoop;
}
