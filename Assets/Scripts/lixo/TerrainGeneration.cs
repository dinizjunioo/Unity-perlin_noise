using UnityEngine;
using UnityEngine.UIElements;

public class TerrainGeneration : MonoBehaviour
{

    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public int scale = 20;

    private void Update()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain ( TerrainData terrainData)
    {
        // onde a coord x é a largura, y profundidade e z é a altura
        terrainData.size = new Vector3(width, depth, height);

        // mudando a altura 
        terrainData.SetHeights(0, 0,GenerationHeight());
        return terrainData;
    }


    // fluxo bidimensional de float
    float [,] GenerationHeight()
    {
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x,y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale; //+  offSetX;
        float yCoord = (float)y / height * scale; //+ offSetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }


    
    // classe que vai guardar informações sobre os pedaços de terreno
    // como o próprio objeto e a posicao.
    // se o objeto estiver distante ele desativará esse objeto
    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, int size)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f;
            SetVisible(false);            
        }

        public void Update()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(EndlessTerrain.viewerPosition));
            bool visible = viewerDstFromNearestEdge <= EndlessTerrain.maxViewDst;
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
    }
}
