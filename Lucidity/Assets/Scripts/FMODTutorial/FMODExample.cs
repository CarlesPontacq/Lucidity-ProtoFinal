using UnityEngine;

public class FMODExample : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string eventPath = "event:/Cable01";

    FMOD.Studio.EventInstance MusicInst;
    FMOD.Studio.EventInstance Run;
    FMOD.Studio.EventInstance t_hum;

    private float MaterialValue;
    public float distance = 0.3f;
    public LayerMask lm;

    private RaycastHit rh;

    Vector3 pos;
    GameObject go;

    private void Awake()
    {
        //Play Sound
        //MusicInst = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        //MusicInst.start();
        //MusicInst.release(); //<- Destruye el sonido una vez haya acabado

        t_hum = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        t_hum.start();
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(t_hum, transform, GetComponent<Rigidbody>());
        t_hum.release();

    }

    private void OnTriggerEnter(Collider other)
    {
        #region Parametros
        if (other.name == "Ellen")
        {
            t_hum.setParameterByName("Trigger", 1f, false); //<- Modificar el valor de un parametro
            //El false es para anular seek speed
            //(Una boleana para decidir si deberia cambiar automaitcamente el valor del parametro)
        }
        #endregion

        #region Attach
        /*
        pos = GetComponent<Transform>().position;        
        //Play One Shot
        //FMODUnity.RuntimeManager.PlayOneShot(eventPath, pos); //<- On another pos
        FMODUnity.RuntimeManager.PlayOneShotAttached(eventPath, gameObject); //<- Attached to game object
        */
        #endregion
    }

    #region Animation Sound
    void PlayRunEvent(string path)
    {
        MaterialCheck();
        Run = FMODUnity.RuntimeManager.CreateInstance(path); 
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Run, transform, GetComponent<Rigidbody>());
        Run.setParameterByName("Material", MaterialValue, false);
        Run.start();
        Run.release();
    }
    #endregion

    void MaterialCheck()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out rh, distance, lm))
        {
            switch (rh.collider.tag)
            {
                case "Earth":
                    MaterialValue = 1;
                    break;
                case "Ground":
                    MaterialValue = 2;
                    break;
                default:
                    MaterialValue = 1;
                    break;
            }
        }
        else
        {
            MaterialValue = 1;
        }
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
        //MusicInst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
