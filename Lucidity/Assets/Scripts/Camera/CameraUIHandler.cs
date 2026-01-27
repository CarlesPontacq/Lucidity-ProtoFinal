using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraUIHandler : MonoBehaviour
{
    [Header("Documentation Mode")]
    [SerializeField] private Image documentationAspect;
    [SerializeField] private Image uvAspect;
    [SerializeField] private Image cameraFlash;
    [SerializeField] private Image photo;
    [SerializeField] private GameObject remainingReelTexts;
    [SerializeField] private TextMeshProUGUI remainingReels;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject documentationModeUI;
    [SerializeField] private GameObject uvModeUI;


    internal void ShowCameraUI(bool showUI)
    {
        uiCanvas.enabled = showUI;
    }

    internal void ShowDocumentationCameraAspect(bool showAspect)
    {
        documentationAspect.enabled = showAspect;
    }
    
    internal void ShowUvCameraAspect(bool showAspect)
    {
        uvAspect.enabled = showAspect;
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

    internal void SetCameraModeUI(CameraMode mode)
    {
        if(mode == null) return;

        switch (mode)
        {
            case DocumentationMode:
                documentationModeUI.SetActive(true);
                uvModeUI.SetActive(false);
                break;
            case UltravioletMode:
                uvModeUI.SetActive(true);
                documentationModeUI.SetActive(false);
                break;
            default:
                break;
        }
    }
}
