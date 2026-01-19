using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    [SerializeField] private Camera photoCamera;
    [SerializeField] private Camera normalCamera;
    [SerializeField] private RenderTexture renderTexture;

    private CameraUIHandler ui;

    private void Start()
    {
        ui = FindAnyObjectByType<CameraUIHandler>();
    }
    
    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureRoutine());
    }

    private IEnumerator CaptureRoutine()
    {
        yield return new WaitForEndOfFrame();

        photoCamera.targetTexture = renderTexture;

        RenderTexture current = RenderTexture.active;
        RenderTexture.active = renderTexture;

        photoCamera.Render();

        Texture2D tex = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGB24,
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
        tex.Apply();

        tex.Apply();

        RenderTexture.active = current;
        photoCamera.targetTexture = null;

        ui.ActualizeLastPhoto(tex);
    } 
}
