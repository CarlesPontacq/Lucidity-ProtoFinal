using System.Collections;
using UnityEngine;

public class EnemyStunner : MonoBehaviour, IStunnable
{
    private bool isCaptured = false;
    private Renderer[] renderers;
    private Collider[] colliders;

    private EnemySpawner spawner;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Init(EnemySpawner ownerSpawner)
    {
        spawner = ownerSpawner;
    }

    public void OnStunned(Texture2D photo)
    {
        if (isCaptured) return;

        isCaptured = true;
        StartCoroutine(DisappearRoutine());
    }

    private IEnumerator DisappearRoutine()
    {
        DisableEnemy();

        yield return new WaitForSeconds(0.5f);

        if (spawner != null)
            spawner.OnEnemyCaptured();

        Destroy(gameObject);
    }

    private void DisableEnemy()
    {
        foreach (var r in renderers)
            r.enabled = false;

        foreach (var c in colliders)
            c.enabled = false;
    }
}