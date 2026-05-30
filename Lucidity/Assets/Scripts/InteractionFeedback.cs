using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InteractionFeedback : MonoBehaviour
{
    public static InteractionFeedback Instance { get; private set; }

    [Header("Reticle")]
    [SerializeField] private Image reticle;
    [SerializeField] private float reticleRegularSize = 0.15f;
    [SerializeField] private float reticleFocusSize = 0.2f;
    [SerializeField] private float animationDuration = 0.15f;
    [SerializeField] private AnimationCurve sizingAnimationCurve;

    [Header("Interact Hint")]
    [SerializeField] private GameObject interactionHintCanvas;
    [SerializeField] private Animator hintAnimator;

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

    public void ShowInteractionFeedback(Vector3 hintPosition)
    {
        MakeReticleBigger();

        ShowInteractHint(hintPosition);
    }

    public void HideInteractionFeedback()
    {
        ReturnReticleToNormalSize();

        HideInteractHint();
    }

    void MakeReticleBigger()
    {
        if (reticle == null)
            return;

        StartReticleSizeAnimation(reticle.rectTransform.localScale.x, reticleFocusSize);
    }

    void ReturnReticleToNormalSize()
    {
        if (reticle == null)
            return;

        StartReticleSizeAnimation(reticle.rectTransform.localScale.x, reticleRegularSize);
    }

    void StartReticleSizeAnimation(float from, float to)
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

    public void ShowInteractHint(Vector3 position)
    {
        if (interactionHintCanvas == null) return;

        MoveInteractHint(position);
        interactionHintCanvas.gameObject.SetActive(true);
        hintAnimator.SetTrigger("Show");
    }

    public void HideInteractHint()
    {
        if (interactionHintCanvas == null) return;

        interactionHintCanvas.gameObject.SetActive(false);
    }

    public void MoveInteractHint(Vector3 position)
    {
        interactionHintCanvas.transform.position = position;
    }
}
