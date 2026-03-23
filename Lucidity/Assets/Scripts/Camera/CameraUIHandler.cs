using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraUIHandler : MonoBehaviour
{
    [Header("Camera UI")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Image cameraAspect;
    [SerializeField] private Image cameraFlash;
    [SerializeField] private Image photoTop;
    [SerializeField] private Image photoBottom;
    [SerializeField] private Image stunCameraEffect;
    [SerializeField] private Image cameraUIIndicator;
    [SerializeField] private GameObject polaroid;
    [SerializeField] private TextMeshProUGUI remainingReels;
    [SerializeField] private List<Image> cameraAspectBorder;

    private bool photoHasBeenTaken = false;

    [Header("Mode Indicator")]
    [SerializeField] private List<GameObject> indicatorPositions;
    [SerializeField] private RectTransform indicator;

    [SerializeField] private float indicatorMoveSpeed = 10f;
    private Coroutine indicatorCoroutine;

    [Header("Stun Photo")]
    [SerializeField] private float duration = 2f;

    internal void ShowCameraAspect(bool showAspect)
    {
        cameraAspect.enabled = showAspect;
        foreach (Image image in cameraAspectBorder)
            image.enabled = showAspect;
        
        indicator.GetComponent<Image>().enabled = showAspect;
        remainingReels.enabled = showAspect;

        if(photoHasBeenTaken)
            ShowPolaroid(!showAspect);
        
        cameraUIIndicator.enabled = showAspect;
    }

    internal void ShowPolaroid(bool showPolaroid)
    {
        polaroid.SetActive(showPolaroid);
    }

    internal void SetPhotoTaken()
    {
        photoHasBeenTaken = true;
    }

    internal void ShowCameraFlash(bool showAspect)
    {
        cameraFlash.enabled = showAspect;
    }

    internal void ActualizeRemainingReelsIndicator(int newRemainingReels)
    {
        remainingReels.text = newRemainingReels.ToString();
    }

    internal void ActualizeTopPhoto(Texture2D tex)
    {
        if (tex == null) return;

        photoTop.color = Color.white;

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );

        photoTop.sprite = sprite;
    }

    internal void ActualizeBottomPhoto(Texture2D tex)
    {
        if (tex == null) return;

        photoBottom.color = Color.white;

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );

        photoBottom.sprite = sprite;

        StartCoroutine(FadeOutTopPhoto());
    }

    internal void SetIndicatorPosition(int modeIndex)
    {
        if (indicator == null || indicatorPositions == null) return;
        if (modeIndex < 0 || modeIndex > indicatorPositions.Count)
            return;

        RectTransform indicatorRT = indicator.GetComponent<RectTransform>();
        RectTransform targetRT = indicatorPositions[modeIndex].GetComponent<RectTransform>();

        if (indicatorCoroutine != null)
            StopCoroutine(indicatorCoroutine);

        indicatorCoroutine = StartCoroutine(MoveIndicator(indicatorRT, targetRT.anchoredPosition));
    }

    private IEnumerator MoveIndicator(RectTransform indicatorRT, Vector2 targetPos)
    {
        while (Vector2.Distance(indicatorRT.anchoredPosition, targetPos) > 0.5f)
        {
            indicatorRT.anchoredPosition = Vector2.Lerp(
                indicatorRT.anchoredPosition,
                targetPos,
                Time.deltaTime * indicatorMoveSpeed
            );
            yield return null;
        }

        indicatorRT.anchoredPosition = targetPos;
    }

    private IEnumerator FadeOutTopPhoto()
    {
        float t = 0f;

        while(t < duration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / duration);

            Color c = photoTop.color;
            c.a = alpha;
            photoTop.color = c;

            yield return null;
        }
    }

    internal void ShowStunCameraEffect(bool showEffect)
    {
        stunCameraEffect.enabled = showEffect;
    }
}
