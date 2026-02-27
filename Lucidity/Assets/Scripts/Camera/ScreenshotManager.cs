using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;

    private CameraUIHandler ui;

    private void Start()
    {
        ui = FindAnyObjectByType<CameraUIHandler>();
    }
    
    public void CaptureScreenshot(Camera sourceCamera)
    {
        StartCoroutine(CaptureRoutine(sourceCamera, true));
    }

    private IEnumerator CaptureRoutine(Camera sourceCamera, bool mainPhoto)
    {
        yield return new WaitForEndOfFrame();

        sourceCamera.targetTexture = renderTexture;

        RenderTexture current = RenderTexture.active;
        RenderTexture.active = renderTexture;

        sourceCamera.Render();

        Texture2D tex = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBA32,
            false,
            false
        );

        tex.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height),
            0, 0
        );

        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = Mathf.LinearToGammaSpace(pixels[i].r);
            pixels[i].g = Mathf.LinearToGammaSpace(pixels[i].g);
            pixels[i].b = Mathf.LinearToGammaSpace(pixels[i].b);
        }
        tex.SetPixels(pixels);

        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        tex.Apply();

        RenderTexture.active = current;
        sourceCamera.targetTexture = null;

        if(mainPhoto)
            ui.ActualizeTopPhoto(tex);
        else
            ui.ActualizeBottomPhoto(tex);
    }

    public void CaptureStunScreenshots(Camera playerCamera, Camera stunCamera)
    {
        StartCoroutine(CaptureRoutine(playerCamera, true));
        StartCoroutine(CaptureRoutine(stunCamera, false));
    }
}
