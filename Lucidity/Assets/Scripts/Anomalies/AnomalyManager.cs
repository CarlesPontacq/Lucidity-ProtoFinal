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

    // Documentadas (si lo sigues usando)
    private readonly HashSet<string> documentedAnomalies = new();

    // Número de entries configuradas
    public int EntryCount => entries != null ? entries.Count : 0;

    // ✅ Instancias vivas REALES en escena (limpia nulos)
    public int ActiveSpawnedCount
    {
        get
        {
            for (int i = spawnedThisLoop.Count - 1; i >= 0; i--)
                if (spawnedThisLoop[i] == null) spawnedThisLoop.RemoveAt(i);

            return spawnedThisLoop.Count;
        }
    }

    private void Start()
    {
        Debug.Log($"[AnomalyManager {GetInstanceID()}] Start() called. EntryCount={EntryCount}");
        StartNewLoop();
    }

    /// <summary>
    /// Empieza un loop: spawnea 1 instancia por Entry (todas a la vez).
    /// </summary>
    public void StartNewLoop()
    {
        Debug.Log($"[AnomalyManager {GetInstanceID()}] StartNewLoop() called. EntryCount={EntryCount}");

        documentedAnomalies.Clear();
        ClearSpawned();
        SpawnAllEntries();

        Debug.Log($"[AnomalyManager {GetInstanceID()}] After SpawnAllEntries. ActiveSpawnedCount={ActiveSpawnedCount} listCount={spawnedThisLoop.Count}");
    }

    private void SpawnAllEntries()
    {
        if (entries == null || entries.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] entries is empty!");
            return;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e == null)
            {
                Debug.LogWarning($"[AnomalyManager] Entry {i} is NULL");
                continue;
            }

            Debug.Log($"[AnomalyManager] Entry {i}: prefab={(e.prefab ? e.prefab.name : "NULL")} anchor={(e.anchor ? e.anchor.name : "NULL")}");

            if (e.prefab == null || e.anchor == null) continue;

            var instance = Instantiate(e.prefab, e.anchor.position, e.anchor.rotation, e.anchor);
            Debug.Log($"Instantiated: {instance.name} active={instance.gameObject.activeInHierarchy}");

            instance.Activate();

            spawnedThisLoop.Add(instance);
            Debug.Log($"spawnedThisLoop.Count = {spawnedThisLoop.Count}");
        }
    }

    public void ClearSpawned()
    {
        for (int i = 0; i < spawnedThisLoop.Count; i++)
        {
            if (spawnedThisLoop[i] != null)
                Destroy(spawnedThisLoop[i].gameObject);
        }
        spawnedThisLoop.Clear();
    }

    public bool IsDocumented(string anomalyId) => documentedAnomalies.Contains(anomalyId);

    public void RegisterDocumentation(Anomaly anomaly)
    {
        if (anomaly == null) return;
        documentedAnomalies.Add(anomaly.Id);
    }
}
