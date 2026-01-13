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

    private List<Entry> selectedEntriesThisLoop = new();

    // Instancias vivas REALES en escena (limpia nulos)
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
        return anomaliesPerLoop;
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

        SelectEntriesForThisLoop();
        //SpawnAllEntries();
        SpawnSelectedEntries();

        Debug.Log($"[AnomalyManager {GetInstanceID()}] After SpawnAllEntries. ActiveSpawnedCount={ActiveSpawnedCount} listCount={spawnedThisLoop.Count}");
    }

    void SpawnSelectedEntries()
    {
        if (selectedEntriesThisLoop == null || selectedEntriesThisLoop.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] selectedEntriesThisLoop is empty!");
            return;
        }

        foreach (var e in selectedEntriesThisLoop)
        {
            if(e == null || e.prefab == null || e.anchor == null) continue;

            var instance = Instantiate(e.prefab, e.anchor.position, e.anchor.rotation, e.anchor);
            Debug.Log(instance.name);
            instance.MarkSpawned();
            instance.Activate();
            spawnedThisLoop.Add(instance);
            Debug.Log($"Instantiated: {instance.name} active={instance.gameObject.activeInHierarchy}");

        }
    }

    /*
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
    */

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

        if(entries == null || entries.Count == 0 ) return;

        int count = Mathf.Min(anomaliesPerLoop, entries.Count);

        List<Entry> bag = new(entries);


        for(int i = 0; i < count; i++)
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
