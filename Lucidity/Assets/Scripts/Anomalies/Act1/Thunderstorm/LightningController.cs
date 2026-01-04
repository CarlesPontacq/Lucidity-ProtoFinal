using UnityEngine;

public class LightningController : MonoBehaviour
{
    [SerializeField] private GameObject[] lightnings;

    [Range(1f, 2f)]
    [SerializeField] private float firstLightnintTimer = 1.7f;

    public float lightningMinDuration = .125f;
    public float lightningMaxDuration = .75f;

    public float nextLightningMinWaiting = 3.5f;
    public float nextLightningMaxWaiting = 7.7f;

    void Start()
    {
        for(int i = 0; i < lightnings.Length; i++)
        {
            lightnings[i].SetActive(false);
        }

        Invoke("CallLightning", firstLightnintTimer);
    }

    void CallLightning()
    {

        int randomLightning = Random.Range(0, lightnings.Length);
        float randomDuration = Random.Range(lightningMinDuration, lightningMaxDuration);

        for(int i = 0; i < lightnings.Length; i++)
        {
            if (i == randomLightning)
            {
                Debug.Log("Strike" + i);
                lightnings[i].SetActive(true);
                Invoke("EndLightning", randomDuration);
            }
        }

        float rand = Random.Range(nextLightningMinWaiting, nextLightningMaxWaiting);
        Invoke("CallLightning", rand);
    }

    void EndLightning()
    {
        for (int i = 0; i < lightnings.Length; i++)
        {
            lightnings[i].SetActive(false);
        }
    }

    void CallThunder()
    {

    }

    void EndThunder()
    {

    }
}
