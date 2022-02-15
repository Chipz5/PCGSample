using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tree Model", menuName = "Models/Tree", order = 1)]
public class TreeModel : Model
{
    public Material bark;
    public Material leaves;

    private void Awake()
    {
        if (bark && leaves)
        {
            Renderer rend = gameObjectPrefab.GetComponent<Renderer>();
            if (rend != null)
            {
                Material[] allMaterials;
                allMaterials = rend.materials;

                //Change the materials you want to change now
                allMaterials[0] = bark;
                allMaterials[1] = leaves;
                rend.materials = allMaterials;
            }
        }
    }
}
