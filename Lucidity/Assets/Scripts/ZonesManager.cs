using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ZonesManager : MonoBehaviour
{
    [Serializable]
    struct Zone
    {
        public DoorInteraction doorToUnlock;
        public int unlockLoopNumber;
    }

    [SerializeField] LoopManager loopManager;
    [SerializeField] List<Zone> unlockableZones;

    void Start()
    {
        foreach (Zone zone in unlockableZones)
        {
            zone.doorToUnlock.Lock();
        }
    }

    public void UpdateZoneDoors(int loopIndex)
    {
        foreach (Zone zone in unlockableZones)
        {
            if (zone.unlockLoopNumber <= loopIndex)
                zone.doorToUnlock.Unlock();
            else
                zone.doorToUnlock.Lock();

        }
    }
}
