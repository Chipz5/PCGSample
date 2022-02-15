using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Forage Model", menuName = "Models/Forage", order = 1)]
public class ForageModel : Model
{
    public Material top;
    public Material bottom;

    private void Awake()
    {
        if (top && bottom)
        {
            Renderer rend = gameObjectPrefab.GetComponent<Renderer>();
            if (rend != null)
            {
                Material[] allMaterials;
                allMaterials = rend.materials;

                //Change the materials you want to change now
                allMaterials[0] = top;
                allMaterials[1] = bottom;
                rend.materials = allMaterials;
            }
        }
    }
}
