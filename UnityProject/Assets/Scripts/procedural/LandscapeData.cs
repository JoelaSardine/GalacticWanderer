using SardineTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeData {

    public Vector3 position;
    public int[] indexes;
    public Vector3[] vertices;
    public Vector2[] UVs;
    public Color[] pixels;

    /// <summary>
    /// Next desired LOD for this landscape
    /// </summary>
    public int nextLOD;

    /// <summary>
    /// Current Level Of Details
    /// </summary>
    public int currentLOD
    {
        get;
        set;
    }


    private int atlasWidth;
    private int atlasHeight;
    private Color[] atlasPixels;

    private int vertexW;
    private int vertexH;

    // Generate mesh :
    //      - vertices
    //      - indexes
    //      - UVS
    public void GenerateMesh(int LOD)
    {
        AllocateMemory(LOD);
        GenerateVertices();
        GenerateIndexes();
    }

    int GetLODFactor(int LOD)
    {
        int iteration = 0;
        int currentLodFactor = 1;
        int targetLod = LOD + 1;
        int maxLod = LandscapeConstants.VERTEX_PER_SIDE - 1;
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

    private void AllocateMemory(int LOD)
    {
        currentLOD = LOD;
        int lodFactor = GetLODFactor(currentLOD);
        int vertexPerLine = (LandscapeConstants.VERTEX_PER_SIDE - 1) / lodFactor + 1;

        vertexH = vertexPerLine;
        vertexW = vertexPerLine;

        pixels = new Color[LandscapeConstants.TEXTURE_RESOLUTION * LandscapeConstants.TEXTURE_RESOLUTION];
        vertices = new Vector3[vertexH * vertexW];

        UVs = new Vector2[vertexH * vertexW];
        indexes = new int[(vertexW - 1) * 6 * (vertexH - 1)];
    }

    private void GenerateVertices()
    {
        Debug.Log("generateVertices() : vertices size : " + vertices.Length);

        for (int currentHeight = 0; currentHeight < vertexH; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW; currentWidth++)
            {
                float x = currentWidth / (float)(vertexW - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                float y = currentHeight / (float)(vertexH - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                float altitude = LandscapeConstants.HEIGHT_INTERVAL.Lerp(LandscapeConstants.NOISE.GetNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));

                vertices[currentHeight * vertexW + currentWidth].Set(x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f, altitude, y - LandscapeConstants.LANDSCAPE_SIZE / 2.0f);

                float widthRatio = currentWidth / (float)(vertexW - 1);
                float heightRatio = currentHeight / (float)(vertexH - 1);

                UVs[currentHeight * vertexW + currentWidth] = new Vector2(widthRatio, heightRatio);
            }
            Debug.Log("Trying to set vertices at height " + currentHeight);
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

    public void BindAtlasPixels(Color[] pixels, int width, int height)
    {
        atlasPixels = pixels;
        atlasWidth = width;
        atlasHeight = height;
    }

    public void BindPosition(Vector3 pos)
    {
        position = pos;
    }

    // Generate landscape's texture using atlas texture
    // Pre : mesh already generated
    public void GenerateTexture()
    {
        if (pixels != null)
        {
            float atlasCellWidth = atlasWidth / (float)LandscapeConstants.ATLAS_COLUMNS;
            float atlasCellHeight = atlasHeight / (float)LandscapeConstants.ATLAS_LINES;

            for (int i = 0; i < LandscapeConstants.TEXTURE_RESOLUTION; i++)
            {
                for (int j = 0; j < LandscapeConstants.TEXTURE_RESOLUTION; j++)
                {
                    float x = j / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                    float y = i / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                    float altitude = LandscapeConstants.HEIGHT_INTERVAL.Lerp(LandscapeConstants.NOISE.GetNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));

                    for (int index = 0; index < LandscapeConstants.BIOMES_HEIGHT.Length; index++)
                    {
                        Interval interval = LandscapeConstants.BIOMES_HEIGHT[index];
                        if (interval.Contains(altitude))
                        {
                            int column = index % LandscapeConstants.ATLAS_COLUMNS;
                            int line = Mathf.FloorToInt(index / (float)(LandscapeConstants.ATLAS_LINES + 1));

                            float cellX = atlasCellWidth * j / (LandscapeConstants.TEXTURE_RESOLUTION - 1);
                            float cellY = atlasCellHeight * (i / (float)(LandscapeConstants.LANDSCAPE_SIZE - 1));

                            Color pixel = atlasPixels[(int)(line * atlasCellHeight + cellY) * atlasWidth + (int)(column * atlasCellWidth + cellX)];
                            pixels[i * LandscapeConstants.TEXTURE_RESOLUTION + j] = pixel;
                            break;
                        }
                    }
                }
            }
        }
    }
}
