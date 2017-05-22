using System.ComponentModel;
using SardineTools;
using UnityEngine;

public class Landscape : MonoBehaviour
{
    /// <summary>
    /// Holds the mesh data 
    /// </summary>
    LandscapeData landscapeData;

    MeshRenderer renderer;
    Mesh mesh;

    /// <summary>
    /// Texture generated from atlas texture based on generated mesh
    /// </summary>
    Texture2D generatedTexture;

    /// <summary>
    /// When true, this means that this landscape is in a worker's pipeline
    /// </summary>
    public bool isInQueue = false;

    /// <summary>
    /// When true, this means we can bind generated landscape's datas to Unity classes
    /// </summary>
    public bool isDirty = false;

    /// <summary>
    /// When false, this means we have to initialized this mesh's texture
    /// </summary>
    private bool initialized = false;

    public string cachedName {
        get;
        private set;
    }

    public Landscape()
    {
        landscapeData = new LandscapeData();
    }

    void Awake()
    {
        mesh = gameObject.AddComponent<MeshFilter>().mesh;
        renderer = gameObject.AddComponent<MeshRenderer>();
        generatedTexture = new Texture2D(LandscapeConstants.TEXTURE_RESOLUTION, LandscapeConstants.TEXTURE_RESOLUTION);
        generatedTexture.wrapMode = TextureWrapMode.Clamp;
        landscapeData.currentLOD = -1;
        cachedName = gameObject.name;
    }

    void Update()
    {
        if (isDirty)
        {
            ApplyData();
            isDirty = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.9f);
        Gizmos.DrawCube(transform.position, new Vector3(LandscapeConstants.LANDSCAPE_SIZE * 0.9f, 1, LandscapeConstants.LANDSCAPE_SIZE * 0.9f));
    }

    void ApplyData()
    {
        landscapeData.currentLOD = landscapeData.nextLOD;

        mesh.vertices = landscapeData.vertices;
        mesh.triangles = landscapeData.indexes;
        mesh.uv = landscapeData.UVs;
        mesh.RecalculateNormals();

        // Initialize the texture the first time we enter this method
        if (!initialized)
        {
            generatedTexture.SetPixels(landscapeData.pixels);
            generatedTexture.Apply();
            renderer.material.mainTexture = generatedTexture;
            initialized = true;
        }
    }

    public LandscapeData GetLandscapeData()
    {
        return landscapeData;
    }

}
