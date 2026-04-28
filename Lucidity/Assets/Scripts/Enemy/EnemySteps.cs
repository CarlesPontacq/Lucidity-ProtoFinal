using UnityEngine;
using UnityEngine.Audio;

public class EnemySteps : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string enemyStepsSFX;
    [SerializeField] private float enemyStepsVolume;
    [SerializeField] private Transform sfxLocation;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayEnemySteps()
    {
        SFXManager.Instance.PlaySpatialSound(enemyStepsSFX, sfxLocation.position, enemyStepsVolume);
    }
}
