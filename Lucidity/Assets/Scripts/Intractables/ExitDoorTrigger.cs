using UnityEngine;

public class ExitDoorTrigger : MonoBehaviour
{
    [SerializeField] private ReportResultState reportState;
    [SerializeField] private LoopManager loopManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (reportState == null || loopManager == null)
        {
            Debug.LogWarning("ExitDoorTrigger: faltan referencias.");
            return;
        }

        if (!reportState.HasSubmittedReport)
        {
            Debug.Log("Aún no has firmado el documento.");
            return;
        }

        if (reportState.WasCorrect)
        {
            Debug.Log("Documento correcto: pasando al siguiente loop.");
            loopManager.StartNextLoop();
        }
        else
        {
            Debug.Log("Documento incorrecto: sigues en el mismo loop.");
        }
    }
}
