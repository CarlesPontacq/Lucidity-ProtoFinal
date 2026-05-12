using UnityEngine;
using UnityEditor;

public class DoorMigrationTool : MonoBehaviour
{
    [MenuItem("Tools/Migrate Doors")]
    public static void MigrateDoors()
    {
        DoorInteraction[] oldDoors = GameObject.FindObjectsOfType<DoorInteraction>(true);

        int count = 0;

        foreach (var oldDoor in oldDoors)
        {
            var newDoor = oldDoor.GetComponent<DoorController>();
            if (newDoor == null)
                newDoor = oldDoor.gameObject.AddComponent<DoorController>();

            newDoor.CopyFromOld(oldDoor);

            EditorUtility.SetDirty(newDoor);
            count++;
        }

        Debug.Log($"Migradas {count} puertas");
    }
}