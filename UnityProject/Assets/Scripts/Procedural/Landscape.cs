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
    public Texture2D atlasTexture;
    public int atlasLines = 2;
    public int atlasColumns = 3;
    public int texResolution = 1024;

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


    private Texture2D generatedTexture;


    // Generated vertices
    private Vector3[] vertices;

    // Generated indexes
    private int[] indexes;

    private Vector2[] uvs;

    private FastNoise noise;

    private bool isReady;

	void Start ()
	{
	    if (!isReady)
	    {
	        InitTexture();
	        InitRenderer();
	        Generate();
	    }
	}

    public void InitTexture()
    {

        if (generatedTexture == null)
        {
            generatedTexture = new Texture2D(texResolution, texResolution);
            float atlasCellWidth = atlasTexture.width / (float)atlasColumns;
            float atlasCellHeight = atlasTexture.height / (float)atlasLines;

            for (int i = 0; i < texResolution; i++)
            {
                for (int j = 0; j < texResolution; j++)
                {
                    float x = j / (float) (texResolution - 1) * size;
                    float y = i / (float) (texResolution - 1) * size;
                    float altitude = heightInterval.Lerp(noise.GetNoise(transform.position.x - size / 2.0f + x, transform.position.z - size / 2.0f + y));

                    for (int index = 0; index < biomesHeight.Length; index++)
                    {
                        Interval interval = biomesHeight[index];
                        if (interval.Contains(altitude))
                        {
                            int column = index % atlasColumns;
                            int line = Mathf.FloorToInt(index / (float)(atlasLines + 1));

                            float cellX = atlasCellWidth * j / (texResolution - 1);
                            float cellY = atlasCellHeight * (i / (float)(texResolution - 1));

                            Color pixel = atlasTexture.GetPixel((int)(column * atlasCellWidth + cellX), (int)(line * atlasCellHeight + cellY));
                            generatedTexture.SetPixel(j, i, pixel);
                            break;
                        }
                    }
                }
            }
            generatedTexture.wrapMode = TextureWrapMode.Clamp;
            generatedTexture.Apply();
        }
    }

    public void InitRenderer()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.mainTexture = generatedTexture;
    }

    public void Generate()
    {
        int lodFactor = GetLODFactor();

        vertexPerLine = (vertexPerSide - 1) / lodFactor + 1;

        vertexH = vertexPerLine;
        vertexW = vertexPerLine;

        noise = new FastNoise(seed);
        noise.SetNoiseType(noiseType);

        InitMeshFilterComponent();

        if (mesh == null)
            return;

        GenerateVertices();
        GenerateIndexes();
        mesh.RecalculateNormals();

        isReady = true;
    }

    void InitMeshFilterComponent()
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
        vertices = new Vector3[vertexH * vertexW];
        uvs = new Vector2[vertexH * vertexW];
        for (int currentHeight = 0; currentHeight < vertexH; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW; currentWidth++)
            {
                float x = currentWidth / (float) (vertexW - 1) * size;
                float y = currentHeight / (float) (vertexH - 1) * size;
                float altitude = heightInterval.Lerp(noise.GetNoise(transform.position.x - size / 2.0f + x, transform.position.z - size / 2.0f + y));
                vertices[currentHeight * vertexW + currentWidth].Set(x - size / 2.0f, altitude, y - size / 2.0f);

                float widthRatio = currentWidth / (float) (vertexW - 1);
                float heightRatio = currentHeight / (float) (vertexH - 1);

                uvs[currentHeight * vertexW + currentWidth] = new Vector2(widthRatio, heightRatio);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
    }

    private void GenerateIndexes()
    {
        int i = 0;
        indexes = new int[(vertexW - 1) * 6 * (vertexH - 1)];
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

        mesh.triangles = indexes;
    }
}
