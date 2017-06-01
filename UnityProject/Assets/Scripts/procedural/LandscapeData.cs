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
		//System.Text.StringBuilder sb = new System.Text.StringBuilder();
		//sb.AppendLine("generateVertices() : vertices size : " + vertices.Length);

        for (int currentHeight = 0; currentHeight < vertexH; currentHeight++)
        {
            for (int currentWidth = 0; currentWidth < vertexW; currentWidth++)
            {
                float x = currentWidth / (float)(vertexW - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                float y = currentHeight / (float)(vertexH - 1) * LandscapeConstants.LANDSCAPE_SIZE;
                float altitude = LandscapeConstants.HEIGHT_INTERVAL.Lerp(LandscapeNoises.GetNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));

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
		if (pixels != null || atlasPixels != null)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			float atlasCellWidth = atlasWidth / (float)LandscapeConstants.ATLAS_COLUMNS;
			float atlasCellHeight = atlasHeight / (float)LandscapeConstants.ATLAS_LINES;

			for (int i = 0; i < LandscapeConstants.TEXTURE_RESOLUTION; i++)
			{
				for (int j = 0; j < LandscapeConstants.TEXTURE_RESOLUTION; j++)
				{
					float x = j / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1) * LandscapeConstants.LANDSCAPE_SIZE;
					float y = i / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1) * LandscapeConstants.LANDSCAPE_SIZE;
					float altitude = LandscapeConstants.HEIGHT_INTERVAL.Lerp(LandscapeNoises.GetNoise(position.x - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + x, position.z - LandscapeConstants.LANDSCAPE_SIZE / 2.0f + y));

					for (int index = 0; index < LandscapeConstants.BIOMES_HEIGHT.Length; index++)
					{
						Interval interval = LandscapeConstants.BIOMES_HEIGHT[index];

						if (interval.Contains(altitude))
						{
							int column = index % LandscapeConstants.ATLAS_COLUMNS;
							int line = Mathf.FloorToInt(index / (float)(LandscapeConstants.ATLAS_LINES + 1));

							float cellX = atlasCellWidth * j / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1);
							float cellY = atlasCellHeight * i / (float)(LandscapeConstants.TEXTURE_RESOLUTION - 1);

							int pixX = (int)(column * atlasCellWidth + cellX);
							int pixY = (int)(line * atlasCellHeight + cellY);
							if (pixY * atlasWidth + pixX >= atlasPixels.Length)
							{
								sb.AppendLine("TOO HIGH : trying to get pixel "
									+ pixY + " * " + atlasWidth + " + " + pixX + " = "
									+ (pixY * atlasWidth + pixX)
									+ " but atlas size is " + atlasPixels.Length);
								break;
							}

							Color pixel = atlasPixels[pixY * atlasWidth + pixX];

							if (i * LandscapeConstants.TEXTURE_RESOLUTION + j >= pixels.Length)
							{
								sb.AppendLine("TOO HIGH : trying to set pixel "
									+ i + " * " + LandscapeConstants.TEXTURE_RESOLUTION + " + " + j + " = "
									+ (i * LandscapeConstants.TEXTURE_RESOLUTION + j)
									+ " but array size is " + pixels.Length);
								break;
							}

							if (index < LandscapeConstants.BIOMES_HEIGHT.Length - 1 && interval.max - altitude < LandscapeConstants.BLEND_RANGE)
							{
								// Blend with superior
								int nextColumn = (index + 1) % LandscapeConstants.ATLAS_COLUMNS;
								int nextLine = Mathf.FloorToInt((index + 1) / (float)(LandscapeConstants.ATLAS_LINES + 1));

								int nextPixX = (int)(nextColumn * atlasCellWidth + cellX);
								int nextPixY = (int)(nextLine * atlasCellHeight + cellY);
								if (nextPixY * atlasWidth + nextPixX >= atlasPixels.Length)
								{
									sb.AppendLine("TOO HIGH : trying to get pixel "
										+ nextPixY + " * " + atlasWidth + " + " + nextPixX + " = "
										+ (nextPixY * atlasWidth + nextPixX)
										+ " but atlas size is " + atlasPixels.Length);
									break;
								}

								Color nextPixel = atlasPixels[nextPixY * atlasWidth + nextPixX];

								float diff = (interval.max - altitude) / LandscapeConstants.BLEND_RANGE;
								pixel = Blend(pixel, nextPixel, (diff + 1) / 2.0f);
							}
							else if (index > 0 && altitude - interval.min < LandscapeConstants.BLEND_RANGE)
							{
								// Blend with superior
								int prevColumn = (index - 1) % LandscapeConstants.ATLAS_COLUMNS;
								int prevLine = Mathf.FloorToInt((index - 1) / (float)(LandscapeConstants.ATLAS_LINES + 1));

								int prevPixX = (int)(prevColumn * atlasCellWidth + cellX);
								int prevPixY = (int)(prevLine * atlasCellHeight + cellY);
								if (prevPixY * atlasWidth + prevPixX >= atlasPixels.Length)
								{
									sb.AppendLine("TOO HIGH : trying to get pixel "
										+ prevPixY + " * " + atlasWidth + " + " + prevPixX + " = "
										+ (prevPixY * atlasWidth + prevPixX)
										+ " but atlas size is " + atlasPixels.Length);
									break;
								}

								Color prevPixel = atlasPixels[prevPixY * atlasWidth + prevPixX];

								float diff = (altitude - interval.min) / LandscapeConstants.BLEND_RANGE;
								pixel = Blend(pixel, prevPixel, (diff + 1) / 2.0f);
							}
							else
							{
								// Don't blend
							}
							
							pixels[i * LandscapeConstants.TEXTURE_RESOLUTION + j] = pixel;
							break;
						}
					}
				}
			}
			if (sb.Length > 0)
			{
				Debug.LogError(sb);
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
