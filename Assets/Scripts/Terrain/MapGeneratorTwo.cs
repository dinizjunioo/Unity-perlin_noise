using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
//using static UnityEditor.PlayerSettings.SplashScreen;
using System.Threading;
//using static Unity.Collections.NativeArray<T>;
using static MapData;
using UnityEditor.SceneManagement;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class MapGeneratorTwo : MonoBehaviour
{

    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;

    //public int mapWidth;
    //public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    // vamos criar uma estrutura de dados fila para as threads 
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    // 
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public GameObject prefabTree;
    public List<GameObject> arvoresPrefab;
    public Queue<Vector3> arvores2 = new Queue<Vector3>();
    public Dictionary<Vector3,int> arvores = new Dictionary<Vector3, int>();
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        arvores2.Enqueue( Vector3.one);
        //Debug.Log("--->" + arvores2.Count);
        if (arvores.Count > 0)
        {
            foreach (KeyValuePair<Vector3, int> pair in arvores)
            {
                Vector3 treePosition = pair.Key;
                if (prefabTree != null)
                {
                    GameObject tree = Instantiate(prefabTree, treePosition, Quaternion.identity);
                    arvoresPrefab.Add(tree);
                }

            }
                //Debug.Log("->" + arvores.Count);
           // for (int i = 0; i < arvores.Count/10; i++)
           // {

                //   Debug.Log("->" + arvores.Count + " - i -> " + i);
                //Vector3 position = arvores.;
                //if (prefabTree != null)
                //   Instantiate(prefabTree, position, Quaternion.identity);
           // }
        }

        MapDisplayTwo display = FindObjectOfType<MapDisplayTwo>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGeneratorTwo.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), 
                TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate {
            MapDataThread(centre, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGeneratorTwo.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo =  mapDataThreadInfoQueue.Dequeue();
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

    void OnApplicationQuit()
    {
        foreach(GameObject gameObjects in arvoresPrefab)
        {
            Destroy(gameObjects);
        }
        arvoresPrefab.Clear();
    }
    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset);
        int num = 0;
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                
                //Vector3 treePosition = new Vector3(x, currentHeight, y);
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                       
                        Vector3 treePosition = new Vector3(4, 1, -1);
                        //Debug.Log("Adi " + currentHeight);
                        if ( (regions[i].height == 0.305f))
                        {
                            
                            // calcula a posi��o de inst�ncia da �rvore
                            //Debug.Log("Adicionando �rvore em x: " + x + ", y: " + y);
                            lock (arvores)
                            {
                                 //Debug.Log("x->" + x + "-y->" + y);
                                if (!arvores.TryGetValue(treePosition, out num) && arvores.Count < 2)
                                {
                                    arvores.Add(treePosition, num);
                                    // A posi��o j� existe, voc� pode usar 'numeroExistente' se necess�rio
                                    //Debug.Log("A posi��o j� existe, N�mero: " + numeroExistente);
                                }
                            }
                            num++;
                        }

                        break;
                    }

                    
                }
               
            }
        }
        //Debug.Log("total de arvores criadas -> " + num);
        return new(noiseMap, colourMap);
    }


    void CriarArvores(Vector3 position)//, Transform transform)
    {
        // Verifica se o prefab da �rvore est� atribu�do
        Debug.Log("craindo uma arvore 2...");
        if (prefabTree != null)
        {
            Debug.Log("craindo uma arvore 3 ...");
            // Cria uma inst�ncia da �rvore como filho do MapGenerator
            //GameObject treeInstance =
            Instantiate(prefabTree, position, Quaternion.identity);
            
            // Adiciona a �rvore � cena ou a um cont�iner, dependendo do que voc� precisar
            // treeInstance.transform.parent = seuContainerTransform;
        }
        else
        {
            Debug.LogError("Prefab da �rvore n�o atribu�do. Atribua o prefab da �rvore � vari�vel 'prefabTree' no Inspector.");
        }
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }

    // lida tanto com dados do mapData como do MeshData
public struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        // -> alt-enter
        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }


    // A classe do terreno vai socilitar o map data.
    // quando isso acontecer vamos passar um m�todo como vari�vel
    // chamaremos esse m�todo de dadados recebidos
    // cria-se uma referencia entre o atributo callback e esse m�todo "onMapDataReceived"
    // ou seja, assim que acabar de criar os mapData chamamos "callback" o m�todo que vai receber
    // e tratar esses dados
    // tudo isso ser� respons�vel por inicializar o mapData thread
   /* private void Start()
    {
        RequestMapData(onMapDataReceived);
    }

    void onMapDataReceived(MapData mapData)
    {

    }
   */
}
/*
    public void Start()
    {
        // Certifique-se de que o prefab n�o � nulo
        if (prefabTree != null)
        {
            // Obt�m o componente Renderer do prefab
            Renderer prefabRenderer = prefabTree.GetComponent<Renderer>();

            // Certifique-se de que o prefab possui um componente Renderer
            if (prefabRenderer != null)
            {
                // Obt�m os bounds do prefab
                Bounds bounds = prefabRenderer.bounds;

                // A largura do objeto � a diferen�a entre as coordenadas x m�xima e m�nima da caixa delimitadora
                float width = bounds.size.x;

                // A altura do objeto � a diferen�a entre as coordenadas y m�xima e m�nima da caixa delimitadora
                float height = bounds.size.y;

                Debug.Log("A largura do prefab �: " + width);
                Debug.Log("A altura do prefab �: " + height);
            }
            else
            {
                Debug.LogError("O prefab n�o possui um componente Renderer.");
            }
        }
        else
        {
            Debug.LogError("O prefab � nulo. Atribua um prefab ao campo 'prefab' no Inspector.");
        }
    }*/