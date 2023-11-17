using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20.0f;
    public float offSetX = 100.0f;
    public float offSetY = 100.0f;
    //private Renderer renderer;

    void Start()
    {
        offSetX = Random.Range(0f, 999999f);
        offSetY = Random.Range(0f, 999999f);
    }
    void Update()
    {
        // acessando o renderizador do meu objeto
        Renderer renderer = GetComponent<Renderer>();
        // mudando a textura
        
        renderer.material.mainTexture = GenerateTexture();
        
        
    }

    Texture2D GenerateTexture()
    {
        Texture2D newTexture = new Texture2D(width, height);
            
        // gerar um mapa de ruído perlin para a texture

        // 4 loops
        // o primeiro percorre a coordenada x
        // o segundo percorre a coordenada y
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                newTexture.SetPixel(x, y, color);
            }
        }
        newTexture.Apply();
        return newTexture;
    }

    Color CalculateColor (int x, int y)
    {
        float xCoord = (float)x / width * scale + offSetX;
        float yCoord = (float)y / height * scale + offSetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color (sample,sample,sample);
    }
}
