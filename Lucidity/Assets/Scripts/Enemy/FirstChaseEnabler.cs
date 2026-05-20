using System;
using UnityEngine;

public class FirstChaseEnabler : MonoBehaviour
{
    [Header("First Enemy Chase")]
    [SerializeField] private GameObject firstChaseEnemy;
    [SerializeField] private bool firstChase;
    [SerializeField] private BoxCollider firstChaseTrigger;

    [SerializeField] private DoorController door;
    [SerializeField] private Transform doorAudioPosition;


    private string playerTag = "Player";
    private string enemySpawnSFX = "EnemySpawn";
    private string doorSFX = "openDoor";
    private int SFXVolume = 1;

    public static event Action OnFirstChaseStarted;

    private void Awake()
    {
        door.Lock();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && firstChase)
        {
            firstChase = false;
            firstChaseEnemy.SetActive(true);
            SFXManager.Instance.PlaySpatialSound(enemySpawnSFX, firstChaseEnemy.transform.position, SFXVolume);

            SFXManager.Instance.PlaySpatialSound(doorSFX, doorAudioPosition.position, SFXVolume);

            firstChaseTrigger.enabled = true;
            door.Unlock();

            OnFirstChaseStarted?.Invoke();
        }
    }
}
