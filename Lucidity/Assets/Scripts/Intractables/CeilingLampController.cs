using System.Collections.Generic;
using UnityEngine;

public class CeilingLampController : MonoBehaviour
{
    [SerializeField] int materialIndex;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material materialLightOn;
    [SerializeField] Material materialLightOff;
    [SerializeField] List<GameObject> lights;
    [SerializeField] bool startTurnedOn;
    bool isOn;

    void Start()
    {
        isOn = true;
        if (!startTurnedOn)
            TurnOff();
    }

    void ChangeMaterial(Material material)
    {
        Material[] mats = meshRenderer.materials;
        mats[materialIndex] = material;
        meshRenderer.materials = mats;
    }

    public void TurnOn()
    {
        if (isOn) return;

        ChangeMaterial(materialLightOn);

        for (int i = 0; i < lights.Count; i++)
            lights[i].gameObject.SetActive(true);

        isOn = true;
    }

    public void TurnOff()
    {
        if (!isOn) return;

        ChangeMaterial(materialLightOff);

        for (int i = 0; i < lights.Count; i++)
            lights[i].gameObject.SetActive(false);

        isOn = false;
    }

    public bool IsLightOn()
    {
        return isOn;
    }
}
