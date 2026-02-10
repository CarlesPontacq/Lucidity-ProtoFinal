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
    [SerializeField] private Image photo;
    [SerializeField] private GameObject polaroid;
    [SerializeField] private TextMeshProUGUI remainingReels;

    [Header("Mode Indicator")]
    [SerializeField] private List<GameObject> indicatorPositions;
    [SerializeField] private RectTransform indicator;

    [SerializeField] private float indicatorMoveSpeed = 10f;
    private Coroutine indicatorCoroutine;

    internal void ShowCameraAspect(bool showAspect)
    {
        cameraAspect.enabled = showAspect;
        indicator.GetComponent<Image>().enabled = showAspect;
        Debug.Log(indicator.name + " - " + indicator.GetComponent<Image>().enabled);
        remainingReels.enabled = showAspect;
        polaroid.SetActive(!showAspect);
    }

    internal void ShowCameraFlash(bool showAspect)
    {
        cameraFlash.enabled = showAspect;
    }

    internal void ActualizeRemainingReelsIndicator(int newRemainingReels)
    {
        remainingReels.text = newRemainingReels.ToString();
    }

    internal void ActualizeLastPhoto(Texture2D tex)
    {
        if (tex == null) return;

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );

        photo.sprite = sprite;
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
}
