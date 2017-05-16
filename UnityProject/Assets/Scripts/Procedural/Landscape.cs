using System.ComponentModel;
using SardineTools;
using UnityEngine;

public class Landscape : MonoBehaviour
{
    [DefaultValue(0)]
    public int seed;

    // Height bounds of the landscape
    public Interval heightInterval = new Interval(-100, 100);
    public Interval[] biomesHeight = new Interval[5];
    public int atlasLines = 2;
    public int atlasColumns = 3;
    public int texResolution = 1024;
    public int atlasWidth;
    public int atlasHeight;

    // Number of vertex on each side of the landscape chunk
    [Range(10, 255)]
    public int vertexPerSide = 201;

    // Size of the landscape chunk in Unity Units
    public float size = 100.0f;

    [Range(0, 5)]
    public int LODLevel;

    public FastNoise.NoiseType noiseType;

    [HideInInspector]
    public Mesh mesh;

    // Set to true to have in-editor simulation
    public bool activatedInEditor;

    private int vertexW;
    private int vertexH;
    private int vertexPerLine;

    public Logger logger = new Logger();


    private Texture2D generatedTexture;


    // Generated vertices
    public Vector3[] vertices;

    // Generated indexes
    public int[] indexes;

    public Vector2[] uvs;

    private FastNoise noise;

    private bool isReady;




    private Vector3 position;
    private Color[] textureData;

    [HideInInspector]
    public Color[] atlasTextureData;

    public void SetPosition(Vector3 pos) {
        position = pos;
    }

	void Start ()
	{
	}

    public void InitTexture()
    {

        if (textureData != null)
        {
            float atlasCellWidth = atlasWidth / (float)atlasColumns;
            float atlasCellHeight = atlasHeight / (float)atlasLines;

            for (int i = 0; i < texResolution; i++)
            {
                for (int j = 0; j < texResolution; j++)
                {
                    float x = j / (float) (texResolution - 1) * size;
                    float y = i / (float) (texResolution - 1) * size;
                    float altitude = heightInterval.Lerp(noise.GetNoise(position.x - size / 2.0f + x, position.z - size / 2.0f + y));

                    for (int index = 0; index < biomesHeight.Length; index++)
                    {
                        Interval interval = biomesHeight[index];
                        if (interval.Contains(altitude))
                        {
                            int column = index % atlasColumns;
                            int line = Mathf.FloorToInt(index / (float)(atlasLines + 1));

                            float cellX = atlasCellWidth * j / (texResolution - 1);
                            float cellY = atlasCellHeight * (i / (float)(texResolution - 1));

                            Color pixel = atlasTextureData[(int)(line * atlasCellHeight + cellY) * atlasWidth + (int)(column * atlasCellWidth + cellX)];
                            textureData[i * texResolution + j] = pixel;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void Generate()
    {
        if (mesh == null)
            return;

        try
        {
            GenerateVertices();
            GenerateIndexes();
        }
        catch (System.Exception e)
        {
            logger.Log(e.Message + "\n" + e.StackTrace.ToString());
        }


        isReady = true;
    }

    public void InitMeshFilterComponent()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            mesh = meshFilter.mesh;
        }
        else if (mesh != null)
        {
            mesh.Clear();
        }
    }

    void OnValidate()
    {
        isReady = false;
        if (activatedInEditor)
        {
            Start();
        }
        else if (GetComponent<MeshFilter>() != null && mesh != null)
        {
            mesh.Clear();
        }
    }

    int GetLODFactor()
    {
        int iteration = 0;
        int currentLodFactor = 1;
        int targetLod = LODLevel + 1;
        int maxLod = vertexPerSide - 1;
        for (int i = 1; i < maxLod; i++)
        {
            if (maxLod % i == 0)
            {
                currentLodFactor = i;
                iteration++;

                if (iteration == targetLod)
                    break;
            }
        }

        return currentLodFactor;
    }

    private void GenerateVertices()
    {
        logger.Log("generateVertices() : vertices size : " + vertices.Length);

        for (int currentHeight = 0; currentHeight < vertexH; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW; currentWidth++)
            {
                float x = currentWidth / (float) (vertexW - 1) * size;
                float y = currentHeight / (float) (vertexH - 1) * size;
                float altitude = heightInterval.Lerp(noise.GetNoise(position.x - size / 2.0f + x, position.z - size / 2.0f + y));

                logger.Log("Trying to set vertices at " + currentWidth + "," + currentHeight);
                vertices[currentHeight * vertexW + currentWidth].Set(x - size / 2.0f, altitude, y - size / 2.0f);

                float widthRatio = currentWidth / (float) (vertexW - 1);
                float heightRatio = currentHeight / (float) (vertexH - 1);

                uvs[currentHeight * vertexW + currentWidth] = new Vector2(widthRatio, heightRatio);
            }
        }
    }

    private void GenerateIndexes()
    {
        int i = 0;
        for (int currentHeight = 0; currentHeight < vertexH - 1; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW - 1; currentWidth++)
            {
                indexes[i] = currentHeight * vertexW + currentWidth;
                indexes[i + 1] = (currentHeight + 1) * vertexW + currentWidth;
                indexes[i + 2] = (currentHeight + 1) * vertexW + currentWidth + 1;

                indexes[i + 3] = currentHeight * vertexW + currentWidth;
                indexes[i + 4] = (currentHeight + 1) * vertexW + currentWidth + 1;
                indexes[i + 5] = currentHeight * vertexW + currentWidth + 1;

                i += 6;
            }
        }
    }

    public void AllocateMemory()
    {
        int lodFactor = GetLODFactor();

        vertexPerLine = (vertexPerSide - 1) / lodFactor + 1;

        vertexH = vertexPerLine;
        vertexW = vertexPerLine;

        textureData = new Color[texResolution * texResolution];
        noise = new FastNoise(seed);
        vertices = new Vector3[vertexH * vertexW];
        Debug.Log("Allocated " + vertices.Length + " vertices");
        uvs = new Vector2[vertexH * vertexW];
        indexes = new int[(vertexW - 1) * 6 * (vertexH - 1)];

        noise.SetNoiseType(noiseType);
    }

    public void BindDataToMesh()
    {
        Debug.Log("binding == vertices.length : " + vertices.Length + " indexes.length : " + indexes.Length + " uvs.length : " + uvs.Length);
        mesh.vertices = vertices;
        mesh.triangles = indexes;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        generatedTexture = new Texture2D(texResolution, texResolution);
        generatedTexture.wrapMode = TextureWrapMode.Clamp;
        generatedTexture.SetPixels(textureData);
        generatedTexture.Apply();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.mainTexture = generatedTexture;
    }
}
