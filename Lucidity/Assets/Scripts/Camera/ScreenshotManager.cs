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
            false
        );

        tex.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height),
            0, 0
        );
        tex.Apply();

        RenderTexture.active = current;
        photoCamera.targetTexture = null;

        ui.ActualizeLastPhoto(tex);
    } 
}
