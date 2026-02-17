using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ReticleController : MonoBehaviour
{
    public static ReticleController Instance { get; private set; }

    [Header("Reticle")]
    [SerializeField] private Image reticle;
    [SerializeField] private float reticleRegularSize = 0.15f;
    [SerializeField] private float reticleFocusSize = 0.2f;
    [SerializeField] private float animationDuration = 0.15f;
    [SerializeField] private AnimationCurve sizingAnimationCurve;

    [Header("Interact Hint")]
    [SerializeField] private TMP_Text interactText; // La "E"

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

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    public void MakeReticleBigger()
    {
        StartSizeAnimation(reticle.rectTransform.localScale.x, reticleFocusSize);
        ShowInteractHint(true);
    }

    public void ReturnReticleToNormalSize()
    {
        StartSizeAnimation(reticle.rectTransform.localScale.x, reticleRegularSize);
        ShowInteractHint(false);
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

    private void ShowInteractHint(bool show)
    {
        if (interactText == null) return;

        interactText.gameObject.SetActive(show);
    }
}
