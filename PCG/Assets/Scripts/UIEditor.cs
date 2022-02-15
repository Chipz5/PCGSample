using UnityEngine;
using UnityEngine.UI;
public class UIEditor : MonoBehaviour
{
    public InputField seed;
    public InputField worldObject;
    int seedVal;
    int worldObjectVal;

    MapGenerator mapGenerator;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        mapGenerator.DrawMapInEditor();
    }

    private void Update()
    {
        if (mapGenerator.seed != seedVal)
        {
            seed.placeholder.GetComponent<Text>().text = mapGenerator.seed.ToString();
        }
        if (mapGenerator.noOfWorldObjects != worldObjectVal)
        {
            worldObject.placeholder.GetComponent<Text>().text = mapGenerator.noOfWorldObjects.ToString();
        }

    }
    public void MakeWorld()
    {
        if(string.IsNullOrEmpty(seed.text))
            seedVal = mapGenerator.seed;
        else
            seedVal = int.Parse(seed.text);

        if (string.IsNullOrEmpty(worldObject.text))
            worldObjectVal = mapGenerator.noOfWorldObjects;
        else
            worldObjectVal = int.Parse(worldObject.text);

        if (worldObjectVal < 25)
            worldObjectVal = 25;
        else if (worldObjectVal > 1000)
        {
            worldObjectVal = 1000;
        }
        if (mapGenerator.seed != seedVal || mapGenerator.noOfWorldObjects != worldObjectVal)
        {
            mapGenerator.seed = seedVal;
            mapGenerator.noOfWorldObjects = worldObjectVal;
            mapGenerator.DrawMapInEditor();
        }
    }

}
