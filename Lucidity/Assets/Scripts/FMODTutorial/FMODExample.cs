using UnityEngine;

public class FMODExample : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string eventPath = "event:/Cable01";

    FMOD.Studio.EventInstance MusicInst;

    private void Awake()
    {
        MusicInst = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        MusicInst.start();
        MusicInst.release(); //<- Destruye el sonido una vez haya acabado
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        MusicInst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
