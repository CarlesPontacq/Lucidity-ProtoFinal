using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraUIHandler : MonoBehaviour
{
    [Header("Documentation Mode")]
    [SerializeField] private Image documentationAspect;
    [SerializeField] private GameObject remainingReelTexts;
    [SerializeField] private TextMeshProUGUI remainingReels;

    internal void ShowCameraAspect(bool showAspect)
    {
        documentationAspect.enabled = showAspect;
    }

    internal void ShowReelIndicator(bool showRemainingReels)
    {
        remainingReelTexts.SetActive(showRemainingReels);
    }

    internal void ActualizeRemainingReelsIndicator(int newRemainingReels)
    {
        remainingReels.text = newRemainingReels.ToString();
    }
}
