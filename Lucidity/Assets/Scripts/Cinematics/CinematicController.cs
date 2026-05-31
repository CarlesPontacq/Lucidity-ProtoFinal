using System;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CinematicFade fade;

    private bool ending;

    private void OnEnable()
    {
        director.stopped += OnCinematicFinished;
    }

    private void OnDisable()
    {
        director.stopped -= OnCinematicFinished;
    }

    private void OnCinematicFinished(PlayableDirector d)
    {
        EndCinematic();
    }

    public void Skip()
    {
        EndCinematic();
    }

    private void EndCinematic()
    {
        if (ending) return;
        ending = true;

        director.stopped -= OnCinematicFinished;
        director.Stop();

        fade.FadeToBlack(() =>
        {
            SceneController.Instance.LoadNextScene();
        });
    }
}