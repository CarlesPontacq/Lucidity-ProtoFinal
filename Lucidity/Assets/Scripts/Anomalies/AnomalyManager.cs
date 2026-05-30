using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class AnomalyManager : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string id;
        public Anomaly prefab;
        public Transform anchor;
        public AreasID areaID;
    }

    [Serializable]
    public class ScriptedLoopAnomalies
    {
        public int loop;
        public bool loopHasBeenSpawned;
        [SerializeField] public List<Entry> loopEntries = new();
    }

    [SerializeField] private List<Entry> entries = new();
    
    private readonly List<Anomaly> spawnedThisLoop = new();
    
    private readonly HashSet<string> documentedAnomalies = new();

    public int EntryCount => entries != null ? entries.Count : 0;

    [Header("Loop Selection")]
    private int anomaliesPerLoop = 0;
    [SerializeField] private int minAnomaliesPerLoop = 2;
    [SerializeField] private int maxAnomaliesPerLoop = 3;
    private int numberOfAttempts = 3;

    [Header("Scripted Loops")]
    [SerializeField] private List<ScriptedLoopAnomalies> scriptedLoopsAnomalies = new();

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
        documentedAnomalies.Clear();
        ClearSpawned();

        int currentLoop = GameManager.LoopManagerRef.GetCurrentLoopIndex();
        currentLoop--;
        if(currentLoop < scriptedLoopsAnomalies.Count)
        {
            if (!scriptedLoopsAnomalies[currentLoop].loopHasBeenSpawned)
            {
                PreparedScriptedLoop(currentLoop);
                return;
            }
        }

       PrepareRandomLoop();
    }

    private void PreparedScriptedLoop(int currentLoop)
    {
        scriptedLoopsAnomalies[currentLoop].loopHasBeenSpawned = true;

        selectedEntriesThisLoop.Clear();

        foreach(Entry entry in scriptedLoopsAnomalies[currentLoop].loopEntries)
        {
            selectedEntriesThisLoop.Add(entry);
        }

        SpawnSelectedEntries();
    }

    private void PrepareRandomLoop()
    {
        int min = Mathf.Max(0, minAnomaliesPerLoop);
        int max = Mathf.Max(min, maxAnomaliesPerLoop);
        anomaliesPerLoop = UnityEngine.Random.Range(min, max + 1);

        Debug.Log($"[AnomalyManager {GetInstanceID()}] StartNewLoop() called. EntryCount={EntryCount} anomaliesPerLoop={anomaliesPerLoop}");

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

        int count = Mathf.Min(anomaliesPerLoop, entries.Count);

        List<Entry> bag = new List<Entry>(entries);
        HashSet<AreasID> usedAreas = new HashSet<AreasID>();
        int maxAttempts = count * numberOfAttempts;
        int attempts = 0;

        for (int i = 0; i < count && attempts < maxAttempts; i++)
        {
            if (bag.Count == 0)
            {
                Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] No more entries available after {i} selections.");
                break;
            }

            Entry selectedEntry = null;
            List<Entry> tempBag = new List<Entry>(bag);

            while (tempBag.Count > 0 && selectedEntry == null)
            {
                int index = UnityEngine.Random.Range(0, tempBag.Count);
                Entry candidate = tempBag[index];

                if (candidate.areaID == AreasID.None || !usedAreas.Contains(candidate.areaID))
                {
                    selectedEntry = candidate;
                    break;
                }

                tempBag.RemoveAt(index);
                attempts++;

                if (attempts >= maxAttempts)
                    break;
            }

            if (selectedEntry == null)
            {
                Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] Could not find valid entry after {attempts} attempts.");
                break;
            }

            selectedEntriesThisLoop.Add(selectedEntry);

            if (selectedEntry.areaID != AreasID.None)
            {
                usedAreas.Add(selectedEntry.areaID);
            }

            bag.Remove(selectedEntry);
        }

        Debug.Log($"[AnomalyManager {GetInstanceID()}] Selected {selectedEntriesThisLoop.Count}/{count} anomalies. Used areas: {string.Join(", ", usedAreas)}");
    }

    private void SpawnSelectedEntries()
    {
        ExpectedAnomaliesThisLoop = 0;

        if (selectedEntriesThisLoop.Count == 0)
        {
            Debug.LogWarning($"[AnomalyManager {GetInstanceID()}] selectedEntriesThisLoop is empty and no enemy to spawn!");
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
            e.prefab.Activate();

            spawnedThisLoop.Add(e.prefab);
            ExpectedAnomaliesThisLoop++;
        }
    }

    //private void DecideIfEnemySpawns()
    //{
    //    if (enemySpawner == null)
    //    {
    //        enemyHasToSpawn = false;
    //        return;
    //    }

    //    enemyHasToSpawn = UnityEngine.Random.value <= enemySpawnProbability;
    //    if (enemyHasToSpawn)
    //    {
    //        Debug.Log($"[AnomalyManager] Enemy will spawn this loop with probability {enemySpawnProbability:P}");
    //    }
    //    else
    //    {
    //        Debug.Log($"[AnomalyManager] No enemy this loop");
    //    }
    //}

    public void ClearSpawned()
    {
        for (int i = 0; i < spawnedThisLoop.Count; i++)
        {
            if (spawnedThisLoop[i] != null)
            {
                spawnedThisLoop[i].Deactivate();
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
