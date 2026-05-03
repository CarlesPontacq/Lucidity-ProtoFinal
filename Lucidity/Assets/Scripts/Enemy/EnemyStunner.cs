using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunner : MonoBehaviour, IStunnable
{
    [Header("Dissolve")]
    [SerializeField] private float dissolveDuration = 1.2f;
    [SerializeField] private AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Particles")]
    [SerializeField] private ParticleSystem stunParticles;
    [SerializeField] private float destroyDelayAfterDissolve = 0.25f;

    private bool isCaptured = false;
    private Renderer[] renderers;
    private Collider[] colliders;
    private Material[] materialInstances;
    private Rigidbody rb;

    private EnemyLoopSpawner spawner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        CacheMaterialInstances();

        if (stunParticles != null)
            stunParticles.gameObject.SetActive(false);
    }

    private void CacheMaterialInstances()
    {
        List<Material> mats = new List<Material>();

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            if (r is ParticleSystemRenderer) continue;

            Material[] rendererMats = r.materials;

            for (int i = 0; i < rendererMats.Length; i++)
            {
                if (rendererMats[i] != null && !mats.Contains(rendererMats[i]))
                    mats.Add(rendererMats[i]);
            }
        }

        materialInstances = mats.ToArray();
    }

    public void Init(EnemyLoopSpawner ownerSpawner)
    {
        spawner = ownerSpawner;
    }

    public void OnStunned(Texture2D photo)
    {
        if (isCaptured) return;

        isCaptured = true;
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        FreezeEnemyPhysics();
        DisableCollidersOnly();

        if (stunParticles != null)
        {
            stunParticles.gameObject.SetActive(true);
            stunParticles.transform.SetParent(null, true);
            stunParticles.Play(true);
        }

        float t = 0f;

        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / dissolveDuration);
            float dissolveValue = dissolveCurve.Evaluate(normalized);

            SetMaterialsAlpha(1f - dissolveValue);

            yield return null;
        }

        SetMaterialsAlpha(0f);

        yield return new WaitForSeconds(destroyDelayAfterDissolve);

        if (spawner != null)
            spawner.OnEnemyCaptured();

        Destroy(gameObject);
    }

    private void FreezeEnemyPhysics()
    {
        if (rb == null) return;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void DisableCollidersOnly()
    {
        foreach (Collider c in colliders)
        {
            if (c != null)
                c.enabled = false;
        }
    }

    private void SetMaterialsAlpha(float alpha)
    {
        if (materialInstances == null) return;

        foreach (Material mat in materialInstances)
        {
            if (mat == null) continue;

            if (mat.HasProperty("_BaseColor"))
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = alpha;
                mat.SetColor("_BaseColor", c);
            }
            else if (mat.HasProperty("_Color"))
            {
                Color c = mat.GetColor("_Color");
                c.a = alpha;
                mat.SetColor("_Color", c);
            }
        }
    }
}