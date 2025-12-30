using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    private CameraUIHandler ui;
    public Camera mainCamera;
    private string folderName = "Screenshots";

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
        ui.ShowCameraUI(false);

        yield return new WaitForEndOfFrame();

        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/" + folderName))
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/" + folderName);

        string path = Application.persistentDataPath + "/" + folderName + "/screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Captura guardada en: " + path);

        yield return null;

        LoadLatestScreenshot();

        ui.ShowCameraUI(true);
    }

    public void LoadLatestScreenshot()
    {
        string path = Application.persistentDataPath + "/" + folderName;
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] files = dir.GetFiles("*.png");
        if (files.Length > 0)
        {
            FileInfo latestFile = files[0];
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > latestFile.LastWriteTime)
                    latestFile = file;
            }

            byte[] fileData = File.ReadAllBytes(latestFile.FullName);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            ui.ActualizeLastPhoto(tex);
        }
    }
}
