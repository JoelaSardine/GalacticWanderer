﻿using SardineTools;
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
		//System.Text.StringBuilder sb = new System.Text.StringBuilder();
		//sb.AppendLine("generateVertices() : vertices size : " + vertices.Length);

        for (int currentHeight = 0; currentHeight < vertexH; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW; currentWidth++)
            {
                float x = currentWidth / (float)(vertexW - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                float y = currentHeight / (float)(vertexH - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                float altitude = LandscapeConstants.HEIGHT_INTERVAL.Lerp(LandscapeNoises.GetElevationNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));

                vertices[currentHeight * vertexW + currentWidth].Set(x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f, altitude, y - LandscapeConstants.LANDSCAPE_SIZE / 2.0f);

                float widthRatio = currentWidth / (float)(vertexW - 1);
                float heightRatio = currentHeight / (float)(vertexH - 1);

                UVs[currentHeight * vertexW + currentWidth] = new Vector2(widthRatio, heightRatio);
			}
			//sb.AppendLine("Height " + currentHeight + " have been generated.");
        }
		//Debug.Log(sb);
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
        for(int i = 0; i < pixels.Length; ++i)
        {
            pixels[i] = Color.red;
        }
		if (pixels != null || atlasPixels != null)
		{
			for (int i = 0; i < LandscapeConstants.TEXTURE_RESOLUTION; i++)
			{
				for (int j = 0; j < LandscapeConstants.TEXTURE_RESOLUTION; j++)
				{
                    Interval moistureInterval = new Interval(0, LandscapeNoises.GetMaxMoisture());
					float x = j / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1) * LandscapeConstants.LANDSCAPE_SIZE;
					float y = i / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1) * LandscapeConstants.LANDSCAPE_SIZE;
					float altitude = LandscapeConstants.HEIGHT_INTERVAL.Lerp(LandscapeNoises.GetElevationNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));
                    float altitudeAlpha = altitude / LandscapeConstants.HEIGHT_INTERVAL.max;
                    float moisture = moistureInterval.Lerp(LandscapeNoises.GetMoistureNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));
                    float column = moisture * WorldBuilder.biomeMapWidth;
                    float line = altitudeAlpha * WorldBuilder.biomeMapHeight;
                    if ((int)(line * WorldBuilder.biomeMapWidth + column) < WorldBuilder.biomeMapPixels.Length && (int)(line * WorldBuilder.biomeMapWidth + column) >= 0)
                    {
                        Color color = WorldBuilder.biomeMapPixels[(int)(line * WorldBuilder.biomeMapWidth + column)];
                        if((int)(i * LandscapeConstants.TEXTURE_RESOLUTION + j) < pixels.Length && (int)(i * LandscapeConstants.TEXTURE_RESOLUTION + j) >= 0)
                        pixels[(int)(i * LandscapeConstants.TEXTURE_RESOLUTION + j)] = color;
                    }
                    
				}
			}
		}
		else
		{
			if (pixels == null)
			{
				Debug.LogError("Pixels is null for landscape at " + position);
			}
			if (atlasPixels == null)
			{
				Debug.LogError("AtlasPixels is null for landscape at " + position);
			}
		}
	}

	private Color Blend(Color c1, Color c2, float percent)
	{
		Color c = new Color();
		c.r = percent * c1.r + (1 - percent) * c2.r;
		c.g = percent * c1.g + (1 - percent) * c2.g;
		c.b = percent * c1.b + (1 - percent) * c2.b;
		return c;
	}
}
