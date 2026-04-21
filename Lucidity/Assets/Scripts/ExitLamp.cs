using UnityEngine;

public class ExitLamp : MonoBehaviour
{
    [SerializeField] MeshRenderer lampMesh;
    [SerializeField] Material onMaterial;
    [SerializeField] Material offMaterial;
    [SerializeField] int materialIndex;

    [SerializeField] GameObject pointLight;

    private Material[] materials;

    void Awake()
    {
        materials = lampMesh.materials;
    }

    public void TurnOn()
    {
        materials[materialIndex] = onMaterial;
        lampMesh.materials = materials;

        pointLight.SetActive(true);
    }

    public void TurnOff()
    {
        materials[materialIndex] = offMaterial;
        lampMesh.materials = materials;

        pointLight.SetActive(false);
    }
}
