using System.ComponentModel;
using SardineTools;
using UnityEngine;

public class Landscape : MonoBehaviour
{
    [DefaultValue(0)]
    public int seed;

    // Height bounds of the landscape
    public Interval heightInterval = new Interval(-100, 100);

    // Number of vertex on each side of the landscape chunk
    [Range(10, 255)]
    public int vertexPerSide = 201;

    // Size of the landscape chunk in Unity Units
    public float size = 100.0f;

    [Range(0, 5)]
    public int LODLevel;

    public FastNoise.NoiseType noiseType;

    // Set to true to have in-editor simulation
    public bool activatedInEditor;

    private int vertexW;
    private int vertexH;
    private int vertexPerLine;

    [HideInInspector]
    public Mesh mesh;

    // Generated vertices
    private Vector3[] vertices;

    // Generated indexes
    private int[] indexes;

    private FastNoise noise;

    private bool isReady;

	void Start ()
	{
	    if (!isReady)
	    {
	        Generate();
	    }
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
        for (int currentHeight = 0; currentHeight < vertexH; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW; currentWidth++)
            {
                float x = currentWidth / (float) (vertexW - 1) * size;
                float y = currentHeight / (float) (vertexH - 1) * size;
                float altitude = heightInterval.Lerp(noise.GetNoise(transform.position.x - size / 2.0f + x, transform.position.z - size / 2.0f + y));
                vertices[currentHeight * vertexW + currentWidth].Set(x, altitude, y);
            }
        }


        mesh.vertices = vertices;
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
