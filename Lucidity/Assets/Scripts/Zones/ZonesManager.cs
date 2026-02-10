using System;
using System.Collections.Generic;
using UnityEngine;

public class ZonesManager : MonoBehaviour
{
    [Serializable]
    public struct Zone
    {
        public ZoneId zoneId;
        public DoorInteraction doorToUnlock;
        public int unlockLoopNumber;
    }

    [SerializeField] private List<Zone> unlockableZones;

    private readonly HashSet<ZoneId> unlockedZones = new();

    private void Start()
    {
        for (int i = 0; i < unlockableZones.Count; i++)
        {
            if (unlockableZones[i].doorToUnlock != null)
                unlockableZones[i].doorToUnlock.Lock();
        }

        UpdateZoneDoors(0);
    }

    public void UpdateZoneDoors(int loopIndex)
    {
        unlockedZones.Clear();

        for (int i = 0; i < unlockableZones.Count; i++)
        {
            Zone zone = unlockableZones[i];
            bool unlocked = zone.unlockLoopNumber <= loopIndex;

            if (zone.doorToUnlock != null)
            {
                if (unlocked) zone.doorToUnlock.Unlock();
                else zone.doorToUnlock.Lock();
            }

            if (unlocked)
                unlockedZones.Add(zone.zoneId);
        }
    }

    public bool IsZoneUnlocked(ZoneId zoneId)
    {
        if (zoneId == 0) return true;

        return unlockedZones.Contains(zoneId);
    }
}
