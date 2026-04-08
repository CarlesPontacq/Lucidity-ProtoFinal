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
    [SerializeField] private Image cameraRedLight;
    [SerializeField] private Image cameraFlash;
    [SerializeField] private Image stunCameraEffect;
    [SerializeField] private TextMeshProUGUI remainingReels;
    [SerializeField] private List<Image> cameraAspectBorder;
    [SerializeField] private List<TextMeshProUGUI> cameraTexts;

    [Header("Stun Photo")]
    [SerializeField] private float duration = 2f;

    internal void ShowCameraAspect(bool showAspect)
    {
        cameraAspect.enabled = showAspect;
        foreach (Image image in cameraAspectBorder)
            image.enabled = showAspect;
        
        remainingReels.enabled = showAspect;

        foreach(TextMeshProUGUI text in cameraTexts) 
            text.enabled = showAspect;

        ShowCameraRedLight(false);
    }

    internal void ShowCameraFlash(bool showAspect)
    {
        cameraFlash.enabled = showAspect;
    }

    internal void ShowCameraRedLight(bool showAspect)
    {
        cameraRedLight.enabled = showAspect;
    }

    internal void ActualizeRemainingReelsIndicator(int newRemainingReels)
    {
        remainingReels.text = newRemainingReels.ToString();
    }

    internal void ShowStunCameraEffect(bool showEffect)
    {
        stunCameraEffect.enabled = showEffect;
    }
}
