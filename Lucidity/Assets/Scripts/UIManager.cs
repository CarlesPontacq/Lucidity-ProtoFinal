using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Image reticle;
    [SerializeField] float reticleRegularSize = 0.15f;
    [SerializeField] float reticleFocusSize = 0.2f;
    [SerializeField] float animationDuration = 0.15f;
    [SerializeField] AnimationCurve sizingAnimationCurve;

    private Coroutine reticleSizeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void MakeReticleBigger()
    {
        StartSizeAnimation(reticle.rectTransform.localScale.x, reticleFocusSize);
    }

    public void ReturnReticleToNormalSize()
    {
        StartSizeAnimation(reticle.rectTransform.localScale.x, reticleRegularSize);
    }

    void StartSizeAnimation(float from, float to)
    {
        if (reticleSizeCoroutine != null)
            StopCoroutine(reticleSizeCoroutine);

        reticleSizeCoroutine = StartCoroutine(AnimateSize(from, to));
    }

    IEnumerator AnimateSize(float from, float to)
    {
        float time = 0f;

        while (time < animationDuration)
        {
            float t = time / animationDuration;
            float curveValue = sizingAnimationCurve.Evaluate(t);
            float size = Mathf.Lerp(from, to, curveValue);

            reticle.rectTransform.localScale = Vector3.one * size;

            time += Time.deltaTime;
            yield return null;
        }

        reticle.rectTransform.localScale = Vector3.one * to;
    }
}
