using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, Mesh}
    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int editorPreviewLOD;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public int seed;
    public int noOfWorldObjects;
    public Vector2 offset;
    public TerrainType[] regions;
    public bool autoUpdate;

    public Model[] waterObjects;
    public Model[] grassObjects;
    public Model[] groundObjects;
    public Model[] snowObjects;
    private List<GameObject> placedObjects = new List<GameObject>();
    private float lastNoiseHeight;
    private int MESH_SCALE = 9;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD);
            display.DrawMesh(meshData, TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
            PlaceObjects(meshData);
        }
    }


    void PlaceObjects(MeshData meshData)
    {
        for (var i = 0; i < placedObjects.Count; i++)
        {
            Destroy(placedObjects[i]);
        }
        
        int placed = 0;
        RaycastHit hitInfo;
        GameObject newGO;
        Model objectToSpawn = waterObjects[0];
        int interval = Mathf.RoundToInt(meshData.vertices.Length / noOfWorldObjects);
        for (int i =0; i < meshData.vertices.Length; i++)
        {
            if (i % interval == 0)
            {
                Vector3 worldPoint = transform.TransformPoint(meshData.vertices[i]);
                var noiseHeight = worldPoint.y;

                    if (noiseHeight > 0 && noiseHeight < 1)
                    {
                        // Chance to generate
                        if (UnityEngine.Random.Range(1, 5) % 2 == 0)
                        {
                        
                            objectToSpawn = waterObjects[UnityEngine.Random.Range(0, waterObjects.Length)];
                            var spawnAboveTerrainBy = noiseHeight * 2;
                            if (placed >= noOfWorldObjects)
                                break;
                   
                    
                   
                    
                            
                       }
                    }

                   if (noiseHeight >= 1 && noiseHeight < 7)
                    {

                            objectToSpawn = grassObjects[UnityEngine.Random.Range(0, grassObjects.Length)];
                            var spawnAboveTerrainBy = noiseHeight * 2;
                            if (placed >= noOfWorldObjects)
                                break;


                   

                     
                    }
                    if (noiseHeight >= 7 && noiseHeight < 10)
                    {

                     objectToSpawn = groundObjects[UnityEngine.Random.Range(0, groundObjects.Length)];
                            var spawnAboveTerrainBy = noiseHeight * 2;
                            if (placed >= noOfWorldObjects)
                                break;


                    
                    
 
                    }
                    // min height for object generation
                    if (noiseHeight >= 10)
                    {

                         objectToSpawn = snowObjects[UnityEngine.Random.Range(0, snowObjects.Length)];
                            var spawnAboveTerrainBy = noiseHeight * 2;
                            if (placed >= noOfWorldObjects)
                                break;

                    }

                if (Physics.Raycast(new Vector3(worldPoint.x * MESH_SCALE, worldPoint.y * 50, worldPoint.z * MESH_SCALE), -Vector3.up, out hitInfo))
                {
                    if (hitInfo.collider.gameObject.tag == "mesh")
                    {
                        newGO = (GameObject)Instantiate(objectToSpawn.gameObjectPrefab, hitInfo.point, Quaternion.identity);
                        placedObjects.Add(newGO);
                        placed++;
                    }
                }
                else if (Physics.Raycast(new Vector3(worldPoint.x * MESH_SCALE, worldPoint.y * 50, worldPoint.z * MESH_SCALE), Vector3.up, out hitInfo))
                {

                    if (hitInfo.collider.gameObject.tag == "mesh")
                    {
                        newGO = (GameObject)Instantiate(objectToSpawn.gameObjectPrefab, hitInfo.point, Quaternion.identity);
                        placedObjects.Add(newGO);
                        placed++;
                    }
                }

                lastNoiseHeight = noiseHeight;
            }
        }

       
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod, callback) ;
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { 
            Application.Quit();

        }
        if (mapDataThreadInfoQueue.Count >0)
        {
            for (int i =0; i < mapDataThreadInfoQueue.Count;i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize,mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset, normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y <mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i =0; i<regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 1;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

