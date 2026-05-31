using UnityEngine;

public class CinematicSkipping : MonoBehaviour
{
    [SerializeField] private CinematicInputObserver input;
    [SerializeField] private CinematicSkipPanelUI ui;
    [SerializeField] private CinematicController cinematicController;

    [SerializeField] private float completeThreshold = 1f;

    private bool skipTriggered;
    bool lastHolding;

    private void Update()
    {
        float progress = input.GetHoldProgress();

        bool holding = progress > 0f;

        if (holding != lastHolding)
        {
            if (holding)
                ui.Show();
            else
                ui.Hide();

            lastHolding = holding;
        }

        ui.SetProgress(progress);

        if (!skipTriggered && progress >= 1f)
        {
            skipTriggered = true;
            cinematicController.Skip();
        }
    }
}