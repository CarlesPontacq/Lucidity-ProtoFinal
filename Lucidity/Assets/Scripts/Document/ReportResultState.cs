using UnityEngine;

public class ReportResultState : MonoBehaviour
{
    public bool HasSubmittedReport { get; private set; }
    public bool WasCorrect { get; private set; }

    public void ResetForNewLoop()
    {
        HasSubmittedReport = false;
        WasCorrect = false;
    }

    public void Submit(bool wasCorrect)
    {
        HasSubmittedReport = true;
        WasCorrect = wasCorrect;
    }
}
