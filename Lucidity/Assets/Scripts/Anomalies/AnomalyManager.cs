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

    [SerializeField] private List<Entry> entries;

    private readonly List<int> bag = new();
    private Anomaly current;

    private void Awake()
    {
        RefillAndShuffleBag();
    }
    private void Start()
    {
        Debug.Log("AnomalyManager Start");
        SpawnNextRandomNoRepeat();
    }

    public void SpawnNextRandomNoRepeat()
    {
        Debug.Log("SpawnNextRandomNoRepeat CALLED");
        if (entries.Count == 0) return;

        if (bag.Count == 0)
            RefillAndShuffleBag();

        int lastIndex = bag.Count - 1;
        int entryIndex = bag[lastIndex];
        bag.RemoveAt(lastIndex);

        if (current != null)
            Destroy(current.gameObject);

        var e = entries[entryIndex];
        current = Instantiate(e.prefab, e.anchor.position, e.anchor.rotation, e.anchor);
        current.Activate();
    }

    public void Clear()
    {
        if (current != null) Destroy(current.gameObject);
        current = null;
    }

    private void RefillAndShuffleBag()
    {
        bag.Clear();
        for (int i = 0; i < entries.Count; i++)
            bag.Add(i);

        for (int i = bag.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (bag[i], bag[j]) = (bag[j], bag[i]);
        }
    }
}
