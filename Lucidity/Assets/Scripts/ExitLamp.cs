using UnityEngine;

public class ExitLamp : MonoBehaviour
{
    [SerializeField] MeshRenderer lampMesh;
    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;
    [SerializeField] float materialIndex;

    [SerializeField] GameObject pointLight;

    void Start()
    {
        TurnOn();
    }

    public void TurnOn()
    {
        lampMesh.material = onMaterial;
        pointLight.SetActive(true);
    }

    public void TurnOff()
    {
        lampMesh.material = offMaterial;
        pointLight.SetActive(false);
    }
}
