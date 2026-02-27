using System.Collections.Generic;
using UnityEngine;

public class CeilingLampController : SwitchableObject
{
    [SerializeField] int materialIndex;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material materialLightOn;
    [SerializeField] Material materialLightOff;
    [SerializeField] List<GameObject> lights;

    void ChangeMaterial(Material material)
    {
        Material[] mats = meshRenderer.materials;
        mats[materialIndex] = material;
        meshRenderer.materials = mats;
    }

    public override void TurnOn()
    {
        if (isOn) return;

        ChangeMaterial(materialLightOn);

        for (int i = 0; i < lights.Count; i++)
            lights[i].gameObject.SetActive(true);

        isOn = true;
    }

    public override void TurnOff()
    {
        if (!isOn) return;

        ChangeMaterial(materialLightOff);

        for (int i = 0; i < lights.Count; i++)
            lights[i].gameObject.SetActive(false);

        isOn = false;
    }
}
