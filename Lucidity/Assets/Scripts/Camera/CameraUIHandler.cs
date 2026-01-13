using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraUIHandler : MonoBehaviour
{
    [Header("Documentation Mode")]
    [SerializeField] private Image documentationAspect;
    [SerializeField] private Image cameraFlash;
    [SerializeField] private Image photo;
    [SerializeField] private GameObject remainingReelTexts;
    [SerializeField] private TextMeshProUGUI remainingReels;
    [SerializeField] private Canvas uiCanvas;


    internal void ShowCameraUI(bool showUI)
    {
        uiCanvas.enabled = showUI;
    }

    internal void ShowCameraAspect(bool showAspect)
    {
        documentationAspect.enabled = showAspect;
    }

    internal void ShowCameraFlash(bool showAspect)
    {
        cameraFlash.enabled = showAspect;
    }

    internal void ShowReelIndicator(bool showRemainingReels)
    {
        remainingReelTexts.SetActive(showRemainingReels);
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
}
